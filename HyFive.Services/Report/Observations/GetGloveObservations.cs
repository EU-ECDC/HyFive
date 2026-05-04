using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
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
    public class GetGloveObservations
    {
        public class Query : IRequest<IEnumerable<GloveObservationReport>>
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

        public class Handler : IRequestHandler<Query, IEnumerable<GloveObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<GloveObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                // 1) Resolve allowed transfer statuses
                var transferCode = query.Role switch
                {
                    AuthorizedRole.Observer => TransferStatusTypeConstants.TransferredToCoordinator,
                    AuthorizedRole.Coordinator => TransferStatusTypeConstants.TransferredToCoordinator,
                    AuthorizedRole.Administrator => TransferStatusTypeConstants.TransferredToAdmin,
                    _ => TransferStatusTypeConstants.TransferredToCoordinator
                };

                // 2) Roots (facility/department ids are Unit ids now)
                var rootIds = new HashSet<int>();

                if (query.FacilityIds != null) foreach (var id in query.FacilityIds) rootIds.Add(id);
                if (query.DepartmentIds != null) foreach (var id in query.DepartmentIds) rootIds.Add(id);

                if (query.FacilityId.HasValue && query.FacilityId.Value > 0) rootIds.Add(query.FacilityId.Value);
                if (query.DepartmentId.HasValue && query.DepartmentId.Value > 0) rootIds.Add(query.DepartmentId.Value);

                // 3) Expand to Unit ids (sessions/observations only exist at Unit level)
                var unitIds = await ResolveUnitIds(rootIds.ToList(), cancellationToken);

                // 4) Query observations
                var queryable = _context.GloveObservation
                    .AsNoTracking()
                    .Include(o => o.GloveSession).ThenInclude(s => s.Observer)
                    .Include(o => o.GloveSession).ThenInclude(s => s.TransferStatus)
                    .Include(o => o.GloveSession).ThenInclude(s => s.OrganisationUnit)
                    .Include(o => o.PostGloveHandHygieneType)
                    .Include(o => o.GloveWithIndicationTypes)
                    .Include(o => o.GloveWithoutIndicationTypes)
                    .Include(o => o.Role)
                    .Where(o => o.GloveSession.TransferStatus.Code == transferCode);

                if (unitIds.Count > 0)
                    queryable = queryable.Where(o => unitIds.Contains(o.GloveSession.OrganisationUnitId));
                else if (rootIds.Count > 0)
                    // If caller sent roots but no units exist under them -> return empty
                    return Array.Empty<GloveObservationReport>();

                if (query.ObserverId > 0)
                    queryable = queryable.Where(o => o.GloveSession.ObserverId == query.ObserverId);

                if (query.SessionId.HasValue)
                    queryable = queryable.Where(o => o.GloveSession.Id == query.SessionId.Value);

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
                .OrderBy(o => o.GloveSession.Id)
                .ThenBy(o => o.Id)
                .ProjectTo<GloveObservationReport>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            }
            private async Task<List<int>> ResolveUnitIds(List<int> rootOrganisationUnitIds, CancellationToken ct)
            {
                if (rootOrganisationUnitIds == null || rootOrganisationUnitIds.Count == 0)
                    return new List<int>();

                // Minimal load for traversal
                var ous = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Select(x => new { x.Id, x.ParentId, Level = x.LevelRef.Level })
                    .ToListAsync(ct);

                var byId = ous.ToDictionary(x => x.Id, x => x);
                var childrenByParent = ous
                    .Where(x => x.ParentId.HasValue)
                    .GroupBy(x => x.ParentId!.Value)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

                var result = new HashSet<int>();
                var visited = new HashSet<int>();
                var queue = new Queue<int>(rootOrganisationUnitIds.Where(byId.ContainsKey));

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    if (!visited.Add(current)) continue;

                    if (string.Equals(byId[current].Level, OrganisationUnitLevels.Unit, StringComparison.OrdinalIgnoreCase))
                        result.Add(current);

                    if (childrenByParent.TryGetValue(current, out var kids))
                    {
                        foreach (var k in kids) queue.Enqueue(k);
                    }
                }

                return result.ToList();
            }
        }
    }
}
