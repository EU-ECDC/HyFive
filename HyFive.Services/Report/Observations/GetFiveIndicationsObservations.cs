using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Report.FiveIndications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;

namespace HyFive.Services.Report.Observations
{
    public class GetFiveIndicationsObservations
    {
        public class Query : IRequest<IEnumerable<FiveIndicationsObservationReport>>
        {
            public List<int> DepartmentIds { get; set; }
            public int? DepartmentId { get; set; }
            public Guid? SessionId { get; set; }
            public int ObserverId { get; set; }
            public List<int> FacilityIds { get; set; }
            public int? FacilityId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<FiveIndicationsObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;


            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<FiveIndicationsObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                // 1) Resolve which UNIT OrganisationUnits we are allowed to include
                var unitIds = await ResolveUnitIdsAsync(query, cancellationToken);

                // If nothing selected, return empty (or decide to return "all" depending on your UX)
                if (unitIds.Count == 0)
                    return Array.Empty<FiveIndicationsObservationReport>();

                // 2) Base query: Observations + Session (UnitId is on session)
                var queryable = _context.FiveIndicationsObservation
                    .AsNoTracking()
                    .Include(o => o.FiveIndicationsSession).ThenInclude(s => s.Observer)
                    .Include(o => o.FiveIndicationsSession).ThenInclude(s => s.TransferStatus)
                    .Include(o => o.Activity)
                    .Include(o => o.IndicationTypes)
                    .Include(o => o.Role)
                    .Where(o => unitIds.Contains(o.FiveIndicationsSession.OrganisationUnitId));

                // 3) TransferStatus filtering based on role
                if (query.Role == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(o => o.FiveIndicationsSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }

                if (query.ObserverId > 0)
                    queryable = queryable.Where(o => o.FiveIndicationsSession.ObserverId == query.ObserverId);

                if (query.SessionId.HasValue)
                    queryable = queryable.Where(o => o.FiveIndicationsSession.Id == query.SessionId.Value);

                if (query.FromDate.HasValue)
                {
                    var fromUtc = DateTime.SpecifyKind(query.FromDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.RegisteredTime.Date >= fromUtc);
                }

                if (query.ToDate.HasValue)
                {
                    var toUtc = DateTime.SpecifyKind(query.ToDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.RegisteredTime.Date <= toUtc);
                }

                return await queryable
                    .OrderBy(o => o.FiveIndicationsSession.Id)
                    .ThenBy(o => o.Id)
                    .ProjectTo<FiveIndicationsObservationReport>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }

            /// <summary>
            /// Converts FacilityIds / DepartmentIds (old API contract) into Unit OrganisationUnitIds.
            /// </summary>
            private async Task<HashSet<int>> ResolveUnitIdsAsync(Query query, CancellationToken ct)
            {
                // Load minimal org-unit graph info once
                var ous = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Select(x => new OrganisationUnitNode(
                        x.Id,
                        x.ParentId,
                        x.LevelRef.Level
                    ))
                    .ToListAsync(ct);

                var childrenByParent = ous
                .Where(x => x.ParentId.HasValue)
                .GroupBy(x => x.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

                var result = new HashSet<int>();

                // If caller uses the "single" fields, normalize into lists (keep old behavior)
                var facilityRoots = (query.FacilityIds ?? new List<int>())
                    .Concat(query.FacilityId.HasValue && query.FacilityId.Value > 0 ? new[] { query.FacilityId.Value } : Array.Empty<int>())
                    .Distinct()
                    .ToList();

                var deptRoots = (query.DepartmentIds ?? new List<int>())
                    .Concat(query.DepartmentId.HasValue && query.DepartmentId.Value > 0 ? new[] { query.DepartmentId.Value } : Array.Empty<int>())
                    .Distinct()
                    .ToList();

                // Facility -> Departments -> Units
                foreach (var facilityId in facilityRoots)
                {
                    var deptIds = GetDescendantsByLevel(
                        rootId: facilityId,
                        targetLevel: OrganisationUnitLevels.Department,
                        ous: ous,
                        childrenByParent: childrenByParent
                    );

                    foreach (var deptId in deptIds)
                    {
                        var unitIds = GetDescendantsByLevel(
                            rootId: deptId,
                            targetLevel: OrganisationUnitLevels.Unit,
                            ous: ous,
                            childrenByParent: childrenByParent
                        );

                        foreach (var u in unitIds)
                            result.Add(u);
                    }
                }

                // Department -> Units
                foreach (var deptId in deptRoots)
                {
                    var unitIds = GetDescendantsByLevel(
                        rootId: deptId,
                        targetLevel: OrganisationUnitLevels.Unit,
                        ous: ous,
                        childrenByParent: childrenByParent
                    );

                    foreach (var u in unitIds)
                        result.Add(u);
                }

                return result;
            }

            private static List<int> GetDescendantsByLevel(
                int rootId,
                string targetLevel,
                List<OrganisationUnitNode> ous,
                Dictionary<int, List<int>> childrenByParent)
            {
                var byId = ous.ToDictionary(x => x.Id);

                var results = new List<int>();
                var stack = new Stack<int>();
                stack.Push(rootId);

                var safety = 0;
                while (stack.Count > 0 && safety++ < 100_000)
                {
                    var current = stack.Pop();

                    if (byId.TryGetValue(current, out var node))
                    {
                        if (string.Equals(node.Level, targetLevel, StringComparison.OrdinalIgnoreCase))
                            results.Add(node.Id);
                    }

                    if (childrenByParent.TryGetValue(current, out var children))
                    {
                        foreach (var childId in children)
                            stack.Push(childId);
                    }
                }

                return results;
            }

            private sealed record OrganisationUnitNode(int Id, int? ParentId, string Level);
        }
    }
}
