using HyFive.DataAccess;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Report.Observations
{
    public class ReportForSessionTypeHasData
    {
        public class Query : IRequest<bool>
        {
            public int SessionType { get; set; }
            public List<int> FacilityIds { get; set; }
            public List<int>? DepartmentIds { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Query query, CancellationToken cancellationToken)
            {
                if (query.FacilityIds == null || query.FacilityIds.Count == 0)
                    return false;

                var hasData = false;

                var fromDateUtc = DateTime.SpecifyKind(query.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(query.ToDate.Date, DateTimeKind.Utc);

                var unitIds = await ResolveUnitIds(query.FacilityIds, query.DepartmentIds, cancellationToken);

                if (unitIds.Count == 0)
                    return false;

                // 2) TransferStatus rule (adapt if Coordinator/Observer should differ)
                var requireTransferredToAdmin = query.Role == AuthorizedRole.Administrator;

                // 3) Query per type
                return query.SessionType switch
                {
                    (int)SessionType.FiveIndications => await HasFiveIndications(unitIds, fromDateUtc, toDateUtc, requireTransferredToAdmin, cancellationToken),
                    (int)SessionType.HandJewelry => await HasHandJewelry(unitIds, fromDateUtc, toDateUtc, requireTransferredToAdmin, cancellationToken),
                    (int)SessionType.Gloves => await HasGloves(unitIds, fromDateUtc, toDateUtc, requireTransferredToAdmin, cancellationToken),
                    (int)SessionType.ProtectiveEquipment => await HasProtectiveEquipment(unitIds, fromDateUtc, toDateUtc, requireTransferredToAdmin, cancellationToken),
                    _ => false
                };
            }

            // Returns OrganisationUnitIds for Units under the provided facilities and (optionally) departments.
            private async Task<HashSet<int>> ResolveUnitIds(
                List<int> facilityIds,
                List<int>? departmentIds,
                CancellationToken ct)
            {
                // Load the minimal OU graph needed (Id, ParentId, Level)
                // We fetch all OUs under the selected facilities (and optionally under selected departments)
                // Since we have a tree, easiest is: load all OUs and compute descendants in-memory
                // If huge, we can switch to a recursive CTE.

                var ous = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Select(o => new
                    {
                        o.Id,
                        o.ParentId,
                        Level = o.LevelRef.Level // relies on relationship; EF will translate if configured
                    })
                    .ToListAsync(ct);

                // Build lookup childrenByParent
                var childrenByParent = ous
                .Where(o => o.ParentId.HasValue)
                .GroupBy(o => o.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());


                // If departments are provided, treat them as roots; otherwise facilities are roots
                var roots = (departmentIds != null && departmentIds.Count > 0)
                    ? departmentIds
                    : facilityIds;

                // Traverse descendants and keep only Unit-level
                var unitIds = new HashSet<int>();
                var visited = new HashSet<int>();
                var stack = new Stack<int>(roots);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (!visited.Add(current))
                        continue;

                    var node = ous.FirstOrDefault(x => x.Id == current);
                    if (node != null && node.Level == OrganisationUnitLevels.Unit)
                        unitIds.Add(node.Id);

                    if (childrenByParent.TryGetValue(current, out var children))
                    {
                        foreach (var childId in children)
                            stack.Push(childId);
                    }
                }

                return unitIds;
            }

            private Task<bool> HasFiveIndications(
           IReadOnlyCollection<int> unitIds,
           DateTime fromUtc,
           DateTime toUtc,
           bool transferredToAdminOnly,
           CancellationToken ct)
            {
                var q = _context.FiveIndicationsObservation.AsNoTracking()
                    .Where(o => unitIds.Contains(o.FiveIndicationsSession.OrganisationUnitId))
                    .Where(o => o.RegisteredTime >= fromUtc && o.RegisteredTime <= toUtc);

                if (transferredToAdminOnly)
                    q = q.Where(o => o.FiveIndicationsSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return q.AnyAsync(ct);
            }

            private Task<bool> HasHandJewelry(
                IReadOnlyCollection<int> unitIds,
                DateTime fromUtc,
                DateTime toUtc,
                bool transferredToAdminOnly,
                CancellationToken ct)
            {
                var q = _context.HandJewelryObservation.AsNoTracking()
                    .Where(o => unitIds.Contains(o.HandJewelrySession.OrganisationUnitId))
                    .Where(o => o.RegisteredTime >= fromUtc && o.RegisteredTime <= toUtc);

                if (transferredToAdminOnly)
                    q = q.Where(o => o.HandJewelrySession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return q.AnyAsync(ct);
            }

            private Task<bool> HasGloves(
                IReadOnlyCollection<int> unitIds,
                DateTime fromUtc,
                DateTime toUtc,
                bool transferredToAdminOnly,
                CancellationToken ct)
            {
                var q = _context.GloveObservation.AsNoTracking()
                    .Where(o => unitIds.Contains(o.GloveSession.OrganisationUnitId))
                    .Where(o => o.RegisteredTime >= fromUtc && o.RegisteredTime <= toUtc);

                if (transferredToAdminOnly)
                    q = q.Where(o => o.GloveSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return q.AnyAsync(ct);
            }

            private Task<bool> HasProtectiveEquipment(
                IReadOnlyCollection<int> unitIds,
                DateTime fromUtc,
                DateTime toUtc,
                bool transferredToAdminOnly,
                CancellationToken ct)
            {
                var q = _context.ProtectiveEquipmentObservation.AsNoTracking()
                    .Where(o => unitIds.Contains(o.ProtectiveEquipmentSession.OrganisationUnitId))
                    .Where(o => o.RegisteredTime >= fromUtc && o.RegisteredTime <= toUtc);

                if (transferredToAdminOnly)
                    q = q.Where(o => o.ProtectiveEquipmentSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return q.AnyAsync(ct);
            }
        }
    }
}
