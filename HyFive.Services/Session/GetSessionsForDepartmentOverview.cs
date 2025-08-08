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
using HyFive.Models.V1.Constants;

namespace HyFive.Services.Session
{
    public class GetSessionsForDepartmentOverview
    {
        public class Query : IRequest<List<SessionOverviewReport>>
        {
            public int DepartmentId { get; set; }
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
                var sessionOverviewReport = new List<SessionOverviewReport>();
                
                if (request.SessionType == null || request.SessionType.Value == SessionType.FiveIndications)
                {
                    var fiveIndicationsSessionsReport= await CreateFiveIndicationsSessionsReport(request, cancellationToken);
                    sessionOverviewReport.AddRange(fiveIndicationsSessionsReport); 
                }
                if (request.SessionType == null || request.SessionType.Value == SessionType.HandJewelry)
                {
                    var handJewelrySessionsReport = await CreateHandJewelrySessionsReport(request, cancellationToken);
                    sessionOverviewReport.AddRange(handJewelrySessionsReport);
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

                sessionOverviewReport = sessionOverviewReport.OrderByDescending(s => s.CreatedDate).ToList();
                sessionOverviewReport.ForEach(s =>
                {
                    s.Observations = s.Observations.OrderByDescending(o => o.RegisteredTime).ToList();
                });

                return sessionOverviewReport;
            }

            private async Task<List<SessionOverviewReport>> CreateFiveIndicationsSessionsReport(Query request, CancellationToken cancellationToken)
            {
                var fromDateUtc = request.FromDate?.Date.ToUniversalTime();
                var toDateUtc = request.ToDate?.Date.ToUniversalTime();

                var FiveIndicationsSessions = await _context.FiveIndicationsSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransferStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.Activity.ActivityType)
                                     .Where(s => s.Department.Id == request.DepartmentId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= fromDateUtc)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= toDateUtc)
                                     .Where(s => TransferStatusTypeConstants.GetTransferStatusTypes(request.TransferStatusType).Contains(s.TransferStatus.Code))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var fiveIndicationsSessionsReport = _mapper.Map<List<Domain.Session.FiveIndicationsSession>, List<SessionOverviewReport>>(FiveIndicationsSessions);

                return fiveIndicationsSessionsReport;
            }

            private async Task<List<SessionOverviewReport>> CreateHandJewelrySessionsReport(Query request, CancellationToken cancellationToken)
            {
                var fromDateUtc = request.FromDate?.Date.ToUniversalTime();
                var toDateUtc = request.ToDate?.Date.ToUniversalTime();

                var handJewelrySessions = await _context.HandJewelrySession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransferStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                                     .Where(s => s.Department.Id == request.DepartmentId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= fromDateUtc)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= toDateUtc)
                                     .Where(s => TransferStatusTypeConstants.GetTransferStatusTypes(request.TransferStatusType).Contains(s.TransferStatus.Code))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var handJewelrySessionsReport = _mapper.Map<List<Domain.Session.HandJewelrySession>, List<SessionOverviewReport>>(handJewelrySessions);

                return handJewelrySessionsReport;
            }

            private async Task<List<SessionOverviewReport>> CreateGloveSessionsReport(Query request, CancellationToken cancellationToken)
            {
                var fromDateUtc = request.FromDate?.Date.ToUniversalTime();
                var toDateUtc = request.ToDate?.Date.ToUniversalTime();

                var gloveSessions = await _context.GloveSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransferStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.GloveWithIndicationTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.GloveWithoutIndicationTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.PostGloveHandHygieneType)
                                     .Where(s => s.Department.Id == request.DepartmentId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= fromDateUtc)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= toDateUtc)
                                     .Where(s => TransferStatusTypeConstants.GetTransferStatusTypes(request.TransferStatusType).Contains(s.TransferStatus.Code))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var gloveSessionsReport = _mapper.Map<List<Domain.Session.GloveSession>, List<SessionOverviewReport>>(gloveSessions);

                return gloveSessionsReport;
            }

            private async Task<List<SessionOverviewReport>> CreateProtectiveEquipmentSessionsReport(Query request, CancellationToken cancellationToken)
            {
                var fromDateUtc = request.FromDate?.Date.ToUniversalTime();
                var toDateUtc = request.ToDate?.Date.ToUniversalTime();

                var protectiveEquipmentSessions = await _context.ProtectiveEquipmentSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransferStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                                     .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.EquipmentType)
                                     .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.MisuseTypes)
                                     .Where(s => s.Department.Id == request.DepartmentId)
                                     .Where(s => request.FromDate == null || s.CreatedDate.Date >= fromDateUtc)
                                     .Where(s => request.ToDate == null || s.CreatedDate.Date <= toDateUtc)
                                     .Where(s => TransferStatusTypeConstants.GetTransferStatusTypes(request.TransferStatusType).Contains(s.TransferStatus.Code))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var protectiveEquipmentSessionsReport = _mapper.Map<List<Domain.Session.ProtectiveEquipmentSession>, List<SessionOverviewReport>>(protectiveEquipmentSessions);

                return protectiveEquipmentSessionsReport;
            }
        }
    }
}