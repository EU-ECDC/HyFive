using HyFive.DataAccess;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Report.Observations
{
    public class ReportForSessionTypeHasData
    {
        public class Query : IRequest<bool>
        {
            public int SessionType { get; set; }
            public List<int> InstitutionIds { get; set; }
            public List<int>? DepartmentIds { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Query query, CancellationToken cancellationToken)
            {
                var hasData = false;

                var fromDateUtc = DateTime.SpecifyKind(query.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(query.ToDate.Date, DateTimeKind.Utc);

                if (query.SessionType == (int)SessionType.FiveIndications)
                {
                    var queryable = _context.FiveIndicationsObservation
                        .Include(p => p.FiveIndicationsSession).ThenInclude(p => p.Department).ThenInclude(p => p.Institution)
                        .AsNoTracking();

                    queryable = AddSearchParameters(queryable, query.InstitutionIds, query.DepartmentIds, fromDateUtc, toDateUtc, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SessionType == (int)SessionType.HandJewelry)
                {
                    var queryable = _context.HandJewelryObservation
                        .Include(p => p.HandJewelrySession).ThenInclude(p => p.Department).ThenInclude(a => a.Institution)
                        .AsNoTracking();

                    queryable = AddSearchParameters(queryable, query.InstitutionIds, query.DepartmentIds, fromDateUtc, toDateUtc, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SessionType == (int)SessionType.Gloves)
                {
                    var queryable = _context.GloveObservation
                        .Include(p => p.GloveSession).ThenInclude(p => p.Department).ThenInclude(a => a.Institution)
                        .AsNoTracking();

                    queryable = AddSearchParameters(queryable, query.InstitutionIds, query.DepartmentIds, fromDateUtc, toDateUtc, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SessionType == (int)SessionType.ProtectiveEquipment)
                {
                    var queryable = _context.ProtectiveEquipmentObservation
                        .Include(p => p.ProtectiveEquipmentSession).ThenInclude(p => p.Department).ThenInclude(a => a.Institution)
                        .AsNoTracking();

                    queryable = AddSearchParameters(queryable, query.InstitutionIds, query.DepartmentIds, fromDateUtc, toDateUtc, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }

                return hasData;
            }

            private static IQueryable<FiveIndicationsObservation> AddSearchParameters(IQueryable<FiveIndicationsObservation> queryable, List<int> institutionIds, List<int>? departmentIds, 
                DateTime fromDate, DateTime toDate, AuthorizedRole role)
            {

                queryable = queryable.Where(p => institutionIds.Contains(p.FiveIndicationsSession.Department.InstitutionId));
                
                if (departmentIds != null && departmentIds.Any())
                    queryable = queryable.Where(p => departmentIds.Contains(p.FiveIndicationsSession.Department.Id));

                queryable = queryable.Where(p => p.RegisteredTime >= fromDate);
                queryable = queryable.Where(p => p.RegisteredTime <= toDate);

                if (role == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.FiveIndicationsSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return queryable;
            }

            private static IQueryable<HandJewelryObservation> AddSearchParameters(IQueryable<HandJewelryObservation> queryable, List<int> institutionIds, List<int>? departmentIds, 
                DateTime fromDate, DateTime toDate, AuthorizedRole role)
            {

                queryable = queryable.Where(p => institutionIds.Contains(p.HandJewelrySession.Department.InstitutionId));
                
                if (departmentIds != null && departmentIds.Any())
                    queryable = queryable.Where(p => departmentIds.Contains(p.HandJewelrySession.Department.Id));

                queryable = queryable.Where(p => p.RegisteredTime.Date >= fromDate);
                queryable = queryable.Where(p => p.RegisteredTime.Date <= toDate);

                if (role == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.HandJewelrySession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return queryable;
            }

            private static IQueryable<GloveObservation> AddSearchParameters(IQueryable<GloveObservation> queryable, List<int> institutionIds, List<int>? departmentIds, 
                DateTime fromDate, DateTime toDate, AuthorizedRole role)
            {

                queryable = queryable.Where(p => institutionIds.Contains(p.GloveSession.Department.InstitutionId));

                if (departmentIds != null && departmentIds.Any())
                    queryable = queryable.Where(p => departmentIds.Contains(p.GloveSession.Department.Id));

                queryable = queryable.Where(p => p.RegisteredTime.Date >= fromDate);
                queryable = queryable.Where(p => p.RegisteredTime.Date <= toDate);

                if (role == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.GloveSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return queryable;
            }

            private static IQueryable<ProtectiveEquipmentObservation> AddSearchParameters(IQueryable<ProtectiveEquipmentObservation> queryable, List<int> institutionIds, List<int>? departmentIds, 
                DateTime fromDate, DateTime toDate, AuthorizedRole role)
            {

                queryable = queryable.Where(p => institutionIds.Contains(p.ProtectiveEquipmentSession.Department.InstitutionId));

                if (departmentIds != null && departmentIds.Any())
                    queryable = queryable.Where(p => institutionIds.Contains(p.ProtectiveEquipmentSession.Department.Id));

                queryable = queryable.Where(p => p.RegisteredTime.Date >= fromDate);
                queryable = queryable.Where(p => p.RegisteredTime.Date <= toDate);

                if (role == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.ProtectiveEquipmentSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);

                return queryable;
            }
        }
    }
}
