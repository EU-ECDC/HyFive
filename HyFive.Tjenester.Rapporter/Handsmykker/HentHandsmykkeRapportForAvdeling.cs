using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Sesjon;
using HyFive.Modeller.V1.Konstanter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Rapporter.Handsmykker
{
    public class HentHandsmykkeRapportForAvdeling
    {
        public class Query : IRequest<HandsmykkerapportForSmykketypeOgRolle>
        {
            public int AvdelingId { get; set; }
            public int InstiusjonId { get; set; }
            public DateTime FraTidspunkt { get; set; }
            public DateTime TilTidspunkt { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, HandsmykkerapportForSmykketypeOgRolle>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }

            public async Task<HandsmykkerapportForSmykketypeOgRolle> Handle(Query request, CancellationToken cancellationToken)
            {
                var rapportForAvdeling = LagRapportForAvdeling(request);
                var rapportForInstitusjon = LagRapportForInstitusjon(request);

                var avdeling = _context.Avdeling.AsNoTracking().First(a => a.Id == request.AvdelingId);
                var institusjon = _context.Institusjon.AsNoTracking().First(a => a.Id == request.InstiusjonId);
                var rapport = new HandsmykkerapportForSmykketypeOgRolle
                {
                    Avdeling = avdeling.Navn,
                    Institusjon = institusjon.Navn,
                    FraTidspunkt = request.FraTidspunkt,
                    TilTidspunkt = request.TilTidspunkt,
                    RapportForAvdeling = rapportForAvdeling,
                    RapportForInstitusjon = rapportForInstitusjon
                };

                return rapport;
            }

            private RapportForEnhet LagRapportForAvdeling(Query request)
            {
                var sesjoner = _context.Sesjon.OfType<HandsmykkeSesjon>()
                    .AsNoTracking()
                    .Include(p => p.Overforingstatus)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Rolle)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Handsmykker)
                    .Where(s =>
                        s.Avdeling.Id == request.AvdelingId
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date)
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date))
                    .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    sesjoner = sesjoner.Where(p => p.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi).ToList();
                }

                var rapportForEnhet = LagRapportForEnhet(sesjoner);

                return rapportForEnhet;
            }

            private RapportForEnhet LagRapportForInstitusjon(Query request)
            {
                var sesjoner = _context.Sesjon.OfType<HandsmykkeSesjon>()
                   .AsNoTracking()
                   .Include(p => p.Overforingstatus)
                   .Include(s => s.Observasjoner).ThenInclude(o => o.Rolle)
                   .Include(s => s.Observasjoner).ThenInclude(o => o.Handsmykker)
                   .Where(s =>
                       s.Avdeling.InstitusjonId == request.InstiusjonId
                       && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date)
                       && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date))
                   .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    sesjoner = sesjoner.Where(p => p.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi).ToList();
                }

                var rapportForEnhet = LagRapportForEnhet(sesjoner);

                return rapportForEnhet;
            }

            private RapportForEnhet LagRapportForEnhet(IEnumerable<HandsmykkeSesjon> sesjoner)
            {
                var observasjoner = sesjoner.SelectMany(p => p.Observasjoner).ToList();

                var smykketypeOgRolleListe = new List<SmykketypeOgRolle>();
                foreach (var observasjon in observasjoner)
                {
                    foreach (var smykketype in observasjon.Handsmykker)
                    {
                        smykketypeOgRolleListe.Add(new SmykketypeOgRolle
                        {
                            Rolle = observasjon.Rolle.Navn,
                            Smykketype = smykketype,
                        });
                    }
                }

                var smykketyper = _context.HandsmykkeType.AsNoTracking().ToList();

                var smykketypeOgAntallForRolleListe = new List<SmykketypeOgAntallForRolle>();
                foreach (var smykketypeNavn in smykketypeOgRolleListe.Select(p => p.Smykketype.Navn).Distinct())
                {
                    var antallForRolleListe = smykketypeOgRolleListe
                        .Where(p => p.Smykketype.Navn == smykketypeNavn)
                        .GroupBy(q => q.Rolle)
                        .Select(r => new AntallForRolle { Antall = r.Count(), Rolle = r.Key })
                        .ToList();

                    var smykketype = smykketyper.First(p => p.Navn == smykketypeNavn);
                    smykketypeOgAntallForRolleListe.Add(new SmykketypeOgAntallForRolle
                    {
                        Smykketype = smykketype,
                        AntallForRolleListe = antallForRolleListe
                    });
                }

                var observasjonerForRolleListe = observasjoner
                    .GroupBy(p => p.Rolle.Navn)
                    .Select(q => new ObservasjonerForRolle { Antall = q.Count(), Rolle = q.Key })
                    .ToList();

                var rapportForEnhet = new RapportForEnhet
                {
                    SmykketypeOgAntallForRolleListe = smykketypeOgAntallForRolleListe,
                    ObservasjonerForRolleListe = observasjonerForRolleListe
                };

                return rapportForEnhet;
            }

            private class SmykketypeOgRolle
            {
                public HandsmykkeType Smykketype { get; set; }
                public string Rolle { get; init; }
            }
        }
    }
}
