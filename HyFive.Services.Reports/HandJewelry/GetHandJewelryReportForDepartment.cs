using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Observation;
using HyFive.Domain.Session;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Reports.HandJewelry
{
    public class GetHandJewelryReportForDepartment
    {
        public class Query : IRequest<JewelryReportForJewelryTypeAndRole>
        {
            public List<int> DepartmentIds { get; set; }
            public List<int> FacilityIds { get; set; }
            public DateTime FromDateTime { get; set; }
            public DateTime ToDateTime { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, JewelryReportForJewelryTypeAndRole>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<JewelryReportForJewelryTypeAndRole> Handle(Query request, CancellationToken cancellationToken)
            {
                var fromUtc = DateTime.SpecifyKind(request.FromDateTime.Date, DateTimeKind.Utc);
                var toUtc = DateTime.SpecifyKind(request.ToDateTime.Date, DateTimeKind.Utc);

                // 1) Load OU graph once
                var nodes = await LoadOuNodesAsync(_context, cancellationToken);
                var childrenByParent = BuildChildrenLookup(nodes);

                // 2) Resolve all unit ids for dept + facility filters (observations live at unit level)
                var unitIds = ResolveUnitIds(
                    request.FacilityIds,
                    request.DepartmentIds,
                    nodes,
                    childrenByParent);

                // If nothing resolved, return empty report
                if (unitIds.Count == 0)
                {
                    return new JewelryReportForJewelryTypeAndRole
                    {
                        Department = "",
                        Facility = "",
                        FromDate = request.FromDateTime,
                        ToDate = request.ToDateTime,
                        ReportForDepartment = new ReportForUnit(),
                        ReportForFacility = new ReportForUnit()
                    };
                }

                // 3) Build 2 logical scopes:
                //    - "department report" -> only unitIds under the provided DepartmentIds
                //    - "facility report"   -> only unitIds under the provided FacilityIds
                var deptUnitIds = ResolveUnitIds(
                    facilityIds: Enumerable.Empty<int>(),
                    departmentIds: request.DepartmentIds ?? new List<int>(),
                    nodes,
                    childrenByParent);

                var facilityUnitIds = ResolveUnitIds(
                    facilityIds: request.FacilityIds ?? new List<int>(),
                    departmentIds: Enumerable.Empty<int>(),
                    nodes,
                    childrenByParent);


                var departmentReport = await CreateUnitReportAsync(deptUnitIds, fromUtc, toUtc, request.Role, cancellationToken);
                var facilityReport = await CreateUnitReportAsync(facilityUnitIds, fromUtc, toUtc, request.Role, cancellationToken);

                // 4) Resolve names from Unit table (instead of old Department/Facility tables)
                var deptNames = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(ou => request.DepartmentIds.Contains(ou.Id))
                    .Where(ou => ou.LevelRef.Level == OrganisationUnitLevels.Department)
                    .Select(ou => ou.Name)
                    .ToListAsync(cancellationToken);

                var facilityNames = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(ou => request.FacilityIds.Contains(ou.Id))
                    .Where(ou => ou.LevelRef.Level == OrganisationUnitLevels.Facility)
                    .Select(ou => ou.Name)
                    .ToListAsync(cancellationToken);

                var report = new JewelryReportForJewelryTypeAndRole
                {
                    Department = string.Join(", ", deptNames),
                    Facility = string.Join(", ", facilityNames),
                    FromDate = request.FromDateTime,
                    ToDate = request.ToDateTime,
                    ReportForDepartment = departmentReport,
                    ReportForFacility = facilityReport
                };

                return report;
            }

            private sealed record OuNode(int Id, int? ParentId, string Name, string Level);

            private static async Task<List<OuNode>> LoadOuNodesAsync(HandHygieneContext ctx, CancellationToken ct)
            {
                return await ctx.OrganisationUnit
                    .AsNoTracking()
                    .Select(ou => new OuNode(
                        ou.Id,
                        ou.ParentId,
                        ou.Name,
                        ou.LevelRef.Level // Level is string in OrganisationUnitLevel
                    ))
                    .ToListAsync(ct);
            }

            private static Dictionary<int?, List<int>> BuildChildrenLookup(IEnumerable<OuNode> nodes) =>
                nodes.GroupBy(n => n.ParentId)
                     .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

            private static List<int> GetDescendantsByLevel(
                int rootId,
                string targetLevel,
                IReadOnlyList<OuNode> nodes,
                IReadOnlyDictionary<int?, List<int>> childrenByParent)
            {
                // Precompute node-by-id for level lookup
                var byId = nodes.ToDictionary(n => n.Id);

                var result = new List<int>();
                var stack = new Stack<int>();
                stack.Push(rootId);

                var safety = 0;
                while (stack.Count > 0 && safety++ < 200_000)
                {
                    var current = stack.Pop();

                    if (byId.TryGetValue(current, out var node) &&
                        string.Equals(node.Level, targetLevel, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(current);
                        // NOTE: still walk deeper, because we may have more nested levels later
                    }

                    if (childrenByParent.TryGetValue(current, out var kids))
                    {
                        foreach (var k in kids)
                            stack.Push(k);
                    }
                }

                return result;
            }

            private static List<int> ResolveUnitIds(
                IEnumerable<int> facilityIds,
                IEnumerable<int> departmentIds,
                IReadOnlyList<OuNode> nodes,
                IReadOnlyDictionary<int?, List<int>> childrenByParent)
            {
                var unitIds = new HashSet<int>();

                // Facility -> (descendants Department) -> (descendants Unit)
                foreach (var facilityId in facilityIds ?? Enumerable.Empty<int>())
                {
                    var deptIds = GetDescendantsByLevel(
                        rootId: facilityId,
                        targetLevel: OrganisationUnitLevels.Department,
                        nodes: nodes,
                        childrenByParent: childrenByParent);

                    foreach (var deptId in deptIds)
                    {
                        var uIds = GetDescendantsByLevel(
                            rootId: deptId,
                            targetLevel: OrganisationUnitLevels.Unit,
                            nodes: nodes,
                            childrenByParent: childrenByParent);

                        foreach (var u in uIds) unitIds.Add(u);
                    }
                }

                // Department -> (descendants Unit)
                foreach (var deptId in departmentIds ?? Enumerable.Empty<int>())
                {
                    var uIds = GetDescendantsByLevel(
                        rootId: deptId,
                        targetLevel: OrganisationUnitLevels.Unit,
                        nodes: nodes,
                        childrenByParent: childrenByParent);

                    foreach (var u in uIds) unitIds.Add(u);
                }

                return unitIds.ToList();
            }

            private async Task<ReportForUnit> CreateUnitReportAsync(
            List<int> unitIds,
            DateTime fromUtc,
            DateTime toUtc,
            AuthorizedRole role,
            CancellationToken ct)
            {
                if (unitIds == null || unitIds.Count == 0)
                    return new ReportForUnit();

                var sessionsQ = _context.Session
                    .OfType<HandJewelrySession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                    .Where(s => unitIds.Contains(s.OrganisationUnitId))
                    .Where(s => s.Observations.Any(o => o.RegisteredTime.Date >= fromUtc.Date && o.RegisteredTime.Date <= toUtc.Date));

                if (role == AuthorizedRole.Administrator)
                {
                    sessionsQ = sessionsQ.Where(s => s.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }
                else if (role == AuthorizedRole.Observer)
                {
                    sessionsQ = sessionsQ.Where(s => s.TransferStatus.Code == TransferStatusTypeConstants.TransferredToCoordinator);
                }

                var sessions = await sessionsQ.ToListAsync(ct);
                return BuildReportFromSessions(sessions);
            }

            private ReportForUnit BuildReportFromSessions(IEnumerable<HandJewelrySession> sessions)
            {
                var observations = sessions.SelectMany(p => p.Observations).ToList();

                // Build (JewelryTypeName, RoleName) pairs
                var pairs = new List<(HandJewelryType Type, string RoleName)>();
                foreach (var obs in observations)
                {
                    var roleName = obs.Role?.Name ?? "";
                    foreach (var jt in obs.HandJewelries ?? new List<HandJewelryType>())
                        pairs.Add((jt, roleName));
                }

                // Group by jewelry type then role
                var jewelryTypeAndCountForRoleList =
                    pairs.GroupBy(p => p.Type.Name)
                         .Select(g => new RoleCountForJewelryType
                         {
                             JewelryType = g.First().Type,
                             CountByRoleList = g.GroupBy(x => x.RoleName)
                                               .Select(rr => new CountByRole { Role = rr.Key, Count = rr.Count() })
                                               .ToList()
                         })
                         .ToList();

                var observationsForRoleList =
                    observations.GroupBy(o => o.Role?.Name ?? "")
                                .Select(g => new ObservationsByRole { Role = g.Key, Count = g.Count() })
                                .ToList();

                return new ReportForUnit
                {
                    RoleJewelrySummaryList = jewelryTypeAndCountForRoleList,
                    ListOfObservationsByRole = observationsForRoleList
                };
            }
        }
    }
}
