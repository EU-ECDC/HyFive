using System.Collections.Generic;
using System.Linq;
using HyFive.DataAccess;
using HyFive.Domain.Bruker;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Session;
using HyFive.Domain.Place;
using HyFive.Modeller.V1.Konstanter;

namespace HyFive.Tjenester.Tests
{
    public class Seed
    {
        private readonly HandHygieneContext _context;

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

        public Seed(HandHygieneContext context)
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
            var regioner = new List<Domain.Place.Region>
                {
                    new Domain.Place.Region {Code = "HELSE_SOR_OST", Name = "Helse Sør-Øst"},
                    new Domain.Place.Region {Code = "HELSE_VEST", Name = "Helse Vest"},
                    new Domain.Place.Region {Code = "HELSE_MIDT_NORGE", Name = "Helse Midt-Norge"},
                    new Domain.Place.Region {Code = "HELSE_NORD", Name = "Helse Nord"},
                    new Domain.Place.Region {Code = "AGDER", Name = "Agder"},
                    new Domain.Place.Region {Code = "INNLANDET", Name = "Innlandet"},
                    new Domain.Place.Region {Code = "MORE_OG_ROMSDAL", Name = "Møre og Romsdal"},
                    new Domain.Place.Region {Code = "NORDLAND", Name = "Nordland"},
                    new Domain.Place.Region {Code = "OSLO", Name = "Oslo"},
                    new Domain.Place.Region {Code = "ROGALAND", Name = "Rogaland"},
                    new Domain.Place.Region {Code = "VEST_OG_TELEMARK", Name = "Vestfold og Telemark"},
                    new Domain.Place.Region {Code = "TROMS_OG_FINNMARK", Name = "Troms og Finnmark"},
                    new Domain.Place.Region {Code = "TRONDELAG", Name = "Trøndelag"},
                    new Domain.Place.Region {Code = "VESTLAND", Name = "Vestland"},
                    new Domain.Place.Region {Code = "VIKEN", Name = "Viken"},
                };

            _context.Region.AddRange(regioner);
            _context.SaveChanges();
        }

        private void SeedHandhygieneEtterHanskebrukType()
        {
            var handhygieneEtterHanskebrukTyper = new List<PostGloveHandHygiene>
                {
                    new PostGloveHandHygiene {Code = "IKKE_INDIKERT", Name = "Ikke Indikert"},
                    new PostGloveHandHygiene {Code = "NEI", Name = "Nei"},
                    new PostGloveHandHygiene {Code = "JA", Name = "Ja"},
                };

            _context.PostGloveHandHygiene.AddRange(handhygieneEtterHanskebrukTyper);
            _context.SaveChanges();
        }

        private void SeedHanskeUtenIndikasjonTyper()
        {
            var hanskerUtenIndikasjonTyper = new List<GeneralPurposeGloveType>
                {
                    new GeneralPurposeGloveType {Code = "ANNET", Name = "Annet"},
                    new GeneralPurposeGloveType {Code = "MAT", Name = "Mat"},
                    new GeneralPurposeGloveType {Code = "STELL_UTEN_KROPPVAESKER", Name = "Stell uten kroppvæsker"},
                };

            _context.GeneralPurposeGloveType.AddRange(hanskerUtenIndikasjonTyper);
            _context.SaveChanges();
        }

        private void SeedOverforingstatusTyper()
        {
            var overforingsstatuser = new[]
            {
                    new TransmissionStatusType()
                    {
                        Code = OverforingstatusTypeKonstanter.OverfortTilKoordinator,
                        Name = "Overført til Koordinator"
                    },
                    new TransmissionStatusType()
                    {
                        Code = OverforingstatusTypeKonstanter.OverfortTilFhi,
                        Name = "Overført til FHI"
                    }
                };
            _context.TransmissionStatusType.AddRange(overforingsstatuser);
            _context.SaveChanges();
        }

        private void SeedHanskeMedIndikasjonTyper()
        {
            var hanskerMedIndikasjonTyper = new List<IndicatedGloveType>
                {
                    new IndicatedGloveType {Code = "ANNET", Name = "Annet"},
                    new IndicatedGloveType {Code = "SMITTE", Name = "Smitte"},
                    new IndicatedGloveType {Code = "KROPPVAESKER", Name = "Kroppvæsker"},
                };

            _context.IndicatedGloveType.AddRange(hanskerMedIndikasjonTyper);
            _context.SaveChanges();
        }

        private void SeedBrukere()
        {
            _context.Observer.Add(new Observator()
            {
                Etternavn = SeedObservatorEtternavn,
                Fornavn = SeedObservatorFornavn,
                HPRNummer = SeedObservatorHprNummer,
                Institusjon = _context.Institution.FirstOrDefault()
            });

            _context.Observer.Add(new Observator()
            {
                Etternavn = SeedObservatorEtternavn,
                Fornavn = SeedObservatorFornavn,
                HPRNummer = SeedObservatorHprNummer,
                Institusjon = _context.Institution.FirstOrDefault(i => i.HERId == "93917")
            });

            _context.Coordinator.Add(new Koordinator()
            {
                Etternavn = SeedKoordinatorEtternavn,
                Fornavn = SeedKoordinatorFornavn,
                HPRNummer = SeedKoordinatorHprNummer,
                Institusjon = _context.Institution.FirstOrDefault()
            });

            _context.Coordinator.Add(new Koordinator()
            {
                Etternavn = SeedKoordinatorEtternavn,
                Fornavn = SeedKoordinatorFornavn,
                HPRNummer = SeedKoordinatorHprNummer,
                Institusjon = _context.Institution.FirstOrDefault(i => i.HERId == "93917")
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
            var indikasjoner = new IndicationTypes[]
            {
                    new IndicationTypes {Code = "FOER_PASIENT", Name = "Før pasient", Number = "1"},
                    new IndicationTypes {Code = "ASEPTISKE_PROSEDYRER", Name = "Aseptisk", Number = "2"},
                    new IndicationTypes {Code = "KROPPSVESKE", Name = "Kroppsvæske", Number = "3"},
                    new IndicationTypes {Code = "ETTER_PASIENT", Name = "Etter pasient", Number = "4"}
            };
            _context.IndicationTypes.AddRange(indikasjoner);
            _context.SaveChanges();
        }

        private void SeedAktivitetTyper()
        {
            var aktivitettyper = new ActivityType[]
            {
                    new ActivityType {Code = "DESINFEKSJON", Name = "Desinfeksjon"},
                    new ActivityType {Code = "HANDVASK", Name = "Håndvask"},
                    new ActivityType {Code = "IKKE_UTFORT", Name = "Ikke utført"},
                    new ActivityType {Code = "IKKE_REGISTRERT", Name = "Ikke registrert"}
            };
            _context.ActivityType.AddRange(aktivitettyper);
            _context.SaveChanges();
        }

        private void SeedHandsmykkeTyper()
        {
            var handsmykketyper = new HandJewelryType[]
            {
                    new HandJewelryType {Code = "ALT_OK", Name = "Alt er ok", Order = 99, IsActive = true},
                    new HandJewelryType {Code = "RING", Name = "Ring", Order = 4 , IsActive = true},
                    new HandJewelryType {Code = "KLOKKE_ARMBAND", Name = "Klokke Armbånd", Order = 3, IsActive = true},
                    new HandJewelryType {Code = "LANG_NEGL", Name = "Lang negl", Order = 2, IsActive = true},
                    new HandJewelryType {Code = "KUNSTIG_NEGL_SHELLAC", Name = "Kunstig negl Shellack", Order = 1, IsActive = true},
                    new HandJewelryType {Code = "KORTERMET", Name = "Kortermet", Order = 5, IsActive = false},
                    new HandJewelryType {Code = "LANGERMET", Name = "Langermet", Order = 5, IsActive = true}
            };

            _context.HandJewelryType.AddRange(handsmykketyper);
            _context.SaveChanges();
        }

        private void SeedInstitusjonTyper()
        {
            var institusjontyper = new InstitutionType[]
            {
                    new InstitutionType {Code = "SYKEHUS", Name = "Sykehus"},
                    new InstitutionType {Code = "SYKEHJEM", Name = "Sykehjem"}
            };
            _context.InstitutionType.AddRange(institusjontyper);
            _context.SaveChanges();
        }


        private void SeedAvdelingTyper()
        {
            var avdelingstyper = new SectionType[]
            {
                    new SectionType {Code ="KIRURGI", Name = "Kirurgi"},
                    new SectionType {Code ="INDREMEDISIN", Name = "Indremedisin"},
                    new SectionType {Code ="FODSELSHJELP_OG_KVINNESYKDOMMER", Name = "Fødselshjelp og kvinnesykdommer"},
                    new SectionType {Code ="HUD_OG_VENERISKE_SYKDOMMER", Name = "Hud- og veneriske sykdommer"},
                    new SectionType {Code ="BARNESYKDOMMER", Name = "Barnesykdommer"},
                    new SectionType {Code ="NEVROLOGI", Name = "Nevrologi"},
                    new SectionType {Code ="ORE_NESE_HALS", Name = "Øre-nese-hals"},
                    new SectionType {Code ="OYESYKDOMMER", Name = "Øyesykdommer"},
                    new SectionType {Code ="ONKOLOGI", Name = "Onkologi"},
                    new SectionType {Code ="REVMATOLOGI", Name = "Revmatologi"},
                    new SectionType {Code ="FYSIKALSK_MEDISIN_REHABILITERING", Name = "Fysikalsk medisin/rehabilitering"},
                    new SectionType {Code ="OBSERVASJONSENHET_AKUTTMOTTAK", Name = "Observasjonsenhet / akuttmottak"},
                    new SectionType {Code ="KIRURGISK_INTENSIV_OVERVAKNING", Name = "Kirurgisk intensiv/overvåking"},
                    new SectionType {Code ="MEDISINSK_INTENSIV_OVERVAKNING", Name = "Medisinsk intensiv/overvåking"},
                    new SectionType {Code ="INTERMEDIARENHET", Name = "Intermediærenhet"},
                    new SectionType {Code ="SKJERMET_ENHET", Name = "Skjermet enhet (demens)"},
                    new SectionType {Code ="REHABILITERINGSENHET", Name = "Rehabiliteringsenhet"},
                    new SectionType {Code ="KORTTIDSAVDELING", Name = "Korttidsavdeling"},
                    new SectionType {Code ="LANGTIDSAVDELING", Name = "Langtidsavdeling"},
                    new SectionType {Code ="KOMBINERT_KORT_OG_LANGTIDSAVDELING", Name = "	Kombinert kort- og langtidsavdeling"}
            };
            _context.SectionType.AddRange(avdelingstyper);
            _context.SaveChanges();
        }

        private void SeedInstitusjoner()
        {
            var roller = new List<Domain.Observation.Role>()
                {
                    new Domain.Observation.Role("Sykepleier"),
                    new Domain.Observation.Role("Lege"),
                    new Domain.Observation.Role("Pleiepersonell"),
                    new Domain.Observation.Role("Jordmor"),
                    new Domain.Observation.Role("Fysioterapeut"),
                    new Domain.Observation.Role("Bioingeniør"),
                    new Domain.Observation.Role("Annet")
                };
            _context.Role.AddRange(roller);

            var institusjontyper = _context.InstitutionType;
            var avdelingtyper = _context.SectionType;

            var institusjoner = new[]
            {
                    new Domain.Place.Institution()
                    {
                        Region = _context.Region.First(),
                        HERId = "87711",
                        Name = "Oslo universitetssykehus HF",
                        Abbreviation = "OUS",
                        InstitutionType = institusjontyper.First(),
                        Departments = new List<Domain.Place.Avdeling>()
                        {
                            new Domain.Place.Avdeling
                            {
                                Navn = "Nevrokirurgisk",
                                Roller = new List<Domain.Observation.Role>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domain.Place.Avdeling
                            {
                                Navn = "Allergi og lungeseksjonen",
                                Roller = new List<Domain.Observation.Role>()
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            },
                            new Domain.Place.Avdeling
                            {
                                Navn = "Avdeling for mikrobiologi",
                                Roller = new List<Domain.Observation.Role>()
                                {
                                    roller[5],
                                    roller[6],
                                    roller[1],
                                },
                                Avdelingtype = avdelingtyper.Skip(2).First()
                            }
                        },
                        PredefinedComments = new List<PredefinedComments>()
                        {
                            new PredefinedComments { Comment = "Hansker i stedet for håndhygiene", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComments { Comment = "Hansker ikke byttet", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComments { Comment = "Dårlig teknikk hånddesinfeksjon", SessionType = SessionType.ProtectiveEquipment }
                        }
                    },
                    new Domain.Place.Institution()
                    {
                        HERId = "93917",
                        Region = _context.Region.First(),
                        Name = "Lillehammer sykehus",
                        Abbreviation = "LS",
                        InstitutionType = institusjontyper.First(),
                        Departments = new List<Domain.Place.Avdeling>()
                        {
                            new Domain.Place.Avdeling
                            {
                                Navn = "Akutt",
                                Roller = new List<Domain.Observation.Role>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domain.Place.Avdeling
                            {
                                Navn = "Medisin",
                                Roller = new List<Domain.Observation.Role>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domain.Place.Avdeling
                            {
                                Navn = "Kirurgisk",
                                Roller = new List<Domain.Observation.Role>()
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            }
                        },
                        PredefinedComments = new List<PredefinedComments>()
                        {
                            new PredefinedComments { Comment = "Hansker i stedet for håndhygiene", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComments { Comment = "Hansker ikke byttet", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComments { Comment = "Dårlig teknikk hånddesinfeksjon", SessionType = SessionType.ProtectiveEquipment }
                        }
                    }
                };
            _context.Institution.AddRange(institusjoner);
            _context.SaveChanges();
        }


        private void SeedBeskyttelsesutstyrTyper()
        {
            if (_context.ProtectiveEquipmentType.Any() == false)
            {
                var beskyttelsesutstyrtyper = LagBeskyttelsesutstyrTyper();
                _context.ProtectiveEquipmentType.AddRange(beskyttelsesutstyrtyper);
                _context.SaveChanges();
            }
        }

        private void SeedBeskyttelsesutstyrsettingTyper()
        {
            if (_context.ProtectiveEquipmentSettingType.Any() == false)
            {
                var beskyttelsesutstyrtyper = _context.ProtectiveEquipmentType.ToList();
                var settingtyper = LagSeedForBeskyttelsesutstyrsettingTyper(beskyttelsesutstyrtyper);
                _context.ProtectiveEquipmentSettingType.AddRange(settingtyper);
                _context.SaveChanges();
            }
        }

        private List<ProtectiveEquipmentType> LagBeskyttelsesutstyrTyper()
        {
            return new List<ProtectiveEquipmentType>
                {
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Hette,
                        Name = "Hette",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse,
                        Name = "Øyebeskyttelse",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Andedrettsvern,
                        Name = "Åndedrettsvern",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Ikke tilpasset/utført fit-sjekk"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Munnbind,
                        Name = "Munnbind",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Løst festet rundt nese/munn"},
                            new MisuseType {Name = "Ikke festet over nese"},
                            new MisuseType {Name = "Ikke trukket under hake"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Smittefrakk,
                        Name = "Smittefrakk",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil bruk ved påtagelse"},
                            new MisuseType {Name = "Ikke lukket skikkelig"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    }
                    ,
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Stellefrakk,
                        Name = "Stellefrakk",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil bruk ved påtagelse"},
                            new MisuseType {Name = "Ikke lukket skikkelig"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Plastforkle,
                        Name = "Plastforkle",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil bruk ved påtagelse"},
                            new MisuseType {Name = "Ikke lukket skikkelig"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = BeskyttelsesutstyrTypeKonstanter.Hansker,
                        Name = "Hansker",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"},
                            new MisuseType {Name = "Ikke festet over mansjett"}
                        }
                    }
                };
        }

        public List<ProtectiveEquipmentSettingType> LagSeedForBeskyttelsesutstyrsettingTyper(List<ProtectiveEquipmentType> typer)
        {
            var settingtyper = new List<ProtectiveEquipmentSettingType>
                {
                    new ProtectiveEquipmentSettingType
                    {
                        Code = BeskyttelsesutstyrsettingTypeKonstanter.Luftsmitte,
                        Name = "Luftsmitte",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = BeskyttelsesutstyrsettingTypeKonstanter.Drapesmitte,
                        Name = "Dråpesmitte",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte,
                        Name = "Kontaktsmitte",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = BeskyttelsesutstyrsettingTypeKonstanter.BasaleSmittevernrutiner,
                        Name = "Basale smittevernrutiner",
                    }
                };

            foreach (var settingtype in settingtyper)
            {
                if (settingtype.Code == BeskyttelsesutstyrsettingTypeKonstanter.Luftsmitte)
                {
                    settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                            BeskyttelsesutstyrTypeKonstanter.Andedrettsvern,
                            BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse,
                            BeskyttelsesutstyrTypeKonstanter.Hansker,
                            BeskyttelsesutstyrTypeKonstanter.Smittefrakk);
                }
                else if (settingtype.Code == BeskyttelsesutstyrsettingTypeKonstanter.Drapesmitte)
                {
                    settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                            BeskyttelsesutstyrTypeKonstanter.Hansker,
                            BeskyttelsesutstyrTypeKonstanter.Smittefrakk,
                            BeskyttelsesutstyrTypeKonstanter.Munnbind,
                            BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse);
                }
                else if (settingtype.Code == BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte)
                {
                    settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                            BeskyttelsesutstyrTypeKonstanter.Hansker,
                            BeskyttelsesutstyrTypeKonstanter.Smittefrakk);
                }
                else if (settingtype.Code == BeskyttelsesutstyrsettingTypeKonstanter.BasaleSmittevernrutiner)
                {
                    settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer);
                }
            }

            return settingtyper;
        }

        private List<PPEConfigurationType> HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(ProtectiveEquipmentSettingType settingType,
                List<ProtectiveEquipmentType> utstyrstyper, params string[] defaultKoder)
        {
            var settingutstyrkoblinger = utstyrstyper.Select(u =>
                new PPEConfigurationType()
                {
                    ProtectiveEquipmentType = u,
                    IsDefault = defaultKoder.Contains(u.Code),
                    ProtectiveEquipmentSettingType = settingType
                }).ToList();
            return settingutstyrkoblinger;
        }
    }
}
