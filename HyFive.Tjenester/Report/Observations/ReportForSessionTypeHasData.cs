using HyFive.DataAccess;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Models.V1.Session;
using Microsoft.EntityFrameworkCore;
using HyFive.Models.V1.Constants;

namespace HyFive.Services.Rapport.Observations
{
    public class ReportForSessionTypeHasData
    {
        public class Query : IRequest<bool>
        {
            public int SessionType { get; set; }
            public int InstitutionId { get; set; }
            public int? DepartmentId { get; set; }
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
                if (query.SessionType == (int)SessionType.FourIndications)
                {
                    var queryable = _context.FourIndicationsObservation
                        .Include(p => p.FourIndicationsSession).ThenInclude(p => p.Department).ThenInclude(p => p.Institution)
                        .AsNoTracking();

                    queryable = AddSearchParameters(queryable, query.InstitutionId, query.DepartmentId, query.FromDate, query.ToDate, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SessionType == (int)SessionType.HandJewelry)
                {
                    var queryable = _context.HandJewelryObservation
                        .Include(p => p.HandJewelrySession).ThenInclude(p => p.Department).ThenInclude(a => a.Institution)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitutionId, query.DepartmentId, query.FromDate, query.ToDate, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SessionType == (int)SessionType.Gloves)
                {
                    var queryable = _context.GloveObservation
                        .Include(p => p.GloveSession).ThenInclude(p => p.Department).ThenInclude(a => a.Institution)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitutionId, query.DepartmentId, query.FromDate, query.ToDate, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SessionType == (int)SessionType.ProtectiveEquipment)
                {
                    var queryable = _context.ProtectiveEquipmentObservation
                        .Include(p => p.ProtectiveEquipmentSession).ThenInclude(p => p.Department).ThenInclude(a => a.Institution)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitutionId, query.DepartmentId, query.FromDate, query.ToDate, query.Role);

                    hasData = await queryable.AnyAsync(cancellationToken);
                }

                return hasData;
            }

            private static IQueryable<FourIndicationsObservation> AddSearchParameters(IQueryable<FourIndicationsObservation> queryable, int institutionId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.FourIndicationsSession.Department.InstitutionId == institutionId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.FourIndicationsSession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.FourIndicationsSession.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi);

                return queryable;
            }

            private static IQueryable<HandJewelryObservation> LeggTilSøkeParametere(IQueryable<HandJewelryObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.HandJewelrySession.Department.InstitutionId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.HandJewelrySession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.HandJewelrySession.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi);

                return queryable;
            }

            private static IQueryable<GloveObservation> LeggTilSøkeParametere(IQueryable<GloveObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.GloveSession.Department.InstitutionId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.GloveSession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.GloveSession.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi);

                return queryable;
            }

            private static IQueryable<ProtectiveEquipmentObservation> LeggTilSøkeParametere(IQueryable<ProtectiveEquipmentObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.ProtectiveEquipmentSession.Department.InstitutionId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.ProtectiveEquipmentSession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.ProtectiveEquipmentSession.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi);

                return queryable;
            }
        }
    }
}
