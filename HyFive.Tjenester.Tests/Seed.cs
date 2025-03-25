using System.Collections.Generic;
using System.Linq;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;
using HyFive.Domene.Observasjon.Hansker;
using HyFive.Domene.Sesjon;
using HyFive.Domene.Sted;
using HyFive.Modeller.V1.Konstanter;

namespace HyFive.Tjenester.Tests
{
    public class Seed
    {
        private readonly HandhygieneContext _context;

        //Test-person (meldt ønsket statisk i PREG) LINE DANSER, ident 13116900216, HPR-nummer 9383840
        public const string SeedObservatorHprNummer = "9383840";
        public const string SeedObservatorFornavn = "Line";
        public const string SeedObservatorEtternavn = "Danser";

        // Test-person (meldt ønsket statisk i PREG) Anne Markussen, ident 15037104229, HPR-nummer 4909402
        public const string SeedKoordinatorHprNummer = "4909402";
        public const string SeedKoordinatorFornavn = "ANNE";
        public const string SeedKoordinatorEtternavn = "MARKUSSEN";

        // Test-person (meldt ønsket statisk i PREG) FELIX MØRK, ident 22127113177
        public const string SeedFhiAdminIdentPseudonym = "5ihl4KLeBmEag6zdpKH8TDOMJXBtRDTz4I02JZbn+Zc=";
        public const string SeedFhiAdminFornavn = "FELIX";
        public const string SeedFhiAdminEtternavn = "MØRK";

        public Seed(HandhygieneContext context)
        {
            _context = context;
        }

        public void SeedData()
        {
            SeedRegioner();
            SeedInstitusjonTyper();
            SeedOverforingstatusTyper();
            SeedAvdelingTyper();
            SeedInstitusjoner();
            SeedAktivitetTyper();
            SeedIndikasjoner();
            SeedHandsmykkeTyper();
            SeedBeskyttelsesutstyrTyper();
            SeedBeskyttelsesutstyrsettingTyper();
            SeedBrukere();
            SeedHanskeMedIndikasjonTyper();
            SeedHanskeUtenIndikasjonTyper();
            SeedHandhygieneEtterHanskebrukType();
        }

        private void SeedRegioner()
        {
            var regioner = new List<Domene.Sted.Region>
                {
                    new Domene.Sted.Region {Kode = "HELSE_SOR_OST", Navn = "Helse Sør-Øst"},
                    new Domene.Sted.Region {Kode = "HELSE_VEST", Navn = "Helse Vest"},
                    new Domene.Sted.Region {Kode = "HELSE_MIDT_NORGE", Navn = "Helse Midt-Norge"},
                    new Domene.Sted.Region {Kode = "HELSE_NORD", Navn = "Helse Nord"},
                    new Domene.Sted.Region {Kode = "AGDER", Navn = "Agder"},
                    new Domene.Sted.Region {Kode = "INNLANDET", Navn = "Innlandet"},
                    new Domene.Sted.Region {Kode = "MORE_OG_ROMSDAL", Navn = "Møre og Romsdal"},
                    new Domene.Sted.Region {Kode = "NORDLAND", Navn = "Nordland"},
                    new Domene.Sted.Region {Kode = "OSLO", Navn = "Oslo"},
                    new Domene.Sted.Region {Kode = "ROGALAND", Navn = "Rogaland"},
                    new Domene.Sted.Region {Kode = "VEST_OG_TELEMARK", Navn = "Vestfold og Telemark"},
                    new Domene.Sted.Region {Kode = "TROMS_OG_FINNMARK", Navn = "Troms og Finnmark"},
                    new Domene.Sted.Region {Kode = "TRONDELAG", Navn = "Trøndelag"},
                    new Domene.Sted.Region {Kode = "VESTLAND", Navn = "Vestland"},
                    new Domene.Sted.Region {Kode = "VIKEN", Navn = "Viken"},
                };

            _context.Region.AddRange(regioner);
            _context.SaveChanges();
        }

        private void SeedHandhygieneEtterHanskebrukType()
        {
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
            var overforingsstatuser = new[]
            {
                    new OverforingstatusType()
                    {
                        Kode = OverforingstatusTypeKonstanter.OverfortTilKoordinator,
                        Navn = "Overført til Koordinator"
                    },
                    new OverforingstatusType()
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
            var hanskerMedIndikasjonTyper = new List<HanskeMedIndikasjonType>
                {
                    new HanskeMedIndikasjonType {Kode = "ANNET", Navn = "Annet"},
                    new HanskeMedIndikasjonType {Kode = "SMITTE", Navn = "Smitte"},
                    new HanskeMedIndikasjonType {Kode = "KROPPVAESKER", Navn = "Kroppvæsker"},
                };

            _context.HanskeMedIndikasjonType.AddRange(hanskerMedIndikasjonTyper);
            _context.SaveChanges();
        }

        private void SeedBrukere()
        {
            _context.Observator.Add(new Observator()
            {
                Etternavn = SeedObservatorEtternavn,
                Fornavn = SeedObservatorFornavn,
                HPRNummer = SeedObservatorHprNummer,
                Institusjon = _context.Institusjon.FirstOrDefault()
            });

            _context.Observator.Add(new Observator()
            {
                Etternavn = SeedObservatorEtternavn,
                Fornavn = SeedObservatorFornavn,
                HPRNummer = SeedObservatorHprNummer,
                Institusjon = _context.Institusjon.FirstOrDefault(i => i.HERId == "93917")
            });

            _context.Koordinator.Add(new Koordinator()
            {
                Etternavn = SeedKoordinatorEtternavn,
                Fornavn = SeedKoordinatorFornavn,
                HPRNummer = SeedKoordinatorHprNummer,
                Institusjon = _context.Institusjon.FirstOrDefault()
            });

            _context.Koordinator.Add(new Koordinator()
            {
                Etternavn = SeedKoordinatorEtternavn,
                Fornavn = SeedKoordinatorFornavn,
                HPRNummer = SeedKoordinatorHprNummer,
                Institusjon = _context.Institusjon.FirstOrDefault(i => i.HERId == "93917")
            });

            _context.FhiAdmin.Add(new FhiAdmin()
            {
                Fornavn = SeedFhiAdminFornavn,
                Etternavn = SeedFhiAdminEtternavn,
                IdentPseudonym = SeedFhiAdminIdentPseudonym
            });

            _context.SaveChanges();
        }

        private void SeedIndikasjoner()
        {
            var indikasjoner = new IndikasjonType[]
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
            var aktivitettyper = new AktivitetType[]
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
            var handsmykketyper = new HandsmykkeType[]
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

        private void SeedInstitusjonTyper()
        {
            var institusjontyper = new InstitusjonType[]
            {
                    new InstitusjonType {Kode = "SYKEHUS", Navn = "Sykehus"},
                    new InstitusjonType {Kode = "SYKEHJEM", Navn = "Sykehjem"}
            };
            _context.InstitusjonType.AddRange(institusjontyper);
            _context.SaveChanges();
        }


        private void SeedAvdelingTyper()
        {
            var avdelingstyper = new AvdelingType[]
            {
                    new AvdelingType {Kode ="KIRURGI", Navn = "Kirurgi"},
                    new AvdelingType {Kode ="INDREMEDISIN", Navn = "Indremedisin"},
                    new AvdelingType {Kode ="FODSELSHJELP_OG_KVINNESYKDOMMER", Navn = "Fødselshjelp og kvinnesykdommer"},
                    new AvdelingType {Kode ="HUD_OG_VENERISKE_SYKDOMMER", Navn = "Hud- og veneriske sykdommer"},
                    new AvdelingType {Kode ="BARNESYKDOMMER", Navn = "Barnesykdommer"},
                    new AvdelingType {Kode ="NEVROLOGI", Navn = "Nevrologi"},
                    new AvdelingType {Kode ="ORE_NESE_HALS", Navn = "Øre-nese-hals"},
                    new AvdelingType {Kode ="OYESYKDOMMER", Navn = "Øyesykdommer"},
                    new AvdelingType {Kode ="ONKOLOGI", Navn = "Onkologi"},
                    new AvdelingType {Kode ="REVMATOLOGI", Navn = "Revmatologi"},
                    new AvdelingType {Kode ="FYSIKALSK_MEDISIN_REHABILITERING", Navn = "Fysikalsk medisin/rehabilitering"},
                    new AvdelingType {Kode ="OBSERVASJONSENHET_AKUTTMOTTAK", Navn = "Observasjonsenhet / akuttmottak"},
                    new AvdelingType {Kode ="KIRURGISK_INTENSIV_OVERVAKNING", Navn = "Kirurgisk intensiv/overvåking"},
                    new AvdelingType {Kode ="MEDISINSK_INTENSIV_OVERVAKNING", Navn = "Medisinsk intensiv/overvåking"},
                    new AvdelingType {Kode ="INTERMEDIARENHET", Navn = "Intermediærenhet"},
                    new AvdelingType {Kode ="SKJERMET_ENHET", Navn = "Skjermet enhet (demens)"},
                    new AvdelingType {Kode ="REHABILITERINGSENHET", Navn = "Rehabiliteringsenhet"},
                    new AvdelingType {Kode ="KORTTIDSAVDELING", Navn = "Korttidsavdeling"},
                    new AvdelingType {Kode ="LANGTIDSAVDELING", Navn = "Langtidsavdeling"},
                    new AvdelingType {Kode ="KOMBINERT_KORT_OG_LANGTIDSAVDELING", Navn = "	Kombinert kort- og langtidsavdeling"}
            };
            _context.AvdelingType.AddRange(avdelingstyper);
            _context.SaveChanges();
        }

        private void SeedInstitusjoner()
        {
            var roller = new List<Domene.Observasjon.Rolle>()
                {
                    new Domene.Observasjon.Rolle("Sykepleier"),
                    new Domene.Observasjon.Rolle("Lege"),
                    new Domene.Observasjon.Rolle("Pleiepersonell"),
                    new Domene.Observasjon.Rolle("Jordmor"),
                    new Domene.Observasjon.Rolle("Fysioterapeut"),
                    new Domene.Observasjon.Rolle("Bioingeniør"),
                    new Domene.Observasjon.Rolle("Annet")
                };
            _context.Rolle.AddRange(roller);

            var institusjontyper = _context.InstitusjonType;
            var avdelingtyper = _context.AvdelingType;

            var institusjoner = new[]
            {
                    new Domene.Sted.Institusjon()
                    {
                        Region = _context.Region.First(),
                        HERId = "87711",
                        Navn = "Oslo universitetssykehus HF",
                        Forkortelse = "OUS",
                        Institusjontype = institusjontyper.First(),
                        Avdelinger = new List<Domene.Sted.Avdeling>()
                        {
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Nevrokirurgisk",
                                Roller = new List<Domene.Observasjon.Rolle>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Allergi og lungeseksjonen",
                                Roller = new List<Domene.Observasjon.Rolle>()
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Avdeling for mikrobiologi",
                                Roller = new List<Domene.Observasjon.Rolle>()
                                {
                                    roller[5],
                                    roller[6],
                                    roller[1],
                                },
                                Avdelingtype = avdelingtyper.Skip(2).First()
                            }
                        },
                        PredefinerteKommmentarer = new List<PredefinertKommentar>()
                        {
                            new PredefinertKommentar { Kommentar = "Hansker i stedet for håndhygiene", SesjonType = SesjonType.Beskyttelsesutstyr },
                            new PredefinertKommentar { Kommentar = "Hansker ikke byttet", SesjonType = SesjonType.Beskyttelsesutstyr },
                            new PredefinertKommentar { Kommentar = "Dårlig teknikk hånddesinfeksjon", SesjonType = SesjonType.Beskyttelsesutstyr }
                        }
                    },
                    new Domene.Sted.Institusjon()
                    {
                        HERId = "93917",
                        Region = _context.Region.First(),
                        Navn = "Lillehammer sykehus",
                        Forkortelse = "LS",
                        Institusjontype = institusjontyper.First(),
                        Avdelinger = new List<Domene.Sted.Avdeling>()
                        {
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Akutt",
                                Roller = new List<Domene.Observasjon.Rolle>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Medisin",
                                Roller = new List<Domene.Observasjon.Rolle>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Kirurgisk",
                                Roller = new List<Domene.Observasjon.Rolle>()
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            }
                        },
                        PredefinerteKommmentarer = new List<PredefinertKommentar>()
                        {
                            new PredefinertKommentar { Kommentar = "Hansker i stedet for håndhygiene", SesjonType = SesjonType.Beskyttelsesutstyr },
                            new PredefinertKommentar { Kommentar = "Hansker ikke byttet", SesjonType = SesjonType.Beskyttelsesutstyr },
                            new PredefinertKommentar { Kommentar = "Dårlig teknikk hånddesinfeksjon", SesjonType = SesjonType.Beskyttelsesutstyr }
                        }
                    }
                };
            _context.Institusjon.AddRange(institusjoner);
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

        private List<BeskyttelsesutstyrType> LagBeskyttelsesutstyrTyper()
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

        private List<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType> HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(BeskyttelsesutstyrsettingType settingType,
                List<BeskyttelsesutstyrType> utstyrstyper, params string[] defaultKoder)
        {
            var settingutstyrkoblinger = utstyrstyper.Select(u =>
                new BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType()
                {
                    BeskyttelsesutstyrType = u,
                    ErDefaultIndikert = defaultKoder.Contains(u.Kode),
                    BeskyttelsesutstyrsettingType = settingType
                }).ToList();
            return settingutstyrkoblinger;
        }
    }
}
