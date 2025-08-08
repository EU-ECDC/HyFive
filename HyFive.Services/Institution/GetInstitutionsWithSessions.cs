using System.Linq;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Overview;
using System.Collections.Generic;
using HyFive.Models.V1.Session;
using System;
using HyFive.Models.V1.Constants;

namespace HyFive.Services.Institution
{
    public class GetInstitutionsWithSessions
    {
        public class Query : IRequest<List<InstitutionOverviewReport>>
        {
            public SessionType? SessionType { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? InstitutionId { get; set; }
            public string TransferStatusType { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<InstitutionOverviewReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<InstitutionOverviewReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var sessionType = GetSessionType(query.SessionType);
                var institutions = await _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Sessions
                        .Where(s => query.SessionType == null || s.Discriminator == sessionType)
                        .Where(s => query.FromDate == null || s.CreatedDate >= query.FromDate)
                        .Where(s => query.ToDate == null || s.CreatedDate <= query.ToDate)
                        .Where(s => TransferStatusTypeConstants.GetTransferStatusTypes(query.TransferStatusType).Contains(s.TransferStatus.Code))
                    )
                    .Where(x => query.InstitutionId == null || query.InstitutionId == x.Id)
                    .OrderBy(i => i.Name)
                    .ToListAsync(cancellationToken);

                var reports  = _mapper.Map<List<Domain.Place.Institution>, List<InstitutionOverviewReport>>(institutions);

                foreach (var report in reports)
                {
                    foreach (var department in report.Departments)
                    {
                        var count = GetNumberOfObservations(department.Id, query);
                        department.NumberOfObservations = count;
                        report.NumberOfObservations += count;
                    }

                    var observationsSortedList = report.Departments.Where(a => a.NumberOfSessions > 0).OrderBy(a => a.Name);
                    var withoutObservationsSortedList = report.Departments.Where(a => a.NumberOfSessions == 0).OrderBy(a => a.Name);

                    report.Departments = observationsSortedList.Union(withoutObservationsSortedList).ToList(); 
                }

                return reports.Where(r=>r.NumberOfObservations > 0).ToList();
            }

            private int GetNumberOfObservations(int departmentId, Query query)
            {
                switch (query.SessionType)
                {
                    case SessionType.ProtectiveEquipment:
                        return GetNumberOfObservationsForProtectiveEquipment(departmentId, query);
                    case SessionType.FiveIndications:
                        return GetNumberOfObservationsForFiveIndications(departmentId, query);
                    case SessionType.Gloves:
                        return GetNumberOfObservationsForGloves(departmentId, query);
                    case SessionType.HandJewelry:
                        return GetNumberOfObservationsForHandJewelry(departmentId, query);
                    default:
                        return GetAggregatedNumberOfObservations(departmentId, query);
                }
            }

            private int GetAggregatedNumberOfObservations(in int departmentId, Query query)
            {
                return 
                    GetNumberOfObservationsForProtectiveEquipment(departmentId, query)
                    + GetNumberOfObservationsForFiveIndications(departmentId, query)
                    + GetNumberOfObservationsForGloves(departmentId, query)
                    + GetNumberOfObservationsForHandJewelry(departmentId, query);
            }

            private int GetNumberOfObservationsForHandJewelry(int departmentId, Query query)
            {
                return _context.HandJewelryObservation
                    .AsNoTracking()
                    .Include(b => b.HandJewelrySession)
                    .Count(bo => bo.HandJewelrySession.Department.Id == departmentId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.TransferStatusType).Contains(bo.HandJewelrySession.TransferStatus.Code)
                                 && (query.FromDate == null || bo.RegisteredTime.Date >= query.FromDate.Value.Date)
                                 && (query.ToDate == null || bo.RegisteredTime.Date <= query.ToDate.Value.Date));
            }

            private int GetNumberOfObservationsForGloves(int departmentId, Query query)
            {
                return _context.GloveObservation
                    .AsNoTracking()
                    .Include(b => b.GloveSession)
                    .Count(bo => bo.GloveSession.Department.Id == departmentId
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.TransferStatusType).Contains(bo.GloveSession.TransferStatus.Code)
                                 && (query.FromDate == null || bo.RegisteredTime.Date >= query.FromDate.Value.Date)
                                 && (query.ToDate == null || bo.RegisteredTime.Date <= query.ToDate.Value.Date));
            }

            private int GetNumberOfObservationsForFiveIndications(int departmentId, Query query)
            {
                return _context.FiveIndicationsObservation
                    .AsNoTracking()
                    .Include(b => b.FiveIndicationsSession)
                    .Count(bo => bo.FiveIndicationsSession.Department.Id == departmentId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.TransferStatusType).Contains(bo.FiveIndicationsSession.TransferStatus.Code)
                                 && (query.FromDate == null || bo.RegisteredTime.Date >= query.FromDate.Value.Date)
                                 && (query.ToDate == null || bo.RegisteredTime.Date <= query.ToDate.Value.Date));
            }

            private int GetNumberOfObservationsForProtectiveEquipment(int departmentId, Query query)
            {
                return _context.ProtectiveEquipmentObservation
                    .AsNoTracking()
                    .Include(b => b.ProtectiveEquipmentSession)
                    .Count(bo => bo.ProtectiveEquipmentSession.Department.Id == departmentId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.TransferStatusType).Contains(bo.ProtectiveEquipmentSession.TransferStatus.Code)
                                 && (query.FromDate == null || bo.RegisteredTime.Date >= query.FromDate.Value.Date)
                                 && (query.ToDate == null || bo.RegisteredTime.Date <= query.ToDate.Value.Date));
            }

            private static string GetSessionType(SessionType? type)
            {
                switch (type)
                {
                    case SessionType.FiveIndications:
                        return nameof(Domain.Session.FiveIndicationsSession);
                    case SessionType.HandJewelry:
                        return nameof(Domain.Session.HandJewelrySession);
                    case SessionType.ProtectiveEquipment:
                        return nameof(Domain.Session.ProtectiveEquipmentSession);
                    case SessionType.Gloves:
                        return nameof(Domain.Session.GloveSession);
                    default:
                        return "";
                }
            }
        }
    }
}
