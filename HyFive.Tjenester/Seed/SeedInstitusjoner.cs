using HyFive.DataAccess;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Observation;
using HyFive.Domene.Place;
using HyFive.Domene.Bruker;

namespace HyFive.Tjenester.Seed
{
    public class SeedInstitusjoner
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
                SeedInstitusjonTyper();
                SeedAvdelingTyper();
                SeedRoller();
                SeedInstitusjoner();
                SeedBrukere();

                return Task.FromResult(true);
            }

            private const string ObservatørIdentPseudonym = "vJs8Xr2F58spTNEPHM/a07KdZtSBGLQN9EmHuBGLy/c=";
            private const string ObservatørFornavn = "Virkelig";
            private const string ObservatørEtternavn = "Kjeltring";

            private const string KoordinatorIdentPseudonym = "PGzVzvP2JvlXV++OJSJAQG5d99BH8QsikmxpdIAKSZk=";
            private const string KoordinatorFornavn = "Kvart";
            private const string KoordinatorEtternavn = "Grevling";

            private const string HerIdOus = "87711";
            private const string HerIdLillehammer = "93917";

            private void SeedBrukere()
            {
                if (_context.Observer.Any() == false)
                {
                    _context.Observer.Add(new Observator
                    {
                        Etternavn = ObservatørEtternavn,
                        Fornavn = ObservatørFornavn,
                        IdentPseudonym = ObservatørIdentPseudonym,
                        Institusjon = _context.Institution.FirstOrDefault(i => i.HERId == HerIdOus)
                    });
                    _context.Observer.Add(new Observator
                    {
                        Etternavn = ObservatørEtternavn,
                        Fornavn = ObservatørFornavn,
                        IdentPseudonym = ObservatørIdentPseudonym,
                        Institusjon = _context.Institution.FirstOrDefault(i => i.HERId == HerIdLillehammer)
                    });
                }

                if (_context.Coordinator.Any() == false)
                {
                    _context.Coordinator.Add(new Koordinator
                    {
                        Etternavn = KoordinatorEtternavn,
                        Fornavn = KoordinatorFornavn,
                        IdentPseudonym = KoordinatorIdentPseudonym,
                        Institusjon = _context.Institution.FirstOrDefault()
                    });
                    _context.Coordinator.Add(new Koordinator
                    {
                        Etternavn = KoordinatorEtternavn,
                        Fornavn = KoordinatorFornavn,
                        IdentPseudonym = KoordinatorIdentPseudonym,
                        Institusjon = _context.Institution.FirstOrDefault(i => i.HERId == HerIdLillehammer)
                    });
                }

                _context.SaveChanges();
            }

            private void SeedInstitusjoner()
            {
                if (_context.Institution.Any())
                    return;

                var roller = _context.Role.ToList();

                var institusjontyper = _context.InstitutionType.ToList();
                var avdelingtyper = _context.SectionType.ToList();

                var institusjoner = new[]
                {
                    new Domene.Place.Institution
                    {
                        Region = _context.Region.First(),
                        HERId = HerIdOus,
                        Name = "Oslo universitetssykehus HF",
                        Abbreviation = "OUS",
                        InstitutionType = institusjontyper.First(),
                        Departments = new List<Domene.Place.Avdeling>
                        {
                            new Domene.Place.Avdeling
                            {
                                Navn = "Nevrokirurgisk",
                                Roller = new List<Role>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Place.Avdeling
                            {
                                Navn = "Allergi og lungeseksjonen",
                                Roller = new List<Role>
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            },
                            new Domene.Place.Avdeling
                            {
                                Navn = "Avdeling for mikrobiologi",
                                Roller = new List<Role>
                                {
                                    roller[5],
                                    roller[6],
                                    roller[1],
                                },
                                Avdelingtype = avdelingtyper.Skip(2).First()
                            }
                        },
                        PredefinedComments = new List<PredefinedComments>
                        {
                            new PredefinedComments { Comment = "Hansker i stedet for håndhygiene", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComments { Comment = "Hansker ikke byttet", SessionType = SessionType.ProtectiveEquipment },
                            new PredefinedComments { Comment = "Dårlig teknikk hånddesinfeksjon", SessionType = SessionType.ProtectiveEquipment }
                        }
                    },
                    new Domene.Place.Institution
                    {
                        HERId = HerIdLillehammer,
                        Region = _context.Region.First(),
                        Name = "Lillehammer sykehus",
                        Abbreviation = "LS",
                        InstitutionType = institusjontyper.First(),
                        Departments = new List<Domene.Place.Avdeling>
                        {
                            new Domene.Place.Avdeling
                            {
                                Navn = "Akutt",
                                Roller = new List<Role>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Place.Avdeling
                            {
                                Navn = "Medisin",
                                Roller = new List<Role>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Place.Avdeling
                            {
                                Navn = "Kirurgisk",
                                Roller = new List<Role>
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            }
                        },
                        PredefinedComments = new List<PredefinedComments>
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

            private void SeedInstitusjonTyper()
            {
                if (_context.InstitutionType.Any())
                    return;

                var institusjontyper = new[]
                {
                    new InstitutionType {Code = "SYKEHUS", Name = "Sykehus"},
                    new InstitutionType {Code = "SYKEHJEM", Name = "Sykehjem"}
                };

                _context.InstitutionType.AddRange(institusjontyper);
                _context.SaveChanges();
            }

            private void SeedAvdelingTyper()
            {
                if (_context.SectionType.Any())
                    return;

                var avdelingstyper = new[]
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

            private void SeedRoller()
            {
                if (_context.Role.Any())
                    return;

                var roller = new List<Role>
                {
                    new Role("Sykepleier"),
                    new Role("Lege"),
                    new Role("Pleiepersonell"),
                    new Role("Jordmor"),
                    new Role("Fysioterapeut"),
                    new Role("Bioingeniør"),
                    new Role("Annet")
                };

                _context.Role.AddRange(roller);
                _context.SaveChanges();
            }
        }
    }
}
