using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Observation;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;
using FourIndicationsSession = HyFive.Domene.Session.FourIndicationsSession;

namespace HyFive.Services.Rapporter.FireIndikasjoner
{
    public class HentFireIndikasjonerRapportForAvdeling
    {
        public class Query : IRequest<FireIndikasjonerRapportForAvdeling>
        {
            public int AvdelingId { get; set; }
            public DateTime FraTidspunkt { get; set; }
            public DateTime TilTidspunkt { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, FireIndikasjonerRapportForAvdeling>
        {
            private readonly HandHygieneContext _context;


            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<FireIndikasjonerRapportForAvdeling> Handle(Query request, CancellationToken cancellationToken)
            {
                var rapportDto = new FireIndikasjonerRapportForAvdeling();

                rapportDto.Avdeling = HentAvdelingsdata(request);
                rapportDto.Klinikker = HentRapporterForKlinikker(request);
                rapportDto.Institusjon = HentInstitusjondata(request);
                rapportDto.SammenlignbareAvdelinger = await HentSammenlignbareAvdelingerData(request);
                rapportDto.SettVisningstidspunkter(request.FraTidspunkt, request.TilTidspunkt);
                return rapportDto;
            }

            private FireIndikasjonerRapport HentAvdelingsdata(Query request)
            {
                var avdeling = _context.Department.AsNoTracking().First(a => a.Id == request.AvdelingId);

                var avdelingssesjonerMedObservasjoner = _context.Sesjon.OfType<FourIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransmissionStatus)
                    .Include(s => s.Observasjoner)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Aktivitet)
                        .ThenInclude(a => a.AktivitetType)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Indikasjonstyper)
                    .Where(s =>
                        s.Department.Id == request.AvdelingId
                        && s.Observasjoner.Any(o => o.RegistrationTime.Date >= request.FraTidspunkt.Date)
                        && s.Observasjoner.Any(o => o.RegistrationTime.Date <= request.TilTidspunkt.Date))
                    .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    avdelingssesjonerMedObservasjoner = avdelingssesjonerMedObservasjoner.Where(p => p.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var sesjon in avdelingssesjonerMedObservasjoner)
                {
                    sesjon.Observasjoner = sesjon.Observasjoner.Where(o =>
                            o.RegistrationTime.Date >= request.FraTidspunkt.Date &&
                            o.RegistrationTime.Date <= request.TilTidspunkt.Date)
                        .ToList();
                }

                var antallObservasjoner = avdelingssesjonerMedObservasjoner.SelectMany(o => o.Observasjoner).Count();

                return new FireIndikasjonerRapport()
                {
                    Navn = avdeling.Navn,
                    FraTidspunkt = request.FraTidspunkt,
                    TilTidspunkt = request.TilTidspunkt,
                    Roller = HentRolleMedKombinasjonerRapportListe(avdelingssesjonerMedObservasjoner),
                    AntallObservasjoner = antallObservasjoner,
                    DebugObservasjonerStringList = avdelingssesjonerMedObservasjoner.SelectMany(o => o.Observasjoner)
                        .Select(o => DebugObservasjon(o))
                        .ToArray()
                };
            }

            private async Task<FireIndikasjonerRapport> HentSammenlignbareAvdelingerData(Query request)
            {
                var avdelingSomSkalSammenlignes = _context.Department.AsNoTracking().Include(a => a.Avdelingtype).First(a => a.Id == request.AvdelingId);

                var sammenlignbareAvdelingersSesjoner = await _context.Sesjon.OfType<FourIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransmissionStatus)
                    .Include(s => s.Department)
                        .ThenInclude(s => s.Avdelingtype)
                    .Include(s => s.Observasjoner)
                        .ThenInclude(o => o.Rolle)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Aktivitet)
                        .ThenInclude(a => a.AktivitetType)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Indikasjonstyper)
                    .Where(s =>
                        s.Avdeling.Id != request.AvdelingId
                        && s.Avdeling.Avdelingtype.Kode == avdelingSomSkalSammenlignes.Avdelingtype.Kode
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date)
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date)
                        && s.Observasjoner.Any())
                    .ToListAsync();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    sammenlignbareAvdelingersSesjoner = sammenlignbareAvdelingersSesjoner.Where(p => p.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var sesjon in sammenlignbareAvdelingersSesjoner)
                {
                    sesjon.Observasjoner = sesjon.Observasjoner.Where(o =>
                            o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date &&
                            o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date)
                        .ToList();
                }

                var antallObservasjoner = sammenlignbareAvdelingersSesjoner.SelectMany(o => o.Observasjoner).Count();

                return new FireIndikasjonerRapport()
                {
                    Navn = $"Sammenlignbare avdelinger for {avdelingSomSkalSammenlignes.Navn}",
                    FraTidspunkt = request.FraTidspunkt,
                    TilTidspunkt = request.TilTidspunkt,
                    Roller = HentRolleMedKombinasjonerRapportListe(sammenlignbareAvdelingersSesjoner),
                    AntallObservasjoner = antallObservasjoner,
                    DebugObservasjonerStringList = sammenlignbareAvdelingersSesjoner.SelectMany(o => o.Observasjoner)
                        .Select(o => DebugObservasjon(o))
                        .ToArray()
                };
            }

            private FireIndikasjonerRapport HentInstitusjondata(Query request)
            {
                var institusjonId = _context.Department
                    .AsNoTracking()
                    .Select(a => new { AvdelingId = a.Id, a.InstitusjonId })
                    .First(a => a.AvdelingId == request.AvdelingId).InstitusjonId;


                var institusjon = _context.Institution
                    .AsNoTracking()
                    .Include(i => i.InstitutionType)
                    .Select(i => new { i.Name, i.Id, Insitusjonstype = i.InstitutionType.Name })
                    .First(i => i.Id == institusjonId);

                var institusjonSesjonerMinusForespurtAvdeling = _context.Sesjon.OfType<FourIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransmissionStatus)
                    .Include(s => s.Department)
                        .ThenInclude(s => s.Avdelingtype)
                    .Include(s => s.Observasjoner)
                        .ThenInclude(o => o.Rolle)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Aktivitet)
                        .ThenInclude(a => a.AktivitetType)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Indikasjonstyper)
                    .Where(s =>
                        s.Avdeling.InstitusjonId == institusjonId
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date)
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date))
                    .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    institusjonSesjonerMinusForespurtAvdeling = institusjonSesjonerMinusForespurtAvdeling.Where(p => p.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var sesjon in institusjonSesjonerMinusForespurtAvdeling)
                {
                    sesjon.Observasjoner = sesjon.Observasjoner.Where(o =>
                            o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date &&
                            o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date)
                        .ToList();
                }

                var antallObservasjoner = institusjonSesjonerMinusForespurtAvdeling.SelectMany(o => o.Observasjoner).Count();

                return new FireIndikasjonerRapport()
                {
                    Navn = $"{institusjon.Insitusjonstype}: {institusjon.Navn} ",
                    FraTidspunkt = request.FraTidspunkt,
                    TilTidspunkt = request.TilTidspunkt,
                    Roller = HentRolleMedKombinasjonerRapportListe(institusjonSesjonerMinusForespurtAvdeling),
                    AntallObservasjoner = antallObservasjoner,
                    DebugObservasjonerStringList = institusjonSesjonerMinusForespurtAvdeling.SelectMany(o => o.Observasjoner)
                        .Select(o => DebugObservasjon(o))
                        .ToArray()
                };
            }

            private List<FireIndikasjonerRapport> HentRapporterForKlinikker(Query request)
            {
                var klinikkrappporter = new List<FireIndikasjonerRapport>();

                var avdelingsklinikkIder = _context.Department
                    .AsNoTracking()
                    .Include(a => a.Klinikker)
                    .First(a => a.Id == request.AvdelingId).Klinikker.Select(s => s.Id);

                foreach (var klinikkId in avdelingsklinikkIder)
                {
                    var rapport = HentRapportForKlinikk(klinikkId, request);
                    klinikkrappporter.Add(rapport);
                }

                return klinikkrappporter;
            }

            private FireIndikasjonerRapport HentRapportForKlinikk(int klinikkId, Query request)
            {
                var tilknyttetKlinikkSesjoner = _context.Sesjon.OfType<FourIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransmissionStatus)
                    .Include(s => s.Department)
                        .ThenInclude(a => a.Avdelingtype)
                    .Include(s => s.Avdeling)
                        .ThenInclude(a => a.Klinikker)
                    .Include(s => s.Observasjoner)
                        .ThenInclude(o => o.Rolle)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Aktivitet)
                        .ThenInclude(a => a.AktivitetType)
                    .Include(o => o.Observasjoner)
                        .ThenInclude(o => o.Indikasjonstyper)
                    .Where(s =>
                        s.Avdeling.Klinikker.Any(k => k.Id == klinikkId)
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date)
                        && s.Observasjoner.Any(o => o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date))
                    .ToList();

                if (request.Rolle == AuthorizedRole.Administrator)
                {
                    tilknyttetKlinikkSesjoner = tilknyttetKlinikkSesjoner.Where(p => p.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var sesjon in tilknyttetKlinikkSesjoner)
                {
                    sesjon.Observasjoner = sesjon.Observasjoner.Where(o =>
                            o.Registrerttidspunkt.Date >= request.FraTidspunkt.Date &&
                            o.Registrerttidspunkt.Date <= request.TilTidspunkt.Date)
                        .ToList();
                }

                var antallObservasjoner = tilknyttetKlinikkSesjoner.SelectMany(o => o.Observasjoner).Count();
                var klinikknavn = _context.Clinic.FirstOrDefault(k => k.Id == klinikkId)?.Name ?? "Uten navn";

                var rapport = new FireIndikasjonerRapport()
                {
                    Navn = $"Klinikk: {klinikknavn}",
                    FraTidspunkt = request.FraTidspunkt,
                    TilTidspunkt = request.TilTidspunkt,
                    Roller = HentRolleMedKombinasjonerRapportListe(tilknyttetKlinikkSesjoner),
                    AntallObservasjoner = antallObservasjoner,
                    DebugObservasjonerStringList = tilknyttetKlinikkSesjoner.SelectMany(o => o.Observasjoner)
                        .Select(o => DebugObservasjon(o))
                        .ToArray()
                };

                return rapport;
            }

            #region Kombinasjoner
            /// <summary>
            ///  * 1: Før pasient
            ///  * 2: Aseptisk
            ///  * 3: Kroppsvæske
            ///  * 4: Etter pasient
            ///  *
            ///  * Forslag fra fagavdelingen v/Mette Fagernes til gruppering av indikasjoner:
            ///  A(før pasient) = 1, 1+2
            ///  B(før aseptisk – inne i sonen) =2, 3+2
            ///  C(etter kroppsvæske – primært inne i sonen) = 3, 
            ///  D(etter pasient) = 4, 3+4, 
            ///  E(overgang mellom pasienter) =4+1, 3+4+1, 3+1, 3+1+2, 4+2, 4+1+2, 3+4+1+2, 3+4+2,
            /// 
            /// </summary>
            /// <param name="sesjoner"></param>
            /// <returns></returns>
            private List<RolleMedKombinasjonerRapport> HentRolleMedKombinasjonerRapportListe(List<FourIndicationsSession> sesjoner)
            {
                var dtoListe = new List<RolleMedKombinasjonerRapport>();
                var groupedByRoles = sesjoner.SelectMany(s => s.Observations).GroupBy(o => new { o.Role.Name });

                foreach (var rolleObservasjoner in groupedByRoles)
                {
                    var handvaskTider = new List<int>();
                    var desinfeksjonTider = new List<int>();

                    var dto = new RolleMedKombinasjonerRapport();
                    dto.Navn = rolleObservasjoner.Key.Navn;
                    dto.TotaltAntallObservasjoner = rolleObservasjoner.Count();
                    var observasjoner = rolleObservasjoner.ToList();
                    handvaskTider.AddRange(
                        observasjoner
                            .Where(o => o.Activity.TimeRecordingWasDone && o.Activity.ActivityType.Code == ActivityTypeConstants.Handwash)
                            .Select(o => o.Activity.TimeSpent));
                    desinfeksjonTider.AddRange(
                        observasjoner
                            .Where(o => o.Activity.TimeRecordingWasDone && o.Activity.ActivityType.Code == ActivityTypeConstants.Disinfection)
                            .Select(o => o.Activity.TimeSpent));

                    dto.Kombinasjoner.Add(LagKombinasjonA(observasjoner));
                    dto.Kombinasjoner.Add(LagKombinasjonB(observasjoner));
                    dto.Kombinasjoner.Add(LagKombinasjonC(observasjoner));
                    dto.Kombinasjoner.Add(LagKombinasjonD(observasjoner));
                    dto.Kombinasjoner.Add(LagKombinasjonE(observasjoner));

                    dtoListe.Add(dto);

                    if (handvaskTider.Any())
                    {
                        dto.SnittidHandvask = handvaskTider.Average().ToString();
                    }
                    if (desinfeksjonTider.Any())
                    {
                        dto.SnittidHanddesinfeksjon = desinfeksjonTider.Average().ToString();
                    }
                }
                return dtoListe;
            }

            /// <summary>
            /// A (før pasient) = 1, 1+2.
            /// * 1: Før pasient
            /// * 2: Aseptisk
            /// * 3: Kroppsvæske
            /// * 4: Etter pasient
            /// </summary>
            /// <param name="observasjoner"></param>
            /// <returns></returns>
            private Kombinasjon LagKombinasjonA(List<FourIndicationsObservation> observasjoner)
            {
                //var navn = "A (Før pasient)";
                var navn = "A";
                var kombinasjoner = new[]
                {
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BeforePatient}}, // 1
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 1+2
                };
                return LagKombinasjon(observasjoner, navn, kombinasjoner);
            }

            /// <summary>
            ///  B (før aseptisk – inne i sonen) =2, 3+2
            /// * 1: Før pasient
            /// * 2: Aseptisk
            /// * 3: Kroppsvæske
            /// * 4: Etter pasient
            /// </summary>
            /// <param name="observasjoner"></param>
            /// <returns></returns>
            private Kombinasjon LagKombinasjonB(List<FourIndicationsObservation> observasjoner)
            {
                //var navn = "B (før aseptisk – inne i sonen)";
                var navn = "B";
                var kombinasjoner = new[]
                {
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.AsepticProcedures}}, // 2
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AsepticProcedures}} // 3 + 2
                };
                return LagKombinasjon(observasjoner, navn, kombinasjoner);
            }

            /// <summary>
            ///  C (etter kroppsvæske – primært inne i sonen) = 3,
            /// * 1: Før pasient
            /// * 2: Aseptisk
            /// * 3: Kroppsvæske
            /// * 4: Etter pasient
            /// </summary>
            /// <param name="observasjoner"></param>
            /// <returns></returns>
            private Kombinasjon LagKombinasjonC(List<FourIndicationsObservation> observasjoner)
            {
                //var navn = "C (etter kroppsvæske – primært inne i sonen)";
                var navn = "C";
                var kombinasjoner = new[]
                {
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid}} // 3
                };
                return LagKombinasjon(observasjoner, navn, kombinasjoner);
            }


            /// <summary>
            /// D (etter pasient) = 4, 3+4
            /// * 1: Før pasient
            /// * 2: Aseptisk
            /// * 3: Kroppsvæske
            /// * 4: Etter pasient
            /// </summary>
            /// <param name="observasjoner"></param>
            /// <returns></returns>
            private Kombinasjon LagKombinasjonD(List<FourIndicationsObservation> observasjoner)
            {
                //var navn = "D (etter pasient)";
                var navn = "D";

                var kombinasjoner = new[]
                {
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.AfterPatient}}, // 4
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient}} // 3+4
                };
                return LagKombinasjon(observasjoner, navn, kombinasjoner);
            }

            /// <summary>
            ///  E (overgang mellom pasienter) =4+1, 3+4+1, 3+1, 3+1+2, 4+2, 4+1+2, 3+4+1+2, 3+4+2,
            /// * 1: Før pasient
            /// * 2: Aseptisk
            /// * 3: Kroppsvæske
            /// * 4: Etter pasient
            /// </summary>
            /// <param name="observasjoner"></param>
            /// <returns></returns>
            private Kombinasjon LagKombinasjonE(List<FourIndicationsObservation> observasjoner)
            {
                //var navn = "E (overgang mellom pasienter)";
                var navn = "E";
                var kombinasjoner = new[]
                {
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient}}, // 4 + 1
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient}}, // 3 + 4 + 1
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.BeforePatient}}, // 3 + 1
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 3 + 1 + 2
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.AfterPatient, IndicationTypeConstants.AsepticProcedures}}, // 4 + 2
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 4 + 1 + 2
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 3 + 4 + 1 + 2
                    new Indikasjonskombinasjon() {Koder = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient, IndicationTypeConstants.AsepticProcedures}}, // 3 + 4 + 2
                };
                return LagKombinasjon(observasjoner, navn, kombinasjoner);
            }


            private Kombinasjon LagKombinasjon(List<FourIndicationsObservation> observasjoner, string kombinasjonsnavn, params Indikasjonskombinasjon[] indikasjonskombinasjoner)
            {
                var aktuelleObservasjoner = observasjoner
                    .Where(o => MøterKombinasjonskriteriet(o.IndicationTypes.Select(i => i.Code), indikasjonskombinasjoner)).ToList();
                var antall = aktuelleObservasjoner.Count();
                var etterlevd = aktuelleObservasjoner
                    .Count(o => o.Activity.ActivityType.Code != ActivityTypeConstants.NotExecuted);

                var etterlevdProsentandel = BeregnEtterlevdProsentandel(antall, etterlevd);
                var ikkeEtterlevdProsentandel = BeregnIkkeEtterlevdProsentandel(antall, etterlevdProsentandel);
                var kombinasjon = new Kombinasjon()
                {
                    Navn = $"{kombinasjonsnavn}",
                    Rolle = observasjoner.First().Role.Name,
                    AntallObservasjoner = antall,
                    ProsentEtterlevd = etterlevdProsentandel,
                    ProsentIkkeEtterlevd = ikkeEtterlevdProsentandel,
                };
                return kombinasjon;
            }

            private double BeregnEtterlevdProsentandel(int antall, int etterlevd)
            {
                if (antall == 0 || etterlevd == 0)
                    return 0.0f;
                return (etterlevd / (double)antall) * 100;
            }

            private double BeregnIkkeEtterlevdProsentandel(int antall, double etterlevd)
            {
                if (antall > 0)
                {
                    return (100d - etterlevd);
                }

                return 0.0d;
            }

            private bool MøterKombinasjonskriteriet(IEnumerable<string> indikasjonstyper, Indikasjonskombinasjon[] indikasjonskombinasjoner)
            {
                foreach (var kombinasjon in indikasjonskombinasjoner)
                {
                    if (indikasjonstyper.OrderBy(i => i).SequenceEqual(kombinasjon.Koder.OrderBy(k => k)))
                    {
                        return true;
                    }
                }
                return false;
            }
            #endregion

            private string DebugObservasjon(FourIndicationsObservation o)
            {
                return
                    $"{o.Id}, rolle: {o.Role.Name} (id:{o.Role.Id} {o.Activity.ActivityType.Code} {string.Join(',', o.IndicationTypes.Select(i => i.Code))}";
            }

            private struct Indikasjonskombinasjon
            {
                public string[] Koder { get; set; }
            }
        }


    }
}
