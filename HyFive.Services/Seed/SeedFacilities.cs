using HyFive.DataAccess;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domain.Observation;
using HyFive.Domain.Place;
using HyFive.Domain.User;

namespace HyFive.Services.Seed
{
    public class SeedFacilities
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
                SeedFacilityTypes();
                SeedDepartmentTypes();
                SeedRoles();
                SeedInstitutions();
                SeedUsers();

                return Task.FromResult(true);
            }

            private const string ObserverIdentityPseudonym = "vJs8Xr2F58spTNEPHM/a07KdZtSBGLQN9EmHuBGLy/c=";
            private const string ObserverFirstName = "Virkelig";
            private const string ObserverLastName = "Kjeltring";

            private const string CoordinatorIdentityPseudonym = "PGzVzvP2JvlXV++OJSJAQG5d99BH8QsikmxpdIAKSZk=";
            private const string CoordinatorFirstName = "Kvart";
            private const string CoordinatorLastName = "Grevling";

            private const string HerIdOus = "87711";
            private const string HerIdLillehammer = "93917";

            private void SeedUsers()
            {
                if (_context.Observer.Any() == false)
                {
                    _context.Observer.Add(new Observer
                    {
                        LastName = ObserverLastName,
                        FirstName = ObserverFirstName,
                        IdentityPseudonym = ObserverIdentityPseudonym,
                        Facility = _context.Facility.FirstOrDefault(i => i.HERId == HerIdOus)
                    });
                    _context.Observer.Add(new Observer
                    {
                        LastName = ObserverLastName,
                        FirstName = ObserverFirstName,
                        IdentityPseudonym = ObserverIdentityPseudonym,
                        Facility = _context.Facility.FirstOrDefault(i => i.HERId == HerIdLillehammer)
                    });
                }

                if (_context.Coordinator.Any() == false)
                {
                    _context.Coordinator.Add(new Coordinator
                    {
                        LastName = CoordinatorLastName,
                        FirstName = CoordinatorFirstName,
                        IdentityPseudonym = CoordinatorIdentityPseudonym,
                        Facility = _context.Facility.FirstOrDefault()
                    });
                    _context.Coordinator.Add(new Coordinator
                    {
                        LastName = CoordinatorLastName,
                        FirstName = CoordinatorFirstName,
                        IdentityPseudonym = CoordinatorIdentityPseudonym,
                        Facility = _context.Facility.FirstOrDefault(i => i.HERId == HerIdLillehammer)
                    });
                }

                _context.SaveChanges();
            }

            private void SeedInstitutions()
            {
                if (_context.Facility.Any())
                    return;

                var roles = _context.Role.ToList();

                var InstitutionTypes = _context.FacilityType.ToList();
                var departmentTypes = _context.DepartmentType.ToList();

                var facilities = new[]
                {
                    new Domain.Place.Facility
                    {
                        Region = _context.Region.First(),
                        HERId = HerIdOus,
                        Name = "Oslo University Hospital HF",
                        Abbreviation = "OUS",
                        FacilityType = InstitutionTypes.First(),
                        Departments = new List<Domain.Place.Department>
                        {
                            new Domain.Place.Department
                            {
                                Name = "Neurosurgical ",
                                Roles = new List<Role>(roles),
                                DepartmentType = departmentTypes.First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Allergy and Pulmonary Section",
                                Roles = new List<Role>
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
                                Roles = new List<Role>
                                {
                                    roles[5],
                                    roles[6],
                                    roles[1],
                                },
                                DepartmentType = departmentTypes.Skip(2).First()
                            }
                        },
                        PredefinedComment = new List<PredefinedComment>
                        {
                            new PredefinedComment { Comment = "Gloves instead of hand hygiene", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComment { Comment = "Gloves not changed", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComment { Comment = "Poor technique hand disinfection", SessionType = SessionType.ProtectiveEquipment }
                        }
                    },
                    new Domain.Place.Facility
                    {
                        HERId = HerIdLillehammer,
                        Region = _context.Region.First(),
                        Name = "Lillehammer sykehus",
                        Abbreviation = "LS",
                        FacilityType = InstitutionTypes.First(),
                        Departments = new List<Domain.Place.Department>
                        {
                            new Domain.Place.Department
                            {
                                Name = "Akutt",
                                Roles = new List<Role>(roles),
                                DepartmentType = departmentTypes.First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Medisin",
                                Roles = new List<Role>(roles),
                                DepartmentType = departmentTypes.First()
                            },
                            new Domain.Place.Department
                            {
                                Name = "Kirurgisk",
                                Roles = new List<Role>
                                {
                                    roles[0],
                                    roles[1],
                                    roles[2],
                                    roles[3],
                                },
                                DepartmentType = departmentTypes.Skip(1).First()
                            }
                        },
                        PredefinedComment = new List<PredefinedComment>
                        {
                            new PredefinedComment { Comment = "Gloves instead of hand hygiene", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComment { Comment = "Gloves not changed", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComment { Comment = "Poor technique in hand disinfection", SessionType = SessionType.ProtectiveEquipment }
                        }
                    }
                };

                _context.Facility.AddRange(facilities);
                _context.SaveChanges();
            }

            private void SeedFacilityTypes()
            {
                if (_context.FacilityType.Any())
                    return;

                var facilityTypes = new[]
                {
                    new FacilityType {Code = "SYKEHUS", Name = "Sykehus"},
                    new FacilityType {Code = "SYKEHJEM", Name = "Sykehjem"}
                };

                _context.FacilityType.AddRange(facilityTypes);
                _context.SaveChanges();
            }

            private void SeedDepartmentTypes()
            {
                if (_context.DepartmentType.Any())
                    return;

                var departmentTypes = new[]
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

                _context.DepartmentType.AddRange(departmentTypes);
                _context.SaveChanges();
            }

            private void SeedRoles()
            {
                if (_context.Role.Any())
                    return;

                var roles = new List<Role>
                {
                    new Role("Nurse"),
                    new Role("Doctor"),
                    new Role("Nursing staff"),
                    new Role("Midwife"),
                    new Role("Physiotherapist"),
                    new Role("Biomedical Engineer"),
                    new Role("Other")
                };

                _context.Role.AddRange(roles);
                _context.SaveChanges();
            }
        }
    }
}
