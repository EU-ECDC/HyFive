using HyFive.DataAccess;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Observation;
using HyFive.Domene.Observation.ProtectiveEquipment;
using HyFive.Domene.Observation.Gloves;
using HyFive.Modeller.V1.Sesjon;
using Microsoft.EntityFrameworkCore;
using HyFive.Modeller.V1.Konstanter;

namespace HyFive.Tjenester.Rapport.Observasjoner
{
    public class RapportForSesjonTypeHarData
    {
        public class Query : IRequest<bool>
        {
            public int SesjonType { get; set; }
            public int InstitusjonId { get; set; }
            public int? AvdelingId { get; set; }
            public DateTime FraDato { get; set; }
            public DateTime TilDato { get; set; }
            public AuthorizedRole Rolle { get; set; }
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
                var harData = false;
                if (query.SesjonType == (int)SesjonType.FireIndikasjoner)
                {
                    var queryable = _context.FourIndicationsObservation
                        .Include(p => p.FourIndicationsSession).ThenInclude(p => p.Department).ThenInclude(p => p.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SesjonType == (int)SesjonType.Handsmykker)
                {
                    var queryable = _context.HandJewelryObservation
                        .Include(p => p.HandJewelrySession).ThenInclude(p => p.Department).ThenInclude(a => a.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SesjonType == (int)SesjonType.Hansker)
                {
                    var queryable = _context.GloveObservation
                        .Include(p => p.GloveSession).ThenInclude(p => p.Department).ThenInclude(a => a.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SesjonType == (int)SesjonType.Beskyttelsesutstyr)
                {
                    var queryable = _context.ProtectiveEquipmentObservation
                        .Include(p => p.ProtectiveEquipmentSession).ThenInclude(p => p.Department).ThenInclude(a => a.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }

                return harData;
            }

            private static IQueryable<FourIndicationsObservation> LeggTilSøkeParametere(IQueryable<FourIndicationsObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.FourIndicationsSession.Department.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.FourIndicationsSession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.FourIndicationsSession.TransmissionStatus.Code == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }

            private static IQueryable<HandJewelryObservation> LeggTilSøkeParametere(IQueryable<HandJewelryObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.HandJewelrySession.Department.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.HandJewelrySession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.HandJewelrySession.TransmissionStatus.Code == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }

            private static IQueryable<GloveObservation> LeggTilSøkeParametere(IQueryable<GloveObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.GloveSession.Department.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.GloveSession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.GloveSession.TransmissionStatus.Code == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }

            private static IQueryable<ProtectiveEquipmentObservation> LeggTilSøkeParametere(IQueryable<ProtectiveEquipmentObservation> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.ProtectiveEquipmentSession.Department.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.ProtectiveEquipmentSession.Department.Id == avdelingId);

                queryable = queryable.Where(p => p.RegistrationTime.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.RegistrationTime.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.ProtectiveEquipmentSession.TransmissionStatus.Code == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }
        }
    }
}
