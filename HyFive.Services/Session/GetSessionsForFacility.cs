using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Session
{
    public class GetSessionsForFacility
    {
        public class Query : IRequest<List<SessionOverviewReport>>
        {
            public int FacilityId { get; set; }
            public int? ObservatorId { get; set; }
            public SessionType? SessionType { get; set; }
            public string TransferStatus { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<SessionOverviewReport>>
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
                public string Level { get; init; } = "";
            }

            public async Task<List<SessionOverviewReport>> Handle(Query request, CancellationToken cancellationToken)
            {
                var fromDateUtc = request.FromDate?.Date.ToUniversalTime();
                var toDateUtc = request.ToDate?.Date.ToUniversalTime();

                // 1) Resolve all Unit ids under this facility
                var unitIds = await GetUnitIdsUnderFacility(request.FacilityId, cancellationToken);
                if (unitIds.Count == 0)
                    return new List<SessionOverviewReport>();

                // 3) Fetch per session type
                var results = new List<SessionOverviewReport>();

                if (request.SessionType == null || request.SessionType == SessionType.FiveIndications)
                    results.AddRange(await LoadFiveIndications(unitIds, request, fromDateUtc, toDateUtc, cancellationToken));

                if (request.SessionType == null || request.SessionType == SessionType.HandJewelry)
                    results.AddRange(await LoadHandJewelry(unitIds, request, fromDateUtc, toDateUtc, cancellationToken));

                if (request.SessionType == null || request.SessionType == SessionType.Gloves)
                    results.AddRange(await LoadGloves(unitIds, request, fromDateUtc, toDateUtc, cancellationToken));

                if (request.SessionType == null || request.SessionType == SessionType.ProtectiveEquipment)
                    results.AddRange(await LoadProtectiveEquipment(unitIds, request, fromDateUtc, toDateUtc, cancellationToken));

                return results
                .OrderByDescending(s => s.CreatedDate)
                .Select(SortObservationsInsideSession)
                .ToList();
            }

            private static SessionOverviewReport SortObservationsInsideSession(SessionOverviewReport s)
            {
                if (s.Observations != null)
                    s.Observations = s.Observations.OrderByDescending(o => o.RegisteredTime).ToList();
                return s;
            }

            private async Task<List<int>> GetUnitIdsUnderFacility(int facilityId, CancellationToken ct)
            {
                // Load minimal OU graph (Id, ParentId, Level)
                var nodes = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Select(x => new OrganisationUnitNode
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Level = x.LevelRef.Level
                    })
                    .ToListAsync(ct);

                var childrenByParent = nodes
                .Where(n => n.ParentId.HasValue)
                .GroupBy(n => n.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

                var nodesById = nodes.ToDictionary(x => x.Id);

                // BFS from facilityId; collect Unit-level nodes
                var result = new List<int>();
                var q = new Queue<int>();
                var visited = new HashSet<int>();

                q.Enqueue(facilityId);
                visited.Add(facilityId);

                while (q.Count > 0)
                {
                    var current = q.Dequeue();
                    if (!childrenByParent.TryGetValue(current, out var children)) continue;

                    foreach (var childId in children)
                    {
                        if (!visited.Add(childId)) continue;

                        if (nodesById.TryGetValue(childId, out var node) &&
                            node.Level == OrganisationUnitLevels.Unit)
                        {
                            result.Add(childId);
                        }

                        q.Enqueue(childId);
                    }
                }

                return result;
            }

            private static IQueryable<TSession> ApplyCommonFilters<TSession>(
            IQueryable<TSession> q,
            List<int> unitIds,
            Query request,
            DateTime? fromUtc,
            DateTime? toUtc)
            where TSession : Domain.Session.Session
            {
                q = q.Where(s => unitIds.Contains(s.OrganisationUnitId));

                if (request.ObservatorId.HasValue)
                    q = q.Where(s => s.ObserverId == request.ObservatorId.Value);

                if (!string.IsNullOrWhiteSpace(request.TransferStatus))
                    q = q.Where(s => s.TransferStatus.Code == request.TransferStatus);

                if (fromUtc.HasValue)
                    q = q.Where(s => s.CreatedDate >= fromUtc.Value);

                if (toUtc.HasValue)
                    q = q.Where(s => s.CreatedDate <= toUtc.Value);

                return q;
            }

            private async Task<List<SessionOverviewReport>> LoadFiveIndications(
            List<int> unitIds, Query request, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
            {
                var q = _context.FiveIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                           .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.Activity.ActivityType)
                    .AsQueryable();

                q = ApplyCommonFilters(q, unitIds, request, fromUtc, toUtc);

                var sessions = await q.ToListAsync(ct);
                return _mapper.Map<List<Domain.Session.FiveIndicationsSession>, List<SessionOverviewReport>>(sessions);
            }

            private async Task<List<SessionOverviewReport>> LoadHandJewelry(
                List<int> unitIds, Query request, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
            {
                var q = _context.HandJewelrySession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                           .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                    .AsQueryable();

                q = ApplyCommonFilters(q, unitIds, request, fromUtc, toUtc);

                var sessions = await q.ToListAsync(ct);
                return _mapper.Map<List<Domain.Session.HandJewelrySession>, List<SessionOverviewReport>>(sessions);
            }

            private async Task<List<SessionOverviewReport>> LoadGloves(
                List<int> unitIds, Query request, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
            {
                var q = _context.GloveSession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                           .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.GloveWithIndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.GloveWithoutIndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.PostGloveHandHygieneType)
                    .AsQueryable();

                q = ApplyCommonFilters(q, unitIds, request, fromUtc, toUtc);

                var sessions = await q.ToListAsync(ct);
                return _mapper.Map<List<Domain.Session.GloveSession>, List<SessionOverviewReport>>(sessions);
            }

            private async Task<List<SessionOverviewReport>> LoadProtectiveEquipment(
                List<int> unitIds, Query request, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
            {
                var q = _context.ProtectiveEquipmentSession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                           .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.EquipmentType)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.MisuseTypes)
                    .AsQueryable();

                q = ApplyCommonFilters(q, unitIds, request, fromUtc, toUtc);

                var sessions = await q.ToListAsync(ct);
                return _mapper.Map<List<Domain.Session.ProtectiveEquipmentSession>, List<SessionOverviewReport>>(sessions);
            }
        }
    }
}