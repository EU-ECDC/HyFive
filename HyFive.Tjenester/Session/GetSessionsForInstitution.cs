using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Session
{
    public class GetSessionsForInstitution
    {
        public class Query : IRequest<List<SessionOverviewReport>>
        {
            public int InstitutionId { get; set; }
            public int? ObservatorId { get; set; }
            public SessionType? SessionType { get; set; }
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

            public async Task<List<SessionOverviewReport>> Handle(Query request, CancellationToken cancellationToken)
            {
                var sessionOverviewReport = new List<SessionOverviewReport>();
                
                if (request.SessionType == null || request.SessionType.Value == SessionType.FourIndications)
                {
                    var fourIndicationsSessionsReport= await CreateFourIndicationsSessionsReport(request, cancellationToken);
                    sessionOverviewReport.AddRange(fourIndicationsSessionsReport); 
                }
                if (request.SessionType == null || request.SessionType.Value == SessionType.HandJewelry)
                {
                    var handsmykkeSesjonerRapport = await CreateHandJewelrySessionsReport(request, cancellationToken);
                    sessionOverviewReport.AddRange(handsmykkeSesjonerRapport);
                }
                if (request.SessionType == null || request.SessionType.Value == SessionType.Gloves)
                {
                    var gloveSessionsReport = await CreateGloveSessionsReport(request, cancellationToken);
                    sessionOverviewReport.AddRange(gloveSessionsReport);
                }
                if (request.SessionType == null || request.SessionType.Value == SessionType.ProtectiveEquipment)
                {
                    var protectiveEquipmentSessionsReport = await CreateProtectiveEquipmentSessionsReport(request, cancellationToken);
                    sessionOverviewReport.AddRange(protectiveEquipmentSessionsReport);
                }

                sessionOverviewReport = sessionOverviewReport.OrderByDescending(s => s.CreatedTime).ToList();
                sessionOverviewReport.ForEach(s =>
                {
                    s.Observations = s.Observations.OrderByDescending(o => o.RegisteredTime).ToList();
                });

                return sessionOverviewReport;
            }

            private async Task<List<SessionOverviewReport>> CreateFourIndicationsSessionsReport(Query request, CancellationToken cancellationToken)
            {
                var fourIndicationsSessions = await _context.FourIndicationsSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.Activity.ActivityType)
                                     .Where(s => s.Department.InstitutionId == request.InstitutionId)
                                     .Where(s => request.ObservatorId == null || s.Observer.Id == request.ObservatorId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= request.FromDate.Value.Date)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= request.ToDate.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var fourIndicationsSessionsReport = _mapper.Map<List<Domain.Session.FourIndicationsSession>, List<SessionOverviewReport>>(fourIndicationsSessions);

                return fourIndicationsSessionsReport;
            }

            private async Task<List<SessionOverviewReport>> CreateHandJewelrySessionsReport(Query request, CancellationToken cancellationToken)
            {
                var handJewelrySessions = await _context.HandJewelrySession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.HandJewelry)
                                     .Where(s => s.Department.InstitutionId == request.InstitutionId)
                                     .Where(s => request.ObservatorId == null || s.Observer.Id == request.ObservatorId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= request.FromDate.Value.Date)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= request.ToDate.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var handJewelrySessionsReport = _mapper.Map<List<Domain.Session.HandJewelrySession>, List<SessionOverviewReport>>(handJewelrySessions);

                return handJewelrySessionsReport;
            }

            private async Task<List<SessionOverviewReport>> CreateGloveSessionsReport(Query request, CancellationToken cancellationToken)
            {
                var gloveSessions = await _context.GloveSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.IndicatedGloveTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.GeneralPurposeGloveTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.PostGloveHandHygieneType)
                                     .Where(s => s.Department.InstitutionId == request.InstitutionId)
                                     .Where(s => request.ObservatorId == null || s.Observer.Id == request.ObservatorId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= request.FromDate.Value.Date)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= request.ToDate.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var gloveSessionsReport = _mapper.Map<List<Domain.Session.GloveSession>, List<SessionOverviewReport>>(gloveSessions);

                return gloveSessionsReport;
            }

            private async Task<List<SessionOverviewReport>> CreateProtectiveEquipmentSessionsReport(Query request, CancellationToken cancellationToken)
            {
                var protectiveEquipmentSessions = await _context.ProtectiveEquipmentSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                                     .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.EquipmentType)
                                     .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.MisuseTypes)
                                     .Where(s => s.Department.InstitutionId == request.InstitutionId)
                                     .Where(s => request.ObservatorId == null || s.Observer.Id == request.ObservatorId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= request.FromDate.Value.Date)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= request.ToDate.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var protectiveEquipmentSessionsReport = _mapper.Map<List<Domain.Session.ProtectiveEquipmentSession>, List<SessionOverviewReport>>(protectiveEquipmentSessions);

                return protectiveEquipmentSessionsReport;
            }
        }
    }
}