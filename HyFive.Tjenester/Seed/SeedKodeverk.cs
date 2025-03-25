using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;
using HyFive.Domene.Observasjon.Hansker;
using HyFive.Domene.Sesjon;
using HyFive.Modeller.V1.Konstanter;
using MediatR;

namespace HyFive.Tjenester.Seed
{
    public class SeedKodeverk
    {
        public class Command : IRequest<bool>
        {
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }


            public Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                SeedOverforingstatusTyper();
                SeedAktivitetTyper();
                SeedIndikasjoner();
                SeedHandsmykkeTyper();
                SeedBeskyttelsesutstyrTyper();
                SeedBeskyttelsesutstyrsettingTyper();
                SeedHanskeMedIndikasjonTyper();
                SeedHanskeUtenIndikasjonTyper();
                SeedHandhygieneEtterHanskebrukType();
                SeedFhiAdmin();

                return Task.FromResult(true);
            }

            private void SeedHandhygieneEtterHanskebrukType()
            {
                if (_context.HandhygieneEtterHanskebrukType.Any())
                    return;

                var handhygieneEtterHanskebrukTyper = new List<HandhygieneEtterHanskebrukType>
                {
                    new HandhygieneEtterHanskebrukType {Kode = "IKKE_INDIKERT", Navn = "Ikke Indikert"},
                    new HandhygieneEtterHanskebrukType {Kode = "NEI", Navn = "Nei"},
                    new HandhygieneEtterHanskebrukType {Kode = "JA", Navn = "Ja"},
                };

                _context.HandhygieneEtterHanskebrukType.AddRange(handhygieneEtterHanskebrukTyper);
                _context.SaveChanges();
            }

            private void SeedHanskeUtenIndikasjonTyper()
            {
                if (_context.HanskeUtenIndikasjonType.Any())
                    return;

                var hanskerUtenIndikasjonTyper = new List<HanskeUtenIndikasjonType>
                {
                    new HanskeUtenIndikasjonType {Kode = "ANNET", Navn = "Annet"},
                    new HanskeUtenIndikasjonType {Kode = "MAT", Navn = "Mat"},
                    new HanskeUtenIndikasjonType {Kode = "STELL_UTEN_KROPPVAESKER", Navn = "Stell uten kroppvæsker"},
                };

                _context.HanskeUtenIndikasjonType.AddRange(hanskerUtenIndikasjonTyper);
                _context.SaveChanges();
            }

            private void SeedOverforingstatusTyper()
            {
                if (_context.OverforingstatusType.Any())
                    return;

                var overforingsstatuser = new[]
                {
                    new OverforingstatusType
                    {
                        Kode = OverforingstatusTypeKonstanter.OverfortTilKoordinator,
                        Navn = "Overført til Koordinator"
                    },
                    new OverforingstatusType
                    {
                        Kode = OverforingstatusTypeKonstanter.OverfortTilFhi,
                        Navn = "Overført til FHI"
                    }
                };
                _context.OverforingstatusType.AddRange(overforingsstatuser);
                _context.SaveChanges();
            }

            private void SeedHanskeMedIndikasjonTyper()
            {
                if (_context.HanskeMedIndikasjonType.Any())
                    return;

                var hanskerMedIndikasjonTyper = new List<HanskeMedIndikasjonType>
                {
                    new HanskeMedIndikasjonType {Kode = "ANNET", Navn = "Annet"},
                    new HanskeMedIndikasjonType {Kode = "SMITTE", Navn = "Smitte"},
                    new HanskeMedIndikasjonType {Kode = "KROPPVAESKER", Navn = "Kroppvæsker"},
                };

                _context.HanskeMedIndikasjonType.AddRange(hanskerMedIndikasjonTyper);
                _context.SaveChanges();
            }

            private void SeedFhiAdmin()
            {
                if (_context.FhiAdmin.Any() == false)
                {
                    _context.FhiAdmin.Add(new FhiAdmin
                    {
                        Fornavn = "Grønn",
                        Etternavn = "Vits",
                        IdentPseudonym = "OCW6BpVN57vnbxBUE8WOOTM9FrkCaBixlD2y8FgYCag="
                    });
                }
                _context.SaveChanges();
            }

            private void SeedIndikasjoner()
            {
                if (_context.Indikasjon.Any())
                    return;

                var indikasjoner = new[]
                {
                    new IndikasjonType {Kode = "FOER_PASIENT", Navn = "Før pasient", Nummer = "1"},
                    new IndikasjonType {Kode = "ASEPTISKE_PROSEDYRER", Navn = "Aseptisk", Nummer = "2"},
                    new IndikasjonType {Kode = "KROPPSVESKE", Navn = "Kroppsvæske", Nummer = "3"},
                    new IndikasjonType {Kode = "ETTER_PASIENT", Navn = "Etter pasient", Nummer = "4"}
                };
                _context.Indikasjon.AddRange(indikasjoner);
                _context.SaveChanges();
            }

            private void SeedAktivitetTyper()
            {
                if (_context.AktivitetType.Any())
                    return;

                var aktivitettyper = new[]
                {
                    new AktivitetType {Kode = "DESINFEKSJON", Navn = "Desinfeksjon"},
                    new AktivitetType {Kode = "HANDVASK", Navn = "Håndvask"},
                    new AktivitetType {Kode = "IKKE_UTFORT", Navn = "Ikke utført"},
                    new AktivitetType {Kode = "IKKE_REGISTRERT", Navn = "Ikke registrert"}
                };
                _context.AktivitetType.AddRange(aktivitettyper);
                _context.SaveChanges();
            }

            private void SeedHandsmykkeTyper()
            {
                if (_context.HandsmykkeType.Any())
                    return;

                var handsmykketyper = new[]
                {
                    new HandsmykkeType {Kode = "ALT_OK", Navn = "Alt er ok", Rekkefolge = 99, ErAktiv = true},
                    new HandsmykkeType {Kode = "RING", Navn = "Ring", Rekkefolge = 4 , ErAktiv = true},
                    new HandsmykkeType {Kode = "KLOKKE_ARMBAND", Navn = "Klokke Armbånd", Rekkefolge = 3, ErAktiv = true},
                    new HandsmykkeType {Kode = "LANG_NEGL", Navn = "Lang negl", Rekkefolge = 2, ErAktiv = true},
                    new HandsmykkeType {Kode = "KUNSTIG_NEGL_SHELLAC", Navn = "Kunstig negl Shellack", Rekkefolge = 1, ErAktiv = true},
                    new HandsmykkeType {Kode = "KORTERMET", Navn = "Kortermet", Rekkefolge = 5, ErAktiv = false},
                    new HandsmykkeType {Kode = "LANGERMET", Navn = "Langermet", Rekkefolge = 5, ErAktiv = true}
                };

                _context.HandsmykkeType.AddRange(handsmykketyper);
                _context.SaveChanges();
            }

            private void SeedBeskyttelsesutstyrTyper()
            {
                if (_context.BeskyttelsesutstyrType.Any() == false)
                {
                    var beskyttelsesutstyrtyper = LagBeskyttelsesutstyrTyper();
                    _context.BeskyttelsesutstyrType.AddRange(beskyttelsesutstyrtyper);
                    _context.SaveChanges();
                }
            }

            private void SeedBeskyttelsesutstyrsettingTyper()
            {
                if (_context.BeskyttelsesutstyrsettingType.Any() == false)
                {
                    var beskyttelsesutstyrtyper = _context.BeskyttelsesutstyrType.ToList();
                    var settingtyper = LagSeedForBeskyttelsesutstyrsettingTyper(beskyttelsesutstyrtyper);
                    _context.BeskyttelsesutstyrsettingType.AddRange(settingtyper);
                    _context.SaveChanges();
                }
            }

            private static IEnumerable<BeskyttelsesutstyrType> LagBeskyttelsesutstyrTyper()
            {
                return new List<BeskyttelsesutstyrType>
                {
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Hette,
                        Navn = "Hette",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil teknikk ved påtagelse"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse,
                        Navn = "Øyebeskyttelse",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil teknikk ved påtagelse"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Andedrettsvern,
                        Navn = "Åndedrettsvern",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil teknikk ved påtagelse"},
                            new FeilbrukType {Navn = "Ikke tilpasset/utført fit-sjekk"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Munnbind,
                        Navn = "Munnbind",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil teknikk ved påtagelse"},
                            new FeilbrukType {Navn = "Løst festet rundt nese/munn"},
                            new FeilbrukType {Navn = "Ikke festet over nese"},
                            new FeilbrukType {Navn = "Ikke trukket under hake"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Smittefrakk,
                        Navn = "Smittefrakk",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil bruk ved påtagelse"},
                            new FeilbrukType {Navn = "Ikke lukket skikkelig"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    }
                    ,
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Stellefrakk,
                        Navn = "Stellefrakk",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil bruk ved påtagelse"},
                            new FeilbrukType {Navn = "Ikke lukket skikkelig"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Plastforkle,
                        Navn = "Plastforkle",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil bruk ved påtagelse"},
                            new FeilbrukType {Navn = "Ikke lukket skikkelig"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new BeskyttelsesutstyrType
                    {
                        Kode = BeskyttelsesutstyrTypeKonstanter.Hansker,
                        Navn = "Hansker",
                        Feilbruktyper = new List<FeilbrukType>
                        {
                            new FeilbrukType {Navn = "Feil teknikk ved påtagelse"},
                            new FeilbrukType {Navn = "Feil teknikk ved avtagelse"},
                            new FeilbrukType {Navn = "Ikke festet over mansjett"}
                        }
                    }
                };
            }

            public List<BeskyttelsesutstyrsettingType> LagSeedForBeskyttelsesutstyrsettingTyper(List<BeskyttelsesutstyrType> typer)
            {
                var settingtyper = new List<BeskyttelsesutstyrsettingType>
                {
                    new BeskyttelsesutstyrsettingType
                    {
                        Kode = BeskyttelsesutstyrsettingTypeKonstanter.Luftsmitte,
                        Navn = "Luftsmitte",
                    },
                    new BeskyttelsesutstyrsettingType
                    {
                        Kode = BeskyttelsesutstyrsettingTypeKonstanter.Drapesmitte,
                        Navn = "Dråpesmitte",
                    },
                    new BeskyttelsesutstyrsettingType
                    {
                        Kode = BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte,
                        Navn = "Kontaktsmitte",
                    },
                    new BeskyttelsesutstyrsettingType
                    {
                        Kode = BeskyttelsesutstyrsettingTypeKonstanter.BasaleSmittevernrutiner,
                        Navn = "Basale smittevernrutiner",
                    }
                };

                foreach (var settingtype in settingtyper)
                {
                    if (settingtype.Kode == BeskyttelsesutstyrsettingTypeKonstanter.Luftsmitte)
                    {
                        settingtype.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                                                BeskyttelsesutstyrTypeKonstanter.Andedrettsvern, 
                                                BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse,
                                                BeskyttelsesutstyrTypeKonstanter.Hansker, 
                                                BeskyttelsesutstyrTypeKonstanter.Smittefrakk);
                    }
                    else if (settingtype.Kode == BeskyttelsesutstyrsettingTypeKonstanter.Drapesmitte)
                    {
                        settingtype.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                                                BeskyttelsesutstyrTypeKonstanter.Hansker, 
                                                BeskyttelsesutstyrTypeKonstanter.Smittefrakk,
                                                BeskyttelsesutstyrTypeKonstanter.Munnbind, 
                                                BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse);
                    }
                    else if (settingtype.Kode == BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte)
                    {
                        settingtype.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                                                BeskyttelsesutstyrTypeKonstanter.Hansker, 
                                                BeskyttelsesutstyrTypeKonstanter.Smittefrakk);
                    }
                    else if (settingtype.Kode == BeskyttelsesutstyrsettingTypeKonstanter.BasaleSmittevernrutiner)
                    {
                        settingtype.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer);
                    }
                }

                return settingtyper;
            }

            private static List<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType> HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(BeskyttelsesutstyrsettingType settingType,
                    IEnumerable<BeskyttelsesutstyrType> utstyrstyper, params string[] defaultKoder)
            {
                var settingutstyrkoblinger = utstyrstyper.Select(u =>
                    new BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType
                    {
                        BeskyttelsesutstyrType = u,
                        ErDefaultIndikert = defaultKoder.Contains(u.Kode),
                        BeskyttelsesutstyrsettingType = settingType
                    }).ToList();

                return settingutstyrkoblinger;
            }
        }
    }
}