using HyFive.Dataaksess;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Sted;
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
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
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
                if (_context.Observator.Any() == false)
                {
                    _context.Observator.Add(new Observator
                    {
                        Etternavn = ObservatørEtternavn,
                        Fornavn = ObservatørFornavn,
                        IdentPseudonym = ObservatørIdentPseudonym,
                        Institusjon = _context.Institusjon.FirstOrDefault(i => i.HERId == HerIdOus)
                    });
                    _context.Observator.Add(new Observator
                    {
                        Etternavn = ObservatørEtternavn,
                        Fornavn = ObservatørFornavn,
                        IdentPseudonym = ObservatørIdentPseudonym,
                        Institusjon = _context.Institusjon.FirstOrDefault(i => i.HERId == HerIdLillehammer)
                    });
                }

                if (_context.Koordinator.Any() == false)
                {
                    _context.Koordinator.Add(new Koordinator
                    {
                        Etternavn = KoordinatorEtternavn,
                        Fornavn = KoordinatorFornavn,
                        IdentPseudonym = KoordinatorIdentPseudonym,
                        Institusjon = _context.Institusjon.FirstOrDefault()
                    });
                    _context.Koordinator.Add(new Koordinator
                    {
                        Etternavn = KoordinatorEtternavn,
                        Fornavn = KoordinatorFornavn,
                        IdentPseudonym = KoordinatorIdentPseudonym,
                        Institusjon = _context.Institusjon.FirstOrDefault(i => i.HERId == HerIdLillehammer)
                    });
                }

                _context.SaveChanges();
            }

            private void SeedInstitusjoner()
            {
                if (_context.Institusjon.Any())
                    return;

                var roller = _context.Rolle.ToList();

                var institusjontyper = _context.InstitusjonType.ToList();
                var avdelingtyper = _context.AvdelingType.ToList();

                var institusjoner = new[]
                {
                    new Domene.Sted.Institusjon
                    {
                        Region = _context.Region.First(),
                        HERId = HerIdOus,
                        Navn = "Oslo universitetssykehus HF",
                        Forkortelse = "OUS",
                        Institusjontype = institusjontyper.First(),
                        Avdelinger = new List<Domene.Sted.Avdeling>
                        {
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Nevrokirurgisk",
                                Roller = new List<Rolle>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Allergi og lungeseksjonen",
                                Roller = new List<Rolle>
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
                                Roller = new List<Rolle>
                                {
                                    roller[5],
                                    roller[6],
                                    roller[1],
                                },
                                Avdelingtype = avdelingtyper.Skip(2).First()
                            }
                        },
                        PredefinerteKommmentarer = new List<PredefinertKommentar>
                        {
                            new PredefinertKommentar { Kommentar = "Hansker i stedet for håndhygiene", SesjonType = SesjonType.Beskyttelsesutstyr },
                            new PredefinertKommentar { Kommentar = "Hansker ikke byttet", SesjonType = SesjonType.Beskyttelsesutstyr },
                            new PredefinertKommentar { Kommentar = "Dårlig teknikk hånddesinfeksjon", SesjonType = SesjonType.Beskyttelsesutstyr }
                        }
                    },
                    new Domene.Sted.Institusjon
                    {
                        HERId = HerIdLillehammer,
                        Region = _context.Region.First(),
                        Navn = "Lillehammer sykehus",
                        Forkortelse = "LS",
                        Institusjontype = institusjontyper.First(),
                        Avdelinger = new List<Domene.Sted.Avdeling>
                        {
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Akutt",
                                Roller = new List<Rolle>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Medisin",
                                Roller = new List<Rolle>(roller),
                                Avdelingtype = avdelingtyper.First()
                            },
                            new Domene.Sted.Avdeling
                            {
                                Navn = "Kirurgisk",
                                Roller = new List<Rolle>
                                {
                                    roller[0],
                                    roller[1],
                                    roller[2],
                                    roller[3],
                                },
                                Avdelingtype = avdelingtyper.Skip(1).First()
                            }
                        },
                        PredefinerteKommmentarer = new List<PredefinertKommentar>
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

            private void SeedInstitusjonTyper()
            {
                if (_context.InstitusjonType.Any())
                    return;

                var institusjontyper = new[]
                {
                    new InstitusjonType {Kode = "SYKEHUS", Navn = "Sykehus"},
                    new InstitusjonType {Kode = "SYKEHJEM", Navn = "Sykehjem"}
                };

                _context.InstitusjonType.AddRange(institusjontyper);
                _context.SaveChanges();
            }

            private void SeedAvdelingTyper()
            {
                if (_context.AvdelingType.Any())
                    return;

                var avdelingstyper = new[]
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

            private void SeedRoller()
            {
                if (_context.Rolle.Any())
                    return;

                var roller = new List<Rolle>
                {
                    new Rolle("Sykepleier"),
                    new Rolle("Lege"),
                    new Rolle("Pleiepersonell"),
                    new Rolle("Jordmor"),
                    new Rolle("Fysioterapeut"),
                    new Rolle("Bioingeniør"),
                    new Rolle("Annet")
                };

                _context.Rolle.AddRange(roller);
                _context.SaveChanges();
            }
        }
    }
}
