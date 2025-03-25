using System.Linq;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Oversikt;
using System.Collections.Generic;
using HyFive.Modeller.V1.Sesjon;
using System;
using HyFive.Modeller.V1.Konstanter;

namespace HyFive.Tjenester.Institusjon
{
    public class HentInstitusjonerMedSesjoner
    {
        public class Query : IRequest<List<InstitusjonOversiktRapport>>
        {
            public SesjonType? Sesjontype { get; set; }
            public DateTime? FraDato { get; set; }
            public DateTime? TilDato { get; set; }
            public int? InstitusjonId { get; set; }
            public string OverforingsstatusType { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<InstitusjonOversiktRapport>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<InstitusjonOversiktRapport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var sesjonType = HentSesjonType(query.Sesjontype);
                var institusjoner = await _context.Institusjon
                    .AsNoTracking()
                    .Include(i => i.Avdelinger)
                    .ThenInclude(a => a.Sesjoner
                        .Where(s => query.Sesjontype == null || s.Discriminator == sesjonType)
                        .Where(s => query.FraDato == null || s.Opprettettidspunkt.Date >= query.FraDato.Value.Date)
                        .Where(s => query.TilDato == null || s.Opprettettidspunkt.Date <= query.TilDato.Value.Date)
                        .Where(s => OverforingstatusTypeKonstanter.HentOverforingsstatusTyper(query.OverforingsstatusType).Contains(s.Overforingstatus.Kode))
                    )
                    .Where(x => query.InstitusjonId == null || query.InstitusjonId == x.Id)
                    .OrderBy(i => i.Navn)
                    .ToListAsync(cancellationToken);

                var rapporter  = _mapper.Map<List<Domene.Sted.Institusjon>, List<InstitusjonOversiktRapport>>(institusjoner);

                foreach (var rapport in rapporter)
                {
                    foreach (var avdeling in rapport.Avdelinger)
                    {
                        var antall = HentAntallObservasjoner(avdeling.Id, query);
                        avdeling.AntallObservasjoner = antall;
                        rapport.AntallObservasjoner += antall;
                    }

                    var sortertListeMedObservasjoner = rapport.Avdelinger.Where(a => a.AntallSesjoner > 0).OrderBy(a => a.Navn);
                    var sortertListeUtenObservasjoner = rapport.Avdelinger.Where(a => a.AntallSesjoner == 0).OrderBy(a => a.Navn);

                    rapport.Avdelinger = sortertListeMedObservasjoner.Union(sortertListeUtenObservasjoner).ToList(); 
                }

                return rapporter.Where(r=>r.AntallObservasjoner > 0).ToList();
            }

            private int HentAntallObservasjoner(int avdelingId, Query query)
            {
                switch (query.Sesjontype)
                {
                    case SesjonType.Beskyttelsesutstyr:
                        return HentAntallObservasjonerForBeskyttelsesutstyr(avdelingId, query);
                    case SesjonType.FireIndikasjoner:
                        return HentAntallObservasjonerForFireIndikasjoner(avdelingId, query);
                    case SesjonType.Hansker:
                        return HentAntallObservasjonerForHansker(avdelingId, query);
                    case SesjonType.Handsmykker:
                        return HentAntallObservasjonerForHandsmykker(avdelingId, query);
                    default:
                        return HentAggregertAntallObservasjoner(avdelingId, query);
                }
            }

            private int HentAggregertAntallObservasjoner(in int avdelingId, Query query)
            {
                return 
                    HentAntallObservasjonerForBeskyttelsesutstyr(avdelingId, query)
                    + HentAntallObservasjonerForFireIndikasjoner(avdelingId, query)
                    + HentAntallObservasjonerForHansker(avdelingId, query)
                    + HentAntallObservasjonerForHandsmykker(avdelingId, query);
            }

            private int HentAntallObservasjonerForHandsmykker(int avdelingId, Query query)
            {
                return _context.HandsmykkeObservasjon
                    .AsNoTracking()
                    .Include(b => b.HandsmykkeSesjon)
                    .Count(bo => bo.HandsmykkeSesjon.Avdeling.Id == avdelingId 
                                 && OverforingstatusTypeKonstanter.HentOverforingsstatusTyper(query.OverforingsstatusType).Contains(bo.HandsmykkeSesjon.Overforingstatus.Kode)
                                 && (query.FraDato == null || bo.Registrerttidspunkt.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.Registrerttidspunkt.Date <= query.TilDato.Value.Date));
            }

            private int HentAntallObservasjonerForHansker(int avdelingId, Query query)
            {
                return _context.HanskeObservasjon
                    .AsNoTracking()
                    .Include(b => b.HanskeSesjon)
                    .Count(bo => bo.HanskeSesjon.Avdeling.Id == avdelingId 
                                 && OverforingstatusTypeKonstanter.HentOverforingsstatusTyper(query.OverforingsstatusType).Contains(bo.HanskeSesjon.Overforingstatus.Kode)
                                 && (query.FraDato == null || bo.Registrerttidspunkt.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.Registrerttidspunkt.Date <= query.TilDato.Value.Date));
            }

            private int HentAntallObservasjonerForFireIndikasjoner(int avdelingId, Query query)
            {
                return _context.FireIndikasjonerObservasjon
                    .AsNoTracking()
                    .Include(b => b.FireIndikasjonerSesjon)
                    .Count(bo => bo.FireIndikasjonerSesjon.Avdeling.Id == avdelingId 
                                 && OverforingstatusTypeKonstanter.HentOverforingsstatusTyper(query.OverforingsstatusType).Contains(bo.FireIndikasjonerSesjon.Overforingstatus.Kode)
                                 && (query.FraDato == null || bo.Registrerttidspunkt.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.Registrerttidspunkt.Date <= query.TilDato.Value.Date));
            }

            private int HentAntallObservasjonerForBeskyttelsesutstyr(int avdelingId, Query query)
            {
                return _context.BeskyttelsesutstyrObservasjon
                    .AsNoTracking()
                    .Include(b => b.BeskyttelsesutstyrSesjon)
                    .Count(bo => bo.BeskyttelsesutstyrSesjon.Avdeling.Id == avdelingId 
                                 && OverforingstatusTypeKonstanter.HentOverforingsstatusTyper(query.OverforingsstatusType).Contains(bo.BeskyttelsesutstyrSesjon.Overforingstatus.Kode)
                                 && (query.FraDato == null || bo.Registrerttidspunkt.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.Registrerttidspunkt.Date <= query.TilDato.Value.Date));
            }

            private static string HentSesjonType(SesjonType? type)
            {
                switch (type)
                {
                    case SesjonType.FireIndikasjoner:
                        return nameof(Domene.Sesjon.FireIndikasjonerSesjon);
                    case SesjonType.Handsmykker:
                        return nameof(Domene.Sesjon.HandsmykkeSesjon);
                    case SesjonType.Beskyttelsesutstyr:
                        return nameof(Domene.Sesjon.BeskyttelsesutstyrSesjon);
                    case SesjonType.Hansker:
                        return nameof(Domene.Sesjon.HanskeSesjon);
                    default:
                        return "";
                }
            }
        }
    }
}
