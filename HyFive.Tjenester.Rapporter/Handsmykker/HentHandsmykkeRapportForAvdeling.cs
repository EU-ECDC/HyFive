using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Observation;
using HyFive.Domene.Session;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Rapporter.Handsmykker
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
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<HandsmykkerapportForSmykketypeOgRolle> Handle(Query request, CancellationToken cancellationToken)
            {
                var rapportForAvdeling = LagRapportForAvdeling(request);
                var rapportForInstitusjon = LagRapportForInstitusjon(request);

                var avdeling = _context.Department.AsNoTracking().First(a => a.Id == request.AvdelingId);
                var institusjon = _context.Institution.AsNoTracking().First(a => a.Id == request.InstiusjonId);
                var rapport = new HandsmykkerapportForSmykketypeOgRolle
                {
                    Avdeling = avdeling.Navn,
                    Institusjon = institusjon.Name,
                    FraTidspunkt = request.FraTidspunkt,
                    TilTidspunkt = request.TilTidspunkt,
                    RapportForAvdeling = rapportForAvdeling,
                    RapportForInstitusjon = rapportForInstitusjon
                };

                return rapport;
            }

            private RapportForEnhet LagRapportForAvdeling(Query request)
            {
                var sesjoner = _context.Sesjon.OfType<HandJewelrySession>()
                    .AsNoTracking()
                    .Include(p => p.TransmissionStatus)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Role)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Handsmykker)
                    .Where(s =>
                        s.Department.Id == request.AvdelingId
                        && s.Observasjoner.Any(o => o.RegistrationTime.Date >= request.FraTidspunkt.Date)
                        && s.Observasjoner.Any(o => o.RegistrationTime.Date <= request.TilTidspunkt.Date))
                    .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    sesjoner = sesjoner.Where(p => p.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                var rapportForEnhet = LagRapportForEnhet(sesjoner);

                return rapportForEnhet;
            }

            private RapportForEnhet LagRapportForInstitusjon(Query request)
            {
                var sesjoner = _context.Sesjon.OfType<HandJewelrySession>()
                   .AsNoTracking()
                   .Include(p => p.TransmissionStatus)
                   .Include(s => s.Observasjoner).ThenInclude(o => o.Role)
                   .Include(s => s.Observasjoner).ThenInclude(o => o.Handsmykker)
                   .Where(s =>
                       s.Department.InstitusjonId == request.InstiusjonId
                       && s.Observasjoner.Any(o => o.RegistrationTime.Date >= request.FraTidspunkt.Date)
                       && s.Observasjoner.Any(o => o.RegistrationTime.Date <= request.TilTidspunkt.Date))
                   .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    sesjoner = sesjoner.Where(p => p.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                var rapportForEnhet = LagRapportForEnhet(sesjoner);

                return rapportForEnhet;
            }

            private RapportForEnhet LagRapportForEnhet(IEnumerable<HandJewelrySession> sesjoner)
            {
                var observasjoner = sesjoner.SelectMany(p => p.Observations).ToList();

                var smykketypeOgRolleListe = new List<SmykketypeOgRolle>();
                foreach (var observasjon in observasjoner)
                {
                    foreach (var smykketype in observasjon.HandJewelry)
                    {
                        smykketypeOgRolleListe.Add(new SmykketypeOgRolle
                        {
                            Rolle = observasjon.Role.Name,
                            Smykketype = smykketype,
                        });
                    }
                }

                var smykketyper = _context.HandJewelryType.AsNoTracking().ToList();

                var smykketypeOgAntallForRolleListe = new List<SmykketypeOgAntallForRolle>();
                foreach (var smykketypeNavn in smykketypeOgRolleListe.Select(p => p.Smykketype.Name).Distinct())
                {
                    var antallForRolleListe = smykketypeOgRolleListe
                        .Where(p => p.Smykketype.Name == smykketypeNavn)
                        .GroupBy(q => q.Rolle)
                        .Select(r => new AntallForRolle { Antall = r.Count(), Rolle = r.Key })
                        .ToList();

                    var smykketype = smykketyper.First(p => p.Name == smykketypeNavn);
                    smykketypeOgAntallForRolleListe.Add(new SmykketypeOgAntallForRolle
                    {
                        Smykketype = smykketype,
                        AntallForRolleListe = antallForRolleListe
                    });
                }

                var observasjonerForRolleListe = observasjoner
                    .GroupBy(p => p.Role.Name)
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
                public HandJewelryType Smykketype { get; set; }
                public string Rolle { get; init; }
            }
        }
    }
}
