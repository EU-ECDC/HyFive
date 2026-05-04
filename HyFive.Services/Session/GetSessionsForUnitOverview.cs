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
    public class GetSessionsForUnitOverview
    {
        public class Query : IRequest<List<SessionOverviewReport>>
        {
            public int UnitId { get; set; }
            public SessionType? SessionType { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string TransferStatusType { get; set; }
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

            public async Task<List<SessionOverviewReport>> Handle(Query request, CancellationToken cancellationToken)
            {
                var fromDateUtc = request.FromDate?.Date.ToUniversalTime();
                var toDateUtc = request.ToDate?.Date.ToUniversalTime();

                var allowedTransferCodes = TransferStatusTypeConstants
               .GetTransferStatusTypes(request.TransferStatusType)
               .ToList();

                var unitExists = await _context.OrganisationUnit
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == request.UnitId, cancellationToken);

                if (!unitExists)
                    return new List<SessionOverviewReport>();

                // 2) Run queries only for requested session type(s)
                var reports = new List<SessionOverviewReport>();

                if (request.SessionType == null || request.SessionType == SessionType.FiveIndications)
                    reports.AddRange(await LoadFiveIndications(request.UnitId, fromDateUtc, toDateUtc, allowedTransferCodes, cancellationToken));

                if (request.SessionType == null || request.SessionType == SessionType.HandJewelry)
                    reports.AddRange(await LoadHandJewelry(request.UnitId, fromDateUtc, toDateUtc, allowedTransferCodes, cancellationToken));

                if (request.SessionType == null || request.SessionType == SessionType.Gloves)
                    reports.AddRange(await LoadGloves(request.UnitId, fromDateUtc, toDateUtc, allowedTransferCodes, cancellationToken));

                if (request.SessionType == null || request.SessionType == SessionType.ProtectiveEquipment)
                    reports.AddRange(await LoadProtectiveEquipment(request.UnitId, fromDateUtc, toDateUtc, allowedTransferCodes, cancellationToken));

                // 3) Sort once at the end
                return reports
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

            private async Task<List<SessionOverviewReport>> LoadFiveIndications(
            int unitId,
            DateTime? fromUtc,
            DateTime? toUtc,
            IReadOnlyCollection<string> allowedTransferCodes,
            CancellationToken ct)
            {
                var sessions = await _context.FiveIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                            .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.Activity.ActivityType)
                    .Where(s => s.OrganisationUnitId == unitId)
                    .Where(s => fromUtc == null || s.CreatedDate >= fromUtc.Value)
                    .Where(s => toUtc == null || s.CreatedDate <= toUtc.Value)
                    .Where(s => allowedTransferCodes.Contains(s.TransferStatus.Code))
                    .ToListAsync(ct);

                return _mapper.Map<List<Domain.Session.FiveIndicationsSession>, List<SessionOverviewReport>>(sessions);
            }

            private async Task<List<SessionOverviewReport>> LoadHandJewelry(
                int unitId,
                DateTime? fromUtc,
                DateTime? toUtc,
                IReadOnlyCollection<string> allowedTransferCodes,
                CancellationToken ct)
            {
                var sessions = await _context.HandJewelrySession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                            .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                    .Where(s => s.OrganisationUnitId == unitId)
                    .Where(s => fromUtc == null || s.CreatedDate >= fromUtc.Value)
                    .Where(s => toUtc == null || s.CreatedDate <= toUtc.Value)
                    .Where(s => allowedTransferCodes.Contains(s.TransferStatus.Code))
                    .ToListAsync(ct);

                return _mapper.Map<List<Domain.Session.HandJewelrySession>, List<SessionOverviewReport>>(sessions);
            }

            private async Task<List<SessionOverviewReport>> LoadGloves(
                int unitId,
                DateTime? fromUtc,
                DateTime? toUtc,
                IReadOnlyCollection<string> allowedTransferCodes,
                CancellationToken ct)
            {
                var sessions = await _context.GloveSession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                            .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.GloveWithIndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.GloveWithoutIndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.PostGloveHandHygieneType)
                    .Where(s => s.OrganisationUnitId == unitId)
                    .Where(s => fromUtc == null || s.CreatedDate >= fromUtc.Value)
                    .Where(s => toUtc == null || s.CreatedDate <= toUtc.Value)
                    .Where(s => allowedTransferCodes.Contains(s.TransferStatus.Code))
                    .ToListAsync(ct);

                return _mapper.Map<List<Domain.Session.GloveSession>, List<SessionOverviewReport>>(sessions);
            }

            private async Task<List<SessionOverviewReport>> LoadProtectiveEquipment(
                int unitId,
                DateTime? fromUtc,
                DateTime? toUtc,
                IReadOnlyCollection<string> allowedTransferCodes,
                CancellationToken ct)
            {
                var sessions = await _context.ProtectiveEquipmentSession
                    .AsNoTracking()
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(ou => ou.Parent)
                            .ThenInclude(parent => parent.Parent)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.EquipmentType)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.MisuseTypes)
                    .Where(s => s.OrganisationUnitId == unitId)
                    .Where(s => fromUtc == null || s.CreatedDate >= fromUtc.Value)
                    .Where(s => toUtc == null || s.CreatedDate <= toUtc.Value)
                    .Where(s => allowedTransferCodes.Contains(s.TransferStatus.Code))
                    .ToListAsync(ct);

                return _mapper.Map<List<Domain.Session.ProtectiveEquipmentSession>, List<SessionOverviewReport>>(sessions);
            }
        }
    }
}