using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observation;
using HyFive.Domene.Observation.ProtectiveEquipment;
using HyFive.Domene.Observation.Gloves;
using HyFive.Domene.Session;
using HyFive.Models.V1.Constants;
using MediatR;

namespace HyFive.Services.Seed
{
    public class SeedKodeverk
    {
        public class Command : IRequest<bool>
        {
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
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
                if (_context.PostGloveHandHygiene.Any())
                    return;

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
                if (_context.GeneralPurposeGloveType.Any())
                    return;

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
                if (_context.TransmissionStatusType.Any())
                    return;

                var overforingsstatuser = new[]
                {
                    new TransmissionStatusType
                    {
                        Code = TransferStatusTypeConstants.TransferredToCoordinator,
                        Name = "Overført til Koordinator"
                    },
                    new TransmissionStatusType
                    {
                        Code = TransferStatusTypeConstants.TransferredToFhi,
                        Name = "Overført til FHI"
                    }
                };
                _context.TransmissionStatusType.AddRange(overforingsstatuser);
                _context.SaveChanges();
            }

            private void SeedHanskeMedIndikasjonTyper()
            {
                if (_context.IndicatedGloveType.Any())
                    return;

                var hanskerMedIndikasjonTyper = new List<IndicatedGloveType>
                {
                    new IndicatedGloveType {Code = "ANNET", Name = "Annet"},
                    new IndicatedGloveType {Code = "SMITTE", Name = "Smitte"},
                    new IndicatedGloveType {Code = "KROPPVAESKER", Name = "Kroppvæsker"},
                };

                _context.IndicatedGloveType.AddRange(hanskerMedIndikasjonTyper);
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
                if (_context.IndicationTypes.Any())
                    return;

                var indikasjoner = new[]
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
                if (_context.ActivityType.Any())
                    return;

                var aktivitettyper = new[]
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
                if (_context.HandJewelryType.Any())
                    return;

                var handsmykketyper = new[]
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

            private static IEnumerable<ProtectiveEquipmentType> LagBeskyttelsesutstyrTyper()
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

            public List<ProtectiveEquipmentSettingType> LagSeedForBeskyttelsesutstyrsettingTyper(List<ProtectiveEquipmentType> typer)
            {
                var settingtyper = new List<ProtectiveEquipmentSettingType>
                {
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingType.AirborneTransmission,
                        Name = "Luftsmitte",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingType.DropletTransmission,
                        Name = "Dråpesmitte",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingType.ContactTransmission,
                        Name = "Kontaktsmitte",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingType.BasicInfectionControlRoutines,
                        Name = "Basale smittevernrutiner",
                    }
                };

                foreach (var settingtype in settingtyper)
                {
                    if (settingtype.Code == ProtectiveEquipmentSettingType.AirborneTransmission)
                    {
                        settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                                                ProtectiveEquipmentTypeConstants.RespiratoryProtection, 
                                                ProtectiveEquipmentTypeConstants.EyeProtection,
                                                ProtectiveEquipmentTypeConstants.Gloves, 
                                                ProtectiveEquipmentTypeConstants.InfectionGown);
                    }
                    else if (settingtype.Code == ProtectiveEquipmentSettingType.DropletTransmission)
                    {
                        settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                                                ProtectiveEquipmentTypeConstants.Gloves, 
                                                ProtectiveEquipmentTypeConstants.InfectionGown,
                                                ProtectiveEquipmentTypeConstants.FaceMask, 
                                                ProtectiveEquipmentTypeConstants.EyeProtection);
                    }
                    else if (settingtype.Code == ProtectiveEquipmentSettingType.ContactTransmission)
                    {
                        settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer,
                                                ProtectiveEquipmentTypeConstants.Gloves, 
                                                ProtectiveEquipmentTypeConstants.InfectionGown);
                    }
                    else if (settingtype.Code == ProtectiveEquipmentSettingType.BasicInfectionControlRoutines)
                    {
                        settingtype.PPEConfigurationTypes = HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(settingtype, typer);
                    }
                }

                return settingtyper;
            }

            private static List<PPEConfigurationType> HentBeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper(ProtectiveEquipmentSettingType settingType,
                    IEnumerable<ProtectiveEquipmentType> utstyrstyper, params string[] defaultKoder)
            {
                var settingutstyrkoblinger = utstyrstyper.Select(u =>
                    new PPEConfigurationType
                    {
                        ProtectiveEquipmentType = u,
                        IsDefault = defaultKoder.Contains(u.Code),
                        ProtectiveEquipmentSettingType = settingType
                    }).ToList();

                return settingutstyrkoblinger;
            }
        }
    }
}