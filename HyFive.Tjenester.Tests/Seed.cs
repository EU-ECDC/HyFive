using HyFive.DataAccess;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Place;
using HyFive.Domain.Session;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using System.Collections.Generic;
using System.Linq;

namespace HyFive.Services.Tests
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
            SeedRegions();
            SeedInstitutionTypes();
            SeedTransmissionStatusTypes();
            SeedDepartmentTypes();
            SeedInstitutions();
            SeedActivityTypes();
            SeedIndication();
            SeedHandJewelryTypes();
            SeedProtectiveEquipmentTypes();
            SeedProtectiveEquipmentSettingTypes();
            SeedUsers();
            SeedGloveWithIndicationTypes();
            SeedGloveWithoutIndicationTypes();
            SeedPostGloveHandHygieneTypes();
        }

        private void SeedRegions()
        {
            var regions = new List<Domain.Place.Region>
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

            _context.Region.AddRange(regions);
            _context.SaveChanges();
        }

        private void SeedPostGloveHandHygieneTypes()
        {
            var handHygieneAfterGloveUseTypes = new List<PostGloveHandHygiene>
                {
                    new PostGloveHandHygiene {Code = "IKKE_INDIKERT", Name = "Ikke Indikert"},
                    new PostGloveHandHygiene {Code = "NEI", Name = "Nei"},
                    new PostGloveHandHygiene {Code = "JA", Name = "Ja"},
                };

            _context.PostGloveHandHygiene.AddRange(handHygieneAfterGloveUseTypes);
            _context.SaveChanges();
        }

        private void SeedGloveWithoutIndicationTypes()
        {
            var glovesWithoutIndicationTypes = new List<GeneralPurposeGloveType>
                {
                    new GeneralPurposeGloveType {Code = "ANNET", Name = "Annet"},
                    new GeneralPurposeGloveType {Code = "MAT", Name = "Mat"},
                    new GeneralPurposeGloveType {Code = "STELL_UTEN_KROPPVAESKER", Name = "Stell uten kroppvæsker"},
                };

            _context.GeneralPurposeGloveType.AddRange(glovesWithoutIndicationTypes);
            _context.SaveChanges();
        }

        private void SeedTransmissionStatusTypes()
        {
            var transmissionStatuses = new[]
            {
                    new TransmissionStatusType()
                    {
                        Code = TransferStatusTypeConstants.TransferredToCoordinator,
                        Name = "Overført til Coordinator"
                    },
                    new TransmissionStatusType()
                    {
                        Code = TransferStatusTypeConstants.TransferredToFhi,
                        Name = "Overført til FHI"
                    }
                };
            _context.TransmissionStatusType.AddRange(transmissionStatuses);
            _context.SaveChanges();
        }

        private void SeedGloveWithIndicationTypes()
        {
            var gloveWithIndicationTypes = new List<IndicatedGloveType>
                {
                    new IndicatedGloveType {Code = "ANNET", Name = "Annet"},
                    new IndicatedGloveType {Code = "SMITTE", Name = "Smitte"},
                    new IndicatedGloveType {Code = "KROPPVAESKER", Name = "Kroppvæsker"},
                };

            _context.IndicatedGloveType.AddRange(gloveWithIndicationTypes);
            _context.SaveChanges();
        }

        private void SeedUsers()
        {
            _context.Observer.Add(new Observer()
            {
                LastName = SeedObservatorEtternavn,
                FirstName = SeedObservatorFornavn,
                HPRNumber = SeedObservatorHprNummer,
                Institution = _context.Institution.FirstOrDefault()
            });

            _context.Observer.Add(new Observer()
            {
                LastName = SeedObservatorEtternavn,
                FirstName = SeedObservatorFornavn,
                HPRNumber = SeedObservatorHprNummer,
                Institution = _context.Institution.FirstOrDefault(i => i.HERId == "93917")
            });

            _context.Coordinator.Add(new Coordinator()
            {
                LastName = SeedKoordinatorEtternavn,
                FirstName = SeedKoordinatorFornavn,
                HPRNumber = SeedKoordinatorHprNummer,
                Institution = _context.Institution.FirstOrDefault()
            });

            _context.Coordinator.Add(new Coordinator()
            {
                LastName = SeedKoordinatorEtternavn,
                FirstName = SeedKoordinatorFornavn,
                HPRNumber = SeedKoordinatorHprNummer,
                Institution = _context.Institution.FirstOrDefault(i => i.HERId == "93917")
            });

            _context.FhiAdmin.Add(new FhiAdmin()
            {
                FirstName = SeedFhiAdminFornavn,
                LastName = SeedFhiAdminEtternavn,
                IdentityPseudonym = SeedFhiAdminIdentPseudonym
            });

            _context.SaveChanges();
        }

        private void SeedIndication()
        {
            var indications = new IndicationType[]
            {
                    new IndicationType {Code = "FOER_PASIENT", Name = "Før pasient", Number = "1"},
                    new IndicationType {Code = "ASEPTISKE_PROSEDYRER", Name = "Aseptisk", Number = "2"},
                    new IndicationType {Code = "KROPPSVESKE", Name = "Kroppsvæske", Number = "3"},
                    new IndicationType {Code = "ETTER_PASIENT", Name = "Etter pasient", Number = "4"}
            };
            _context.IndicationType.AddRange(indications);
            _context.SaveChanges();
        }

        private void SeedActivityTypes()
        {
            var activityTypes = new ActivityType[]
            {
                    new ActivityType {Code = "DESINFEKSJON", Name = "Desinfeksjon"},
                    new ActivityType {Code = "HANDVASK", Name = "Håndvask"},
                    new ActivityType {Code = "IKKE_UTFORT", Name = "Ikke utført"},
                    new ActivityType {Code = "IKKE_REGISTRERT", Name = "Ikke registrert"}
            };
            _context.ActivityType.AddRange(activityTypes);
            _context.SaveChanges();
        }

        private void SeedHandJewelryTypes()
        {
            var handJewelryTypes = new HandJewelryType[]
            {
                    new HandJewelryType {Code = "ALT_OK", Name = "Alt er ok", Order = 99, IsActive = true},
                    new HandJewelryType {Code = "RING", Name = "Ring", Order = 4 , IsActive = true},
                    new HandJewelryType {Code = "KLOKKE_ARMBAND", Name = "Klokke Armbånd", Order = 3, IsActive = true},
                    new HandJewelryType {Code = "LANG_NEGL", Name = "Lang negl", Order = 2, IsActive = true},
                    new HandJewelryType {Code = "KUNSTIG_NEGL_SHELLAC", Name = "Kunstig negl Shellack", Order = 1, IsActive = true},
                    new HandJewelryType {Code = "KORTERMET", Name = "Kortermet", Order = 5, IsActive = false},
                    new HandJewelryType {Code = "LANGERMET", Name = "Langermet", Order = 5, IsActive = true}
            };

            _context.HandJewelryType.AddRange(handJewelryTypes);
            _context.SaveChanges();
        }

        private void SeedInstitutionTypes()
        {
            var institutionTypes = new InstitutionType[]
            {
                    new InstitutionType {Code = "SYKEHUS", Name = "Sykehus"},
                    new InstitutionType {Code = "SYKEHJEM", Name = "Sykehjem"}
            };
            _context.InstitutionType.AddRange(institutionTypes);
            _context.SaveChanges();
        }


        private void SeedDepartmentTypes()
        {
            var departmentTypes = new DepartmentType[]
            {
                    new DepartmentType {Code ="KIRURGI", Name = "Kirurgi"},
                    new DepartmentType {Code ="INDREMEDISIN", Name = "Indremedisin"},
                    new DepartmentType {Code ="FODSELSHJELP_OG_KVINNESYKDOMMER", Name = "Fødselshjelp og kvinnesykdommer"},
                    new DepartmentType {Code ="HUD_OG_VENERISKE_SYKDOMMER", Name = "Hud- og veneriske sykdommer"},
                    new DepartmentType {Code ="BARNESYKDOMMER", Name = "Barnesykdommer"},
                    new DepartmentType {Code ="NEVROLOGI", Name = "Nevrologi"},
                    new DepartmentType {Code ="ORE_NESE_HALS", Name = "Øre-nese-hals"},
                    new DepartmentType {Code ="OYESYKDOMMER", Name = "Øyesykdommer"},
                    new DepartmentType {Code ="ONKOLOGI", Name = "Onkologi"},
                    new DepartmentType {Code ="REVMATOLOGI", Name = "Revmatologi"},
                    new DepartmentType {Code ="FYSIKALSK_MEDISIN_REHABILITERING", Name = "Fysikalsk medisin/rehabilitering"},
                    new DepartmentType {Code ="OBSERVASJONSENHET_AKUTTMOTTAK", Name = "Observasjonsenhet / akuttmottak"},
                    new DepartmentType {Code ="KIRURGISK_INTENSIV_OVERVAKNING", Name = "Kirurgisk intensiv/overvåking"},
                    new DepartmentType {Code ="MEDISINSK_INTENSIV_OVERVAKNING", Name = "Medisinsk intensiv/overvåking"},
                    new DepartmentType {Code ="INTERMEDIARENHET", Name = "Intermediærenhet"},
                    new DepartmentType {Code ="SKJERMET_ENHET", Name = "Skjermet enhet (demens)"},
                    new DepartmentType {Code ="REHABILITERINGSENHET", Name = "Rehabiliteringsenhet"},
                    new DepartmentType {Code ="KORTTIDSAVDELING", Name = "Korttidsavdeling"},
                    new DepartmentType {Code ="LANGTIDSAVDELING", Name = "Langtidsavdeling"},
                    new DepartmentType {Code ="KOMBINERT_KORT_OG_LANGTIDSAVDELING", Name = "	Kombinert kort- og langtidsavdeling"}
            };
            _context.SectionType.AddRange(departmentTypes);
            _context.SaveChanges();
        }

        private void SeedInstitutions()
        {
            var roles = new List<Domain.Observation.Role>()
                {
                    new Domain.Observation.Role("Sykepleier"),
                    new Domain.Observation.Role("Lege"),
                    new Domain.Observation.Role("Pleiepersonell"),
                    new Domain.Observation.Role("Jordmor"),
                    new Domain.Observation.Role("Fysioterapeut"),
                    new Domain.Observation.Role("Bioingeniør"),
                    new Domain.Observation.Role("Annet")
                };
            _context.Role.AddRange(roles);

            var institutionTypes = _context.InstitutionType;
            var departmentTypes = _context.SectionType;

            var institutions = new[]
            {
                    new Domain.Place.Institution()
                    {
                        Region = _context.Region.First(),
                        HERId = "87711",
                        Name = "Oslo universitetssykehus HF",
                        Abbreviation = "OUS",
                        InstitutionType = institutionTypes.First(),
                        Departments = new List<Domain.Place.Department>()
                        {
                            new Domain.Place.Department
                            {
                                Name = "Nevrokirurgisk",
                                Roles = new List<Domain.Observation.Role>(roles),
                                DepartmentType = departmentTypes.First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Allergi og lungeseksjonen",
                                Roles = new List<Domain.Observation.Role>()
                                {
                                    roles[0],
                                    roles[1],
                                    roles[2],
                                    roles[3],
                                },
                                DepartmentType = departmentTypes.Skip(1).First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Department for mikrobiologi",
                                Roles = new List<Domain.Observation.Role>()
                                {
                                    roles[5],
                                    roles[6],
                                    roles[1],
                                },
                                DepartmentType = departmentTypes.Skip(2).First()
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
                        InstitutionType = institutionTypes.First(),
                        Departments = new List<Domain.Place.Department>()
                        {
                            new Domain.Place.Department
                            {
                                Name = "Akutt",
                                Roles = new List<Domain.Observation.Role>(roles),
                                DepartmentType = departmentTypes.First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Medisin",
                                Roles = new List<Domain.Observation.Role>(roles),
                                DepartmentType = departmentTypes.First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Kirurgisk",
                                Roles = new List<Domain.Observation.Role>()
                                {
                                    roles[0],
                                    roles[1],
                                    roles[2],
                                    roles[3],
                                },
                                DepartmentType = departmentTypes.Skip(1).First()
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
            _context.Institution.AddRange(institutions);
            _context.SaveChanges();
        }


        private void SeedProtectiveEquipmentTypes()
        {
            if (_context.ProtectiveEquipmentType.Any() == false)
            {
                var protectiveEquipmentTypes = CreateProtectiveEquipmentTypes();
                _context.ProtectiveEquipmentType.AddRange(protectiveEquipmentTypes);
                _context.SaveChanges();
            }
        }

        private void SeedProtectiveEquipmentSettingTypes()
        {
            if (_context.ProtectiveEquipmentSettingType.Any() == false)
            {
                var protectiveEquipmentTypes = _context.ProtectiveEquipmentType.ToList();
                var settingtypes = CreateSeedForProtectiveEquipmentSettingTypes(protectiveEquipmentTypes);
                _context.ProtectiveEquipmentSettingType.AddRange(settingtypes);
                _context.SaveChanges();
            }
        }

        private List<ProtectiveEquipmentType> CreateProtectiveEquipmentTypes()
        {
            return new List<ProtectiveEquipmentType>
                {
                    new ProtectiveEquipmentType
                    {
                        Code = ProtectiveEquipmentTypeConstants.Hood,
                        Name = "Hette",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = ProtectiveEquipmentTypeConstants.EyeProtection,
                        Name = "Øyebeskyttelse",
                        MisuseTypes = new List<MisuseType>
                        {
                            new MisuseType {Name = "Feil teknikk ved påtagelse"},
                            new MisuseType {Name = "Feil teknikk ved avtagelse"}
                        }
                    },
                    new ProtectiveEquipmentType
                    {
                        Code = ProtectiveEquipmentTypeConstants.RespiratoryProtection,
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
                        Code = ProtectiveEquipmentTypeConstants.FaceMask,
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
                        Code = ProtectiveEquipmentTypeConstants.InfectionGown,
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
                        Code = ProtectiveEquipmentTypeConstants.CareGown,
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
                        Code = ProtectiveEquipmentTypeConstants.PlasticApron,
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
                        Code = ProtectiveEquipmentTypeConstants.Gloves,
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

        public List<Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType> CreateSeedForProtectiveEquipmentSettingTypes(List<ProtectiveEquipmentType> typer)
        {
            var settingTypes = new List<Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>
                {
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Code = Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.AirborneTransmission,
                        Name = "Luftsmitte",
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Code = Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.DropletTransmission,
                        Name = "Dråpesmitte",
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Code = Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.ContactTransmission,
                        Name = "Kontaktsmitte",
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Code = Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.BasicInfectionControlRoutines,
                        Name = "Basale smittevernrutiner",
                    }
                };

            foreach (var settingType in settingTypes)
            {
                if (settingType.Code == Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.AirborneTransmission)
                {
                    settingType.PPEConfigurationTypes = GetProtectiveEquipmentSettingTypeEquipmentTypes(settingType, typer,
                            ProtectiveEquipmentTypeConstants.RespiratoryProtection,
                            ProtectiveEquipmentTypeConstants.EyeProtection,
                            ProtectiveEquipmentTypeConstants.Gloves,
                            ProtectiveEquipmentTypeConstants.InfectionGown);
                }
                else if (settingType.Code == Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.DropletTransmission)
                {
                    settingType.PPEConfigurationTypes = GetProtectiveEquipmentSettingTypeEquipmentTypes(settingType, typer,
                            ProtectiveEquipmentTypeConstants.Gloves,
                            ProtectiveEquipmentTypeConstants.InfectionGown,
                            ProtectiveEquipmentTypeConstants.FaceMask,
                            ProtectiveEquipmentTypeConstants.EyeProtection);
                }
                else if (settingType.Code == Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.ContactTransmission)
                {
                    settingType.PPEConfigurationTypes = GetProtectiveEquipmentSettingTypeEquipmentTypes(settingType, typer,
                            ProtectiveEquipmentTypeConstants.Gloves,
                            ProtectiveEquipmentTypeConstants.InfectionGown);
                }
                else if (settingType.Code == Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.BasicInfectionControlRoutines)
                {
                    settingType.PPEConfigurationTypes = GetProtectiveEquipmentSettingTypeEquipmentTypes(settingType, typer);
                }
            }

            return settingTypes;
        }

        private List<PPEConfigurationType> GetProtectiveEquipmentSettingTypeEquipmentTypes(Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType settingType,
                List<ProtectiveEquipmentType> equipmentTypes, params string[] defaultCodes)
        {
            var settingEquipmentConnections = equipmentTypes.Select(u =>
                new PPEConfigurationType()
                {
                    ProtectiveEquipmentType = u,
                    IsDefault = defaultCodes.Contains(u.Code),
                    ProtectiveEquipmentSettingType = settingType
                }).ToList();
            return settingEquipmentConnections;
        }
    }
}
