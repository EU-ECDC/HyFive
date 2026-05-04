using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Report.HandJewelry;
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
    public class GetHandJewelryObservations
    {
        public class Query : IRequest<IEnumerable<HandJewelryObservationReport>>
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

        public class Handler : IRequestHandler<Query, IEnumerable<HandJewelryObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HandJewelryObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                // 1) Resolve target UnitIds from FacilityIds/DepartmentIds (because sessions live on Unit)
                var unitIds = await ResolveUnitIds(query, cancellationToken);

                // If caller provided facility/department filters but they resolve to nothing, return empty fast
                var hadOrgFilters =
                    (query.FacilityIds?.Any() == true) || (query.FacilityId > 0) ||
                    (query.DepartmentIds?.Any() == true) || (query.DepartmentId > 0);

                if (hadOrgFilters && unitIds.Count == 0)
                    return Array.Empty<HandJewelryObservationReport>();

                // 2) Build observation query
                var queryable = _context.HandJewelryObservation
                    .AsNoTracking()
                    .Include(o => o.HandJewelrySession).ThenInclude(s => s.Observer)
                    .Include(o => o.HandJewelrySession).ThenInclude(s => s.TransferStatus)
                    .Include(o => o.HandJewelrySession).ThenInclude(s => s.OrganisationUnit)
                    .Include(o => o.HandJewelries)
                    .Include(o => o.Role)
                    .AsQueryable();

                if (query.Role == AuthorizedRole.Observer)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToCoordinator);
                }
                else if (query.Role == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }

                // Facility/Department filter (now via unitIds)
                if (unitIds.Count > 0)
                {
                    queryable = queryable.Where(o => unitIds.Contains(o.HandJewelrySession.OrganisationUnitId));
                }

                // Observer filter
                if (query.ObserverId > 0)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.ObserverId == query.ObserverId);
                }

                // Session filter
                if (query.SessionId.HasValue)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.Id == query.SessionId.Value);
                }

                if (query.FromDate != null)
                {
                    var fromDateUtc = DateTime.SpecifyKind(query.FromDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.RegisteredTime.Date >= fromDateUtc);
                }

                if (query.ToDate != null)
                {
                    var toDateUtc = DateTime.SpecifyKind(query.ToDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.RegisteredTime.Date <= toDateUtc);
                }
                
                return await queryable
                            .OrderBy(o => o.HandJewelrySession.Id)
                            .ThenBy(o => o.Id)
                            .ProjectTo<HandJewelryObservationReport>(_mapper.ConfigurationProvider)
                            .ToListAsync();
            }
            private async Task<HashSet<int>> ResolveUnitIds(Query query, CancellationToken ct)
            {
                // - DepartmentIds (if any) else UnitId
                // - FacilityIds (if any) else OrganisationUnitId

                var roots = new HashSet<int>();

                if (query.DepartmentIds?.Any() == true)
                    foreach (var id in query.DepartmentIds) roots.Add(id);
                else if (query.DepartmentId > 0)
                    roots.Add(query.DepartmentId.Value);

                if (query.FacilityIds?.Any() == true)
                    foreach (var id in query.FacilityIds) roots.Add(id);
                else if (query.FacilityId > 0)
                    roots.Add(query.FacilityId.Value);

                if (roots.Count == 0)
                    return new HashSet<int>(); // no org filter at all

                // We need to walk Unit tree and return descendants where LevelRef.Level == "Unit"
                // We'll load a slim list once and do an in-memory traversal (fast enough unless you have huge trees).
                var ous = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Select(x => new OrganisationUnitSlim
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Level = x.LevelRef.Level
                    })
                    .ToListAsync(ct);

                var childrenByParent = ous
                    .Where(x => x.ParentId.HasValue)
                    .GroupBy(x => x.ParentId!.Value)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

                var unitIds = new HashSet<int>();

                foreach (var rootId in roots)
                {
                    CollectDescendantsByLevel(
                        rootId,
                        targetLevel: OrganisationUnitLevels.Unit,
                        ousById: ous.ToDictionary(x => x.Id),
                        childrenByParent: childrenByParent,
                        result: unitIds);
                }

                return unitIds;
            }

            private static void CollectDescendantsByLevel(
                int rootId,
                string targetLevel,
                Dictionary<int, OrganisationUnitSlim> ousById,
                Dictionary<int, List<int>> childrenByParent,
                HashSet<int> result)
            {
                if (!ousById.ContainsKey(rootId))
                    return;

                var stack = new Stack<int>();
                stack.Push(rootId);

                var safety = 0;

                while (stack.Count > 0 && safety++ < 200_000)
                {
                    var currentId = stack.Pop();

                    if (ousById.TryGetValue(currentId, out var ou) &&
                        string.Equals(ou.Level, targetLevel, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(currentId);
                    }

                    if (childrenByParent.TryGetValue(currentId, out var children))
                    {
                        foreach (var childId in children)
                            stack.Push(childId);
                    }
                }
            }

            private sealed class OrganisationUnitSlim
            {
                public int Id { get; set; }
                public int? ParentId { get; set; }
                public string Level { get; set; }
            }
        }
    }
}
