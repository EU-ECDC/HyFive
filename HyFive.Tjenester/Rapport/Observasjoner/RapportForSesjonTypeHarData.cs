using HyFive.Dataaksess;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;
using HyFive.Domene.Observasjon.Hansker;
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
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Query query, CancellationToken cancellationToken)
            {
                var harData = false;
                if (query.SesjonType == (int)SesjonType.FireIndikasjoner)
                {
                    var queryable = _context.FireIndikasjonerObservasjon
                        .Include(p => p.FireIndikasjonerSesjon).ThenInclude(p => p.Avdeling).ThenInclude(p => p.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SesjonType == (int)SesjonType.Handsmykker)
                {
                    var queryable = _context.HandsmykkeObservasjon
                        .Include(p => p.HandsmykkeSesjon).ThenInclude(p => p.Avdeling).ThenInclude(a => a.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SesjonType == (int)SesjonType.Hansker)
                {
                    var queryable = _context.HanskeObservasjon
                        .Include(p => p.HanskeSesjon).ThenInclude(p => p.Avdeling).ThenInclude(a => a.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }
                else if (query.SesjonType == (int)SesjonType.Beskyttelsesutstyr)
                {
                    var queryable = _context.BeskyttelsesutstyrObservasjon
                        .Include(p => p.BeskyttelsesutstyrSesjon).ThenInclude(p => p.Avdeling).ThenInclude(a => a.Institusjon)
                        .AsNoTracking();

                    queryable = LeggTilSøkeParametere(queryable, query.InstitusjonId, query.AvdelingId, query.FraDato, query.TilDato, query.Rolle);

                    harData = await queryable.AnyAsync(cancellationToken);
                }

                return harData;
            }

            private static IQueryable<FireIndikasjonerObservasjon> LeggTilSøkeParametere(IQueryable<FireIndikasjonerObservasjon> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.FireIndikasjonerSesjon.Avdeling.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.FireIndikasjonerSesjon.Avdeling.Id == avdelingId);

                queryable = queryable.Where(p => p.Registrerttidspunkt.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.Registrerttidspunkt.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.FireIndikasjonerSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }

            private static IQueryable<HandsmykkeObservasjon> LeggTilSøkeParametere(IQueryable<HandsmykkeObservasjon> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.HandsmykkeSesjon.Avdeling.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.HandsmykkeSesjon.Avdeling.Id == avdelingId);

                queryable = queryable.Where(p => p.Registrerttidspunkt.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.Registrerttidspunkt.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.HandsmykkeSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }

            private static IQueryable<HanskeObservasjon> LeggTilSøkeParametere(IQueryable<HanskeObservasjon> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.HanskeSesjon.Avdeling.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.HanskeSesjon.Avdeling.Id == avdelingId);

                queryable = queryable.Where(p => p.Registrerttidspunkt.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.Registrerttidspunkt.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.HanskeSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }

            private static IQueryable<BeskyttelsesutstyrObservasjon> LeggTilSøkeParametere(IQueryable<BeskyttelsesutstyrObservasjon> queryable, int institusjonId, int? avdelingId, 
                DateTime fraDato, DateTime tilDato, AuthorizedRole rolle)
            {

                queryable = queryable.Where(p => p.BeskyttelsesutstyrSesjon.Avdeling.InstitusjonId == institusjonId);
                if (avdelingId != null)
                    queryable = queryable.Where(p => p.BeskyttelsesutstyrSesjon.Avdeling.Id == avdelingId);

                queryable = queryable.Where(p => p.Registrerttidspunkt.Date >= fraDato.Date);
                queryable = queryable.Where(p => p.Registrerttidspunkt.Date <= tilDato.Date);

                if (rolle == AuthorizedRole.Administrator)
                    queryable = queryable.Where(p => p.BeskyttelsesutstyrSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);

                return queryable;
            }
        }
    }
}
