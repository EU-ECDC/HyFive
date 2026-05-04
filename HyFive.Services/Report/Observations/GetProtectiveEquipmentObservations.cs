using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Report.ProtectiveEquipment;
using HyFive.Models.V1.Report.Glove;
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
    public class GetProtectiveEquipmentObservations
    {
        public class Query : IRequest<IEnumerable<PPEObservationReport>>
        {
            public List<int> DepartmentIds { get; set; }
            public int DepartmentId { get; set; }
            public Guid? SessionId { get; set; }
            public int ObserverId { get; set; }
            public List<int> FacilityIds { get; set; }
            public int FacilityId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<PPEObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;


            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<PPEObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                // 1) Decide transfer-status filtering (same idea as before)
                // Adjust if you want Coordinator/Observer allowed here.
                var transferCode = query.Role switch
                {
                    AuthorizedRole.Administrator => TransferStatusTypeConstants.TransferredToAdmin,
                    AuthorizedRole.Coordinator => TransferStatusTypeConstants.TransferredToCoordinator,
                    _ => null
                };

                // 2) Translate FacilityIds/DepartmentIds -> UnitIds (Unit ids where Level=Unit)
                var unitIds = await ResolveUnitIdsAsync(query, cancellationToken);

                // 3) Base query
                var queryable = _context.ProtectiveEquipmentObservation
                    .AsNoTracking()
                    .Include(o => o.ProtectiveEquipmentSession).ThenInclude(s => s.Observer)
                    .Include(o => o.ProtectiveEquipmentSession).ThenInclude(s => s.TransferStatus)
                    .Include(o => o.ProtectiveEquipmentSession).ThenInclude(s => s.OrganisationUnit)
                    .Include(o => o.SettingType)
                        .ThenInclude(st => st.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes)
                        .ThenInclude(link => link.ProtectiveEquipmentType)
                    .Include(o => o.ProtectiveEquipmentList).ThenInclude(p => p.MisuseTypes)
                    .Include(o => o.ProtectiveEquipmentList).ThenInclude(p => p.EquipmentType)
                    .Include(o => o.Role)
                    .SelectMany(o => o.ProtectiveEquipmentList) 
                    .AsQueryable();

                // 4) Apply filters
                if (!string.IsNullOrEmpty(transferCode))
                {
                    queryable = queryable.Where(x =>
                        x.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.TransferStatus.Code == transferCode);
                }

                if (unitIds.Count > 0)
                {
                    queryable = queryable.Where(x =>
                        unitIds.Contains(x.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.OrganisationUnitId));
                }

                if (query.ObserverId > 0)
                {
                    queryable = queryable.Where(x =>
                        x.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.ObserverId == query.ObserverId);
                }

                if (query.SessionId.HasValue)
                {
                    queryable = queryable.Where(x =>
                        x.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id == query.SessionId.Value);
                }

                if (query.FromDate != null)
                {
                    var fromDateUtc = DateTime.SpecifyKind(query.FromDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.RegisteredTime.Date >= fromDateUtc);
                }

                if (query.ToDate != null)
                {
                    var toDateUtc = DateTime.SpecifyKind(query.ToDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.RegisteredTime.Date <= toDateUtc);
                }

                return await queryable
                                    .OrderBy(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id)
                                    .ThenBy(o => o.ProtectiveEquipmentObservation.Id)
                                    .ProjectTo<PPEObservationReport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }

            private sealed record OuNode(int Id, int? ParentId, string Level);

            private async Task<HashSet<int>> ResolveUnitIdsAsync(Query query, CancellationToken ct)
            {
                // Gather roots coming from request (we keep FE DTO intact)
                var roots = new HashSet<int>();

                if (query.FacilityIds?.Count > 0) foreach (var id in query.FacilityIds) roots.Add(id);
                else if (query.FacilityId > 0) roots.Add(query.FacilityId);

                if (query.DepartmentIds?.Count > 0) foreach (var id in query.DepartmentIds) roots.Add(id);
                else if (query.DepartmentId > 0) roots.Add(query.DepartmentId);

                if (roots.Count == 0)
                    return new HashSet<int>(); // means "no OU filter"

                // Load OU graph (Id, ParentId, Level) once
                var ous = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Select(x => new OuNode(x.Id, x.ParentId, x.LevelRef.Level))
                    .ToListAsync(ct);

                var childrenByParent = ous
                    .Where(x => x.ParentId.HasValue)
                    .GroupBy(x => x.ParentId!.Value)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

                // For each root, find all descendant Unit ids
                var unitIds = new HashSet<int>();

                foreach (var rootId in roots)
                {
                    CollectDescendantsByLevel(
                        rootId: rootId,
                        targetLevel: OrganisationUnitLevels.Unit,
                        ous: ous,
                        childrenByParent: childrenByParent,
                        results: unitIds
                    );
                }

                return unitIds;
            }

            private static void CollectDescendantsByLevel(
                int rootId,
                string targetLevel,
                List<OuNode> ous,
                Dictionary<int, List<int>> childrenByParent,
                HashSet<int> results)
            {
                var byId = ous.ToDictionary(x => x.Id);

                // BFS
                var queue = new Queue<int>();
                queue.Enqueue(rootId);

                var safety = 0;
                while (queue.Count > 0 && safety++ < 100_000)
                {
                    var current = queue.Dequeue();

                    if (byId.TryGetValue(current, out var node) &&
                        string.Equals(node.Level, targetLevel, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(node.Id);
                    }

                    if (childrenByParent.TryGetValue(current, out var children))
                    {
                        foreach (var childId in children)
                            queue.Enqueue(childId);
                    }
                }
            }

        }
    }
}
