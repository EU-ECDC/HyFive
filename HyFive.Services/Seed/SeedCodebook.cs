using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Session;
using HyFive.Models.V1.Constants;
using MediatR;

namespace HyFive.Services.Seed
{
    public class SeedCodebook
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
                SeedTransferStatusTypes();
                SeedActivityTypes();
                SeedIndications();
                SeedHandJewelryTypes();
                SeedProtectiveEquipmentTypes();
                SeedProtectiveEquipmentSettingTypes();
                SeedGlovesWithIndicationTypes();
                SeedGlovesWithoutIndicationTypes();
                SeedHandHygieneAfterGloveUseType();
                SeedFhiAdmin();

                return Task.FromResult(true);
            }

            private void SeedHandHygieneAfterGloveUseType()
            {
                if (_context.HandHygieneAfterGloveUseType.Any())
                    return;

                var handHygieneAfterGloveUseTypes = new List<HandHygieneAfterGloveUseType>
                {
                    new HandHygieneAfterGloveUseType {Code = "NOT_INDICATED", Name = "Not Indicated"},
                    new HandHygieneAfterGloveUseType {Code = "NO", Name = "No"},
                    new HandHygieneAfterGloveUseType {Code = "YES", Name = "Yes"},
                };

                _context.HandHygieneAfterGloveUseType.AddRange(handHygieneAfterGloveUseTypes);
                _context.SaveChanges();
            }

            private void SeedGlovesWithoutIndicationTypes()
            {
                if (_context.GloveWithoutIndicationType.Any())
                    return;

                var glovesWithoutIndicationTypes = new List<GloveWithoutIndicationType>
                {
                    new GloveWithoutIndicationType {Code = "OTHER", Name = "Other"},
                    new GloveWithoutIndicationType {Code = "FOOD", Name = "Food"},
                    new GloveWithoutIndicationType {Code = "CARE_WITHOUT_BODY_FLUIDS", Name = "Care without body fluids"},
                };

                _context.GloveWithoutIndicationType.AddRange(glovesWithoutIndicationTypes);
                _context.SaveChanges();
            }

            private void SeedTransferStatusTypes()
            {
                if (_context.TransferStatusType.Any())
                    return;

                var transferStatuses = new[]
                {
                    new TransferStatusType
                    {
                        Code = TransferStatusTypeConstants.TransferredToCoordinator,
                        Name = "Transferred To Coordinator"
                    },
                    new TransferStatusType
                    {
                        Code = TransferStatusTypeConstants.TransferredToAdmin,
                        Name = "Transferred To FHI"
                    }
                };
                _context.TransferStatusType.AddRange(transferStatuses);
                _context.SaveChanges();
            }

            private void SeedGlovesWithIndicationTypes()
            {
                if (_context.GloveWithIndicationType.Any())
                    return;

                var glovesWithIndicationTypes = new List<GloveWithIndicationType>
                {
                    new GloveWithIndicationType {Code = "OTHER", Name = "Other"},
                    new GloveWithIndicationType {Code = "INFECTION", Name = "iNFECTION"},
                    new GloveWithIndicationType {Code = "BODY_FLUIDS", Name = "Body Fluids"},
                };

                _context.GloveWithIndicationType.AddRange(glovesWithIndicationTypes);
                _context.SaveChanges();
            }

            private void SeedFhiAdmin()
            {
                if (_context.FhiAdmin.Any() == false)
                {
                    _context.FhiAdmin.Add(new FhiAdmin
                    {
                        FirstName = "Grønn",
                        LastName = "Vits",
                        IdentityPseudonym = "OCW6BpVN57vnbxBUE8WOOTM9FrkCaBixlD2y8FgYCag="
                    });
                }
                _context.SaveChanges();
            }

            private void SeedIndications()
            {
                if (_context.IndicationTypes.Any())
                    return;

                var indications = new[]
                {
                    new IndicationTypes {Code = "BEFORE_PATIENT", Name = "Before Patient", Number = "1"},
                    new IndicationTypes {Code = "ASEPTIC_PROCEDURES", Name = "Aseptic", Number = "2"},
                    new IndicationTypes {Code = "BODILY_FLUID", Name = "Bodily Fluid", Number = "3"},
                    new IndicationTypes {Code = "AFTER_PATIENT", Name = "After patient", Number = "4"}
                };
                _context.IndicationTypes.AddRange(indications);
                _context.SaveChanges();
            }

            private void SeedActivityTypes()
            {
                if (_context.ActivityType.Any())
                    return;

                var activityTypes = new[]
                {
                    new ActivityType {Code = "DISINFECTION", Name = "Disinfection"},
                    new ActivityType {Code = "HAND_WASH", Name = "Hand wash"},
                    new ActivityType {Code = "NOT_PERFORMED", Name = "Not performed"},
                    new ActivityType {Code = "NOT_REGISTERED", Name = "Not registered"}
                };
                _context.ActivityType.AddRange(activityTypes);
                _context.SaveChanges();
            }

            private void SeedHandJewelryTypes()
            {
                if (_context.HandJewelryType.Any())
                    return;

                var handJewelryTypes = new[]
                {
                    new HandJewelryType {Code = "ALL_OK", Name = "All is ok", Order = 99, IsActive = true},
                    new HandJewelryType {Code = "RING", Name = "Ring", Order = 4, IsActive = true},
                    new HandJewelryType {Code = "WATCH_BRACELET", Name = "Watch Bracelet", Order = 3, IsActive = true},
                    new HandJewelryType {Code = "LONG_NAIL", Name = "Long nail", Order = 2, IsActive = true},
                    new HandJewelryType {Code = "ARTIFICIAL_NAIL_SHELLAC", Name = "Artificial nail Shellac", Order = 1, IsActive = true},
                    new HandJewelryType {Code = "SHORT_SLEEVES", Name = "Short sleeves", Order = 5, IsActive = false},
                    new HandJewelryType {Code = "LONG_SLEEVES", Name = "Long sleeves", Order = 5, IsActive = true}
                };

                _context.HandJewelryType.AddRange(handJewelryTypes);
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
                    var settingTypes = CreateSeedForProtectiveEquipmentSettingTypes(protectiveEquipmentTypes);
                    _context.ProtectiveEquipmentSettingType.AddRange(settingTypes);
                    _context.SaveChanges();
                }
            }

            private static IEnumerable<ProtectiveEquipmentType> CreateProtectiveEquipmentTypes()
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
                        Code = ProtectiveEquipmentTypeConstants.IsolationGown,
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

            public List<ProtectiveEquipmentSettingType> CreateSeedForProtectiveEquipmentSettingTypes(List<ProtectiveEquipmentType> types)
            {
                var settingTypes = new List<ProtectiveEquipmentSettingType>
                {
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingTypeConstants.AirborneTransmission,
                        Name = "Airborne transmission",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingTypeConstants.DropletTransmission,
                        Name = "Droplet transmission",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingTypeConstants.ContactTransmission,
                        Name = "Contact transmission",
                    },
                    new ProtectiveEquipmentSettingType
                    {
                        Code = ProtectiveEquipmentSettingTypeConstants.BasicIsolationRoutines,
                        Name = "Basic isolation routines",
                    }
                };

                foreach (var settingType in settingTypes)
                {
                    if (settingType.Code == ProtectiveEquipmentSettingTypeConstants.AirborneTransmission)
                    {
                        settingType.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes = GetProtectionEquipmentSettingTypeProtectionEquipmentTypes(settingType, types,
                                                ProtectiveEquipmentTypeConstants.RespiratoryProtection, 
                                                ProtectiveEquipmentTypeConstants.EyeProtection,
                                                ProtectiveEquipmentTypeConstants.Gloves, 
                                                ProtectiveEquipmentTypeConstants.IsolationGown);
                    }
                    else if (settingType.Code == ProtectiveEquipmentSettingTypeConstants.DropletTransmission)
                    {
                        settingType.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes = GetProtectionEquipmentSettingTypeProtectionEquipmentTypes(settingType, types,
                                                ProtectiveEquipmentTypeConstants.Gloves, 
                                                ProtectiveEquipmentTypeConstants.IsolationGown,
                                                ProtectiveEquipmentTypeConstants.FaceMask, 
                                                ProtectiveEquipmentTypeConstants.EyeProtection);
                    }
                    else if (settingType.Code == ProtectiveEquipmentSettingTypeConstants.ContactTransmission)
                    {
                        settingType.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes = GetProtectionEquipmentSettingTypeProtectionEquipmentTypes(settingType, types,
                                                ProtectiveEquipmentTypeConstants.Gloves, 
                                                ProtectiveEquipmentTypeConstants.IsolationGown);
                    }
                    else if (settingType.Code == ProtectiveEquipmentSettingTypeConstants.BasicIsolationRoutines)
                    {
                        settingType.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes = GetProtectionEquipmentSettingTypeProtectionEquipmentTypes(settingType, types);
                    }
                }

                return settingTypes;
            }

            private static List<ProtectiveEquipmentSettingTypeProtectiveEquipmentType> GetProtectionEquipmentSettingTypeProtectionEquipmentTypes(ProtectiveEquipmentSettingType settingType,
                    IEnumerable<ProtectiveEquipmentType> equipmentTypes, params string[] defaultCodes)
            {
                var settingEquipmentConnections = equipmentTypes.Select(u =>
                    new ProtectiveEquipmentSettingTypeProtectiveEquipmentType
                    {
                        ProtectiveEquipmentType = u,
                        IsDefault = defaultCodes.Contains(u.Code),
                        ProtectiveEquipmentSettingType = settingType
                    }).ToList();

                return settingEquipmentConnections;
            }
        }
    }
}