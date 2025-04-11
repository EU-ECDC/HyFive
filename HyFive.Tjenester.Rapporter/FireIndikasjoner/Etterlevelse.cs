using HyFive.DataAccess;
using HyFive.Domene.Observation;
using HyFive.Modeller.V1.Konstanter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Rapporter.FireIndikasjoner
{
    public class Etterlevelse
    {
        public class Query : IRequest<List<GrafDto>>
        {
            public int InstitusjonId { get; set; }
            public string Intervall { get; set; }
            public int FraManed { get; set; }
            public int FraAr { get; set; }
            public int TilManed { get; set; }
            public int TilAr { get; set; }
            public int? RolleId { get; set; }
            public int? AvdelingId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<GrafDto>>
        {
            private readonly HandHygieneContext _context;
            private const string IntervallUke = "uke";
            private const string IntervallManed = "maned";
            private const string IntervallTertial = "tertial";

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<List<GrafDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var fraDato = new DateTime(request.FraAr, 1, 1);
                var tilDato = new DateTime(request.TilAr, 1, 1);
                if (request.Intervall == IntervallUke)
                {
                    fraDato = fraDato.AddMonths(request.FraManed - 1);
                    tilDato = tilDato.AddMonths(request.TilManed);
                }
                else
                {
                    tilDato = tilDato.AddYears(1);
                }

                var observasjonerIAktueltTidromQuery = _context.FourIndicationsObservation.Include(f => f.Activity.ActivityType)
                                                                                      .Include(f => f.IndicationTypes)
                                                                                      .Include(f => f.Role)
                                                                                      .AsNoTracking()
                                                                                      .Where(f => f.RegistrationTime >= fraDato &&
                                                                                                  f.RegistrationTime < tilDato &&
                                                                                                  f.FourIndicationsSession.Department.Institusjon.Id == request.InstitusjonId);

                if (request.RolleId != null)
                {
                    observasjonerIAktueltTidromQuery = observasjonerIAktueltTidromQuery.Where(x => x.Rolle.Id == request.RolleId);
                }

                if (request.AvdelingId != null)
                {
                    observasjonerIAktueltTidromQuery = observasjonerIAktueltTidromQuery.Where(x => x.FireIndikasjonerSesjon.Avdeling.Id == request.AvdelingId);
                }

                var observasjonerIAktueltTidrom = observasjonerIAktueltTidromQuery.ToList();
                var etterlevelseGrafData = LagEtterlevelseGrafData(observasjonerIAktueltTidrom, request.Intervall, fraDato, tilDato);

                FjernElementerMedIngenRegistreringerIBegynnelsenAvSokePerioden(etterlevelseGrafData);
                FjernElementerMedIngenRegistreringerISluttenAvSokePerioden(etterlevelseGrafData);

                var grafProsent = LagGraf(etterlevelseGrafData, "Etterlevelse (%)", true);
                var grafAntall = LagGraf(etterlevelseGrafData, "Etterlevelse antall", false);

                return new List<GrafDto> { grafProsent, grafAntall };
            }

            private static GrafDto LagGraf(List<EtterlevelseGrafData> etterlevelseGrafData, string tittel, bool brukProsent)
            {
                List<GrafDataDto> grafData = MapEtterlevelseTilGrafdata(etterlevelseGrafData, brukProsent);

                var graf = new GrafDto
                {
                    Tittel = tittel,
                    GrafDataListe = grafData
                };
                return graf;
            }

            private static List<GrafDataDto> MapEtterlevelseTilGrafdata(List<EtterlevelseGrafData> etterlevelseGrafDataListe, bool brukProsent)
            {
                var grafDataDtoListe = new List<GrafDataDto>();
                foreach (var etterlevelseGrafData in etterlevelseGrafDataListe)
                {
                    var punktDtoListe = new List<PunktDto>();
                    foreach (var punkt in etterlevelseGrafData.Data)
                    {
                        var punktDto = new PunktDto()
                        {
                            Name = punkt.Name,
                            Y = brukProsent ? punkt.Prosent : punkt.Antall
                        };
                        punktDtoListe.Add(punktDto);
                    }

                    grafDataDtoListe.Add(new GrafDataDto()
                    {
                        Name = etterlevelseGrafData.Name,
                        Data = punktDtoListe
                    });
                }

                return grafDataDtoListe;
            }

            private static List<EtterlevelseGrafData> LagEtterlevelseGrafData(List<FourIndicationsObservation> observasjonerIAktueltTidrom, string intervall, DateTime fraDato, DateTime tilDato)
            {
                var etterlevelseAlleIndikasjoner = LagEtterlevelseForAlleIndiksasjoner(intervall, observasjonerIAktueltTidrom, fraDato, tilDato);
                var etterlevelseFørPasient = LagEtterlevelseGrafDataForIndikasjon(intervall, observasjonerIAktueltTidrom, "Før pasient", IndikasjonTypeKonstanter.FoerPasient, fraDato, tilDato);
                var etterlevelseAseptisk = LagEtterlevelseGrafDataForIndikasjon(intervall, observasjonerIAktueltTidrom, "Aseptisk", IndikasjonTypeKonstanter.AseptiskeProsedyrer, fraDato, tilDato);
                var etterlevelseKroppsvæske = LagEtterlevelseGrafDataForIndikasjon(intervall, observasjonerIAktueltTidrom, "Kroppsvæske", IndikasjonTypeKonstanter.Kroppsveske, fraDato, tilDato);
                var etterlevelseEtterPasient = LagEtterlevelseGrafDataForIndikasjon(intervall, observasjonerIAktueltTidrom, "Etter pasient", IndikasjonTypeKonstanter.EtterPasient, fraDato, tilDato);

                return new List<EtterlevelseGrafData>
                        {
                            etterlevelseAlleIndikasjoner,
                            etterlevelseFørPasient,
                            etterlevelseAseptisk,
                            etterlevelseKroppsvæske,
                            etterlevelseEtterPasient
                        };
            }

            private static EtterlevelseGrafData LagEtterlevelseForAlleIndiksasjoner(string intervall, List<FourIndicationsObservation> observasjonerIAktueltTidrom, DateTime fraDato, DateTime tilDato)
            {
                var etterlevelseFAlleIndikasjoner = new EtterlevelseGrafData
                {
                    Name = "Alle indikasjoner"
                };

                var periodeTilDato = fraDato;
                var punktliste = new List<EtterlevelsePunkt>();
                while (periodeTilDato < tilDato)
                {
                    var periodeFraDato = periodeTilDato;
                    periodeTilDato = BeregnNestePeriodeTilDato(intervall, periodeTilDato);

                    var observasjonerIPeriode = observasjonerIAktueltTidrom.Where(o => o.RegistrationTime >= periodeFraDato && o.RegistrationTime < periodeTilDato);

                    var indikasjoner = observasjonerIPeriode.Select(x => x.IndicationTypes);
                    decimal antallIndikasjoner = indikasjoner.Sum(item => item.Count);

                    var etterlevdeIndikasjoner = observasjonerIPeriode.Where(o => o.Activity.ActivityType.Code == AktivitetTypeKonstanter.Handvask ||
                                                                                                        o.Activity.ActivityType.Code == AktivitetTypeKonstanter.Desinfeksjon)
                                                                                            .Select(o => o.IndicationTypes);

                    decimal antallEtterlevdeIndikasjoner = etterlevdeIndikasjoner.Sum(item => item.Count);

                    string periodenavn = BeregnPeriodenavn(intervall, periodeFraDato, periodeTilDato);
                    var punkt = LagPunkt(antallIndikasjoner, antallEtterlevdeIndikasjoner, periodenavn);
                    punktliste.Add(punkt);
                }

                etterlevelseFAlleIndikasjoner.Data = punktliste;

                return etterlevelseFAlleIndikasjoner;
            }

            private static EtterlevelseGrafData LagEtterlevelseGrafDataForIndikasjon(string intervall, List<FourIndicationsObservation> observasjonerIAktueltTidrom, string tittel, string indikasjonstype, DateTime fraDato, DateTime tilDato)
            {
                var etterlevelse = new EtterlevelseGrafData
                {
                    Name = tittel
                };

                var observasjonerMeIndikasjonstype = observasjonerIAktueltTidrom.Where(o => o.IndicationTypes.Any(i => i.Code == indikasjonstype)).ToList();
                etterlevelse.Data = LagGrafDataForIndikasjon(observasjonerMeIndikasjonstype, intervall, fraDato, tilDato);
                return etterlevelse;
            }

            private static List<EtterlevelsePunkt> LagGrafDataForIndikasjon(List<FourIndicationsObservation> observasjonerIAktueltTidrom, string intervall, DateTime fraDato, DateTime tilDato)
            {
                var periodeTilDato = fraDato;
                var punktliste = new List<EtterlevelsePunkt>();
                while (periodeTilDato < tilDato)
                {
                    var periodeFraDato = periodeTilDato;
                    periodeTilDato = BeregnNestePeriodeTilDato(intervall, periodeTilDato);

                    var observasjonerIPeriode = observasjonerIAktueltTidrom.Where(o => o.RegistrationTime >= periodeFraDato && o.RegistrationTime < periodeTilDato);
                    var etterlevdeObservasjonerIPeriode = observasjonerIPeriode.Where(o => o.Activity.ActivityType.Code == AktivitetTypeKonstanter.Handvask || o.Activity.ActivityType.Code == AktivitetTypeKonstanter.Desinfeksjon);

                    decimal antallObservasjonerIPeriode = observasjonerIPeriode.Count();
                    decimal antallEtterlevdeObservasjonerIPeriode = etterlevdeObservasjonerIPeriode.Count();

                    string periodenavn = BeregnPeriodenavn(intervall, periodeFraDato, periodeTilDato);
                    var punkt = LagPunkt(antallObservasjonerIPeriode, antallEtterlevdeObservasjonerIPeriode, periodenavn);

                    punktliste.Add(punkt);
                }

                return punktliste;
            }

            private static EtterlevelsePunkt LagPunkt(decimal antallObservasjonerIPeriode, decimal antallEtterlevdeObservasjonerIPeriode, string periodenavn)
            {
                decimal prosentEtterlevd = 0;

                if (antallObservasjonerIPeriode > 0)
                {
                    prosentEtterlevd = (antallEtterlevdeObservasjonerIPeriode / antallObservasjonerIPeriode) * 100;
                }

                EtterlevelsePunkt punkt = new()
                {
                    Name = periodenavn,
                    Antall = antallObservasjonerIPeriode,
                    Prosent = prosentEtterlevd
                };
                return punkt;
            }

            private static string BeregnPeriodenavn(string intervall, DateTime periodeFraDato, DateTime periodeTilDato)
            {
                if (intervall == IntervallUke)
                {
                    return $"{periodeFraDato:d MMM} - {periodeTilDato:d MMM yy}";
                }
                else if (intervall == IntervallManed)
                {
                    return $"{periodeFraDato:MMMM yy}";
                }
                else if (intervall == IntervallTertial)
                {
                    return $"{periodeFraDato:MMM yy} - {periodeTilDato.AddMonths(-1):MMM yy}";
                }

                return "";
            }

            private static DateTime BeregnNestePeriodeTilDato(string intervall, DateTime periodeTilDato)
            {
                if (intervall == IntervallUke)
                {
                    return periodeTilDato.AddDays(7);
                }
                else if (intervall == IntervallManed)
                {
                    return periodeTilDato.AddMonths(1);
                }
                else if (intervall == IntervallTertial)
                {
                    return periodeTilDato.AddMonths(4);
                }
                else
                {
                    throw new Exception("Intervall må være uke, måned eller tertial.");
                }
            }

            private static void FjernElementerMedIngenRegistreringerIBegynnelsenAvSokePerioden(List<EtterlevelseGrafData> etterlevelseGrafData)
            {
                var alleIndikasjonerGrafdata = etterlevelseGrafData.First(x => x.Name == "Alle indikasjoner");

                var antallElementerSomSkalFjernes = 0;
                foreach (var data in alleIndikasjonerGrafdata.Data)
                {
                    if (data.Antall > 0)
                    {
                        break;
                    }
                    antallElementerSomSkalFjernes++;
                }

                foreach (var grafdata in etterlevelseGrafData)
                {
                    grafdata.Data.RemoveRange(0, antallElementerSomSkalFjernes);
                }
            }

            private void FjernElementerMedIngenRegistreringerISluttenAvSokePerioden(List<EtterlevelseGrafData> etterlevelseGrafData)
            {
                var alleIndikasjonerGrafdata = etterlevelseGrafData.First(x => x.Name == "Alle indikasjoner");

                var antallElementerSomSkalFjernes = 0;

                var antallElementer = alleIndikasjonerGrafdata.Data.Count;
                for (var i = antallElementer; i > 0; i--)
                {
                    if (alleIndikasjonerGrafdata.Data[i - 1].Antall > 0)
                    {
                        break;
                    }

                    antallElementerSomSkalFjernes++;
                }

                foreach (var grafdata in etterlevelseGrafData)
                {
                    int index = antallElementer - antallElementerSomSkalFjernes;
                    grafdata.Data.RemoveRange(index, antallElementerSomSkalFjernes);
                }
            }
        }
    }
}
