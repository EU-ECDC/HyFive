using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class GetFacilitiesWithSessions
    {
        public class Query : IRequest<List<FacilityOverviewReport>>
        {
            public SessionType? SessionType { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? FacilityId { get; set; }
            public string TransferStatusType { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<FacilityOverviewReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            private sealed class OrganisationUnitNode
            {
                public int Id { get; init; }
                public int? ParentId { get; init; }
                public string Name { get; init; } = "";
                public string Level { get; init; } = "";
            }


            public async Task<List<FacilityOverviewReport>> Handle(Query query, CancellationToken ct)
            {
                var allowedTransferCodes = TransferStatusTypeConstants
                    .GetTransferStatusTypes(query.TransferStatusType)
                    .ToList(); // List<string> implements IReadOnlyCollection<string>

                // 1) Load OU minimal graph (typed)
                var nodes = await LoadOrganisationUnitNodes(ct);
                var nodesById = nodes.ToDictionary(x => x.Id);

                // 2) Filter facilities (optionally by OrganisationUnitId)
                var facilities = GetFacilities(nodes, query.FacilityId);

                if (facilities.Count == 0)
                    return new List<FacilityOverviewReport>();

                // 3) Build tree lookup once
                var childrenByParent = BuildChildrenLookup(nodes);

                // 4) Build facility -> departments and department -> units maps
                var facilityToDepartments = facilities.ToDictionary(
                    f => f.Id,
                    f => GetDescendantsByLevel(f.Id, OrganisationUnitLevels.Department, nodesById, childrenByParent)
                );

                // Sessions/Observations exist only on Unit level => we only care about unit ids per department.
                var departmentToUnits = facilityToDepartments.Values
                    .SelectMany(deptIds => deptIds)
                    .Distinct()
                    .ToDictionary(
                        deptId => deptId,
                        deptId => GetDescendantsByLevel(deptId, OrganisationUnitLevels.Unit, nodesById, childrenByParent)
                    );

                var allUnitIds = departmentToUnits.Values.SelectMany(x => x).Distinct().ToList();
                if (allUnitIds.Count == 0)
                    return new List<FacilityOverviewReport>();

                // 5) Query counts once (sessions) and 1-4 times (observations)
                var sessionCountByUnit = await LoadSessionCountsByUnit(allUnitIds, query, allowedTransferCodes, ct);
                var observationCountByUnit = await LoadObservationCountsByUnit(allUnitIds, query, allowedTransferCodes, ct);

                // 6) Compose reports in a functional way
                var result = facilities
                    .Select(f => BuildFacilityReport(
                        facility: f,
                        nodes: nodes,
                        departmentIds: facilityToDepartments.GetValueOrDefault(f.Id) ?? new List<int>(),
                        departmentToUnits: departmentToUnits,
                        sessionCountByUnit: sessionCountByUnit,
                        observationCountByUnit: observationCountByUnit
                    ))
                    .Where(r => r.NumberOfObservations > 0)
                    .ToList();

                return result;
            }

            // -------------------------
            // Data loading helpers
            // -------------------------

            private Task<List<OrganisationUnitNode>> LoadOrganisationUnitNodes(CancellationToken ct)
            {
                return _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Select(x => new OrganisationUnitNode
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        Level = x.LevelRef.Level
                    })
                    .ToListAsync(ct);
            }

            private static List<OrganisationUnitNode> GetFacilities(List<OrganisationUnitNode> nodes, int? facilityId)
            {
                return nodes
                    .Where(n => n.Level == OrganisationUnitLevels.Facility)
                    .Where(n => facilityId == null || n.Id == facilityId.Value)
                    .OrderBy(n => n.Name)
                    .ToList();
            }

            private const int RootKey = 0;
            private static Dictionary<int, List<int>> BuildChildrenLookup(List<OrganisationUnitNode> nodes)
            {
                return nodes
                .GroupBy(n => n.ParentId ?? RootKey)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());
            }

            private async Task<Dictionary<int, int>> LoadSessionCountsByUnit(
                List<int> unitIds,
                Query query,
                IReadOnlyCollection<string> allowedTransferCodes,
                CancellationToken ct)
            {
                var discriminator = GetSessionDiscriminator(query.SessionType);

                var q = _context.Session
                    .AsNoTracking()
                    .Where(s => unitIds.Contains(s.OrganisationUnitId))
                    .Where(s => query.SessionType == null || s.Discriminator == discriminator)
                    .Where(s => query.FromDate == null || s.CreatedDate >= query.FromDate.Value)
                    .Where(s => query.ToDate == null || s.CreatedDate <= query.ToDate.Value)
                    .Where(s => allowedTransferCodes.Contains(s.TransferStatus.Code));

                return await q
                    .GroupBy(s => s.OrganisationUnitId)
                    .Select(g => new { UnitId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UnitId, x => x.Count, ct);
            }

            private async Task<Dictionary<int, int>> LoadObservationCountsByUnit(
                List<int> unitIds,
                Query query,
                IReadOnlyCollection<string> allowedTransferCodes,
                CancellationToken ct)
            {
                // Aggregate counts into one dictionary
                var result = new Dictionary<int, int>();

                void Merge(Dictionary<int, int> add)
                {
                    foreach (var kv in add)
                        result[kv.Key] = result.TryGetValue(kv.Key, out var existing) ? existing + kv.Value : kv.Value;
                }

                // if SessionType specified => run only that. else run all and sum.
                if (query.SessionType == null || query.SessionType == SessionType.FiveIndications)
                    Merge(await CountFiveIndications(unitIds, query, allowedTransferCodes, ct));

                if (query.SessionType == null || query.SessionType == SessionType.HandJewelry)
                    Merge(await CountHandJewelry(unitIds, query, allowedTransferCodes, ct));

                if (query.SessionType == null || query.SessionType == SessionType.Gloves)
                    Merge(await CountGloves(unitIds, query, allowedTransferCodes, ct));

                if (query.SessionType == null || query.SessionType == SessionType.ProtectiveEquipment)
                    Merge(await CountProtectiveEquipment(unitIds, query, allowedTransferCodes, ct));

                return result;
            }

            // -------------------------
            // Observation count queries (grouped by UnitId)
            // -------------------------

            private Task<Dictionary<int, int>> CountFiveIndications(
                List<int> unitIds, Query query, IReadOnlyCollection<string> allowedTransferCodes, CancellationToken ct)
            {
                return _context.FiveIndicationsObservation
                    .AsNoTracking()
                    .Where(o => unitIds.Contains(o.FiveIndicationsSession.OrganisationUnitId))
                    .Where(o => allowedTransferCodes.Contains(o.FiveIndicationsSession.TransferStatus.Code))
                    .Where(o => query.FromDate == null || o.RegisteredTime.Date >= query.FromDate.Value.Date)
                    .Where(o => query.ToDate == null || o.RegisteredTime.Date <= query.ToDate.Value.Date)
                    .GroupBy(o => o.FiveIndicationsSession.OrganisationUnitId)
                    .Select(g => new { UnitId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UnitId, x => x.Count, ct);
            }

            private Task<Dictionary<int, int>> CountHandJewelry(
                List<int> unitIds, Query query, IReadOnlyCollection<string> allowedTransferCodes, CancellationToken ct)
            {
                return _context.HandJewelryObservation
                    .AsNoTracking()
                    .Where(o => unitIds.Contains(o.HandJewelrySession.OrganisationUnitId))
                    .Where(o => allowedTransferCodes.Contains(o.HandJewelrySession.TransferStatus.Code))
                    .Where(o => query.FromDate == null || o.RegisteredTime.Date >= query.FromDate.Value.Date)
                    .Where(o => query.ToDate == null || o.RegisteredTime.Date <= query.ToDate.Value.Date)
                    .GroupBy(o => o.HandJewelrySession.OrganisationUnitId)
                    .Select(g => new { UnitId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UnitId, x => x.Count, ct);
            }

            private Task<Dictionary<int, int>> CountGloves(
                List<int> unitIds, Query query, IReadOnlyCollection<string> allowedTransferCodes, CancellationToken ct)
            {
                return _context.GloveObservation
                    .AsNoTracking()
                    .Where(o => unitIds.Contains(o.GloveSession.OrganisationUnitId))
                    .Where(o => unitIds.Contains(o.GloveSession.OrganisationUnitId))
                    .Where(o => allowedTransferCodes.Contains(o.GloveSession.TransferStatus.Code))
                    .Where(o => query.FromDate == null || o.RegisteredTime.Date >= query.FromDate.Value.Date)
                    .Where(o => query.ToDate == null || o.RegisteredTime.Date <= query.ToDate.Value.Date)
                    .GroupBy(o => o.GloveSession.OrganisationUnitId)
                    .Select(g => new { UnitId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UnitId, x => x.Count, ct);
            }

            private Task<Dictionary<int, int>> CountProtectiveEquipment(
                List<int> unitIds, Query query, IReadOnlyCollection<string> allowedTransferCodes, CancellationToken ct)
            {
                return _context.ProtectiveEquipmentObservation
                    .AsNoTracking()
                    .Where(o => unitIds.Contains(o.ProtectiveEquipmentSession.OrganisationUnitId))
                    .Where(o => allowedTransferCodes.Contains(o.ProtectiveEquipmentSession.TransferStatus.Code))
                    .Where(o => query.FromDate == null || o.RegisteredTime.Date >= query.FromDate.Value.Date)
                    .Where(o => query.ToDate == null || o.RegisteredTime.Date <= query.ToDate.Value.Date)
                    .GroupBy(o => o.ProtectiveEquipmentSession.OrganisationUnitId)
                    .Select(g => new { UnitId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UnitId, x => x.Count, ct);
            }

            // -------------------------
            // Report composition helpers
            // -------------------------

            private static FacilityOverviewReport BuildFacilityReport(
                OrganisationUnitNode facility,
                List<OrganisationUnitNode> nodes,
                List<int> departmentIds,
                Dictionary<int, List<int>> departmentToUnits,
                IReadOnlyDictionary<int, int> sessionCountByUnit,
                IReadOnlyDictionary<int, int> observationCountByUnit)
            {
                var nodesById = nodes.ToDictionary(x => x.Id);

                var units = departmentIds
                    .SelectMany(deptId =>
                    {
                        nodesById.TryGetValue(deptId, out var deptNode);
                        var unitIds = departmentToUnits.TryGetValue(deptId, out var u) ? u : new List<int>();

                        return unitIds.Select(unitId =>
                        {
                            nodesById.TryGetValue(unitId, out var unitNode);

                            var sessions = sessionCountByUnit.TryGetValue(unitId, out var sCount) ? sCount : 0;
                            var observations = observationCountByUnit.TryGetValue(unitId, out var oCount) ? oCount : 0;

                            return new UnitOverviewReport
                            {
                                Id = unitId,
                                Name = unitNode?.Name ?? $"Unit {unitId}",
                                NumberOfSessions = sessions,
                                NumberOfObservations = observations,
                                DepartmentId = deptId,
                                DepartmentName = deptNode?.Name ?? $"Department {deptId}"
                            };
                        });
                    })
                    .ToList();

                var totalSessions = units.Sum(u => u.NumberOfSessions);
                var totalObs = units.Sum(u => u.NumberOfObservations);

                var sortedUnits = units
                    .Where(u => u.NumberOfSessions > 0)
                    .OrderBy(u => u.DepartmentName)
                    .ThenBy(u => u.Name)
                    .Concat(
                        units.Where(u => u.NumberOfSessions == 0)
                             .OrderBy(u => u.DepartmentName)
                             .ThenBy(u => u.Name))
                    .ToList();

                return new FacilityOverviewReport
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    NumberOfSessions = totalSessions,
                    NumberOfObservations = totalObs,
                    Units = sortedUnits
                };
            }

            // -------------------------
            // Tree traversal helper
            // -------------------------

            private static List<int> GetDescendantsByLevel(
                int rootId,
                string targetLevel,
                IReadOnlyDictionary<int, OrganisationUnitNode> nodesById,
                Dictionary<int, List<int>> childrenByParent)
            {
                var result = new List<int>();
                var queue = new Queue<int>();
                var visited = new HashSet<int>();

                queue.Enqueue(rootId);
                visited.Add(rootId);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    if (!childrenByParent.TryGetValue(current, out var children))
                        continue;

                    foreach (var childId in children)
                    {
                        if (!visited.Add(childId))
                            continue;

                        if (nodesById.TryGetValue(childId, out var child) && child.Level == targetLevel)
                            result.Add(childId);

                        queue.Enqueue(childId);
                    }
                }

                return result;
            }

            private static string GetSessionDiscriminator(SessionType? type) =>
                type switch
                {
                    SessionType.FiveIndications => nameof(Domain.Session.FiveIndicationsSession),
                    SessionType.HandJewelry => nameof(Domain.Session.HandJewelrySession),
                    SessionType.ProtectiveEquipment => nameof(Domain.Session.ProtectiveEquipmentSession),
                    SessionType.Gloves => nameof(Domain.Session.GloveSession),
                    _ => ""
                };
        }

    }
}
    

