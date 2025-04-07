using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Tjenester.AutoMapperProfiler.V1;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Bruker;
using HyFive.Tjenester.FireIndikasjoner;
using HyFive.Tjenester.Institusjon;
using Moq;
using Microsoft.Extensions.Logging;
using HyFive.Tjenester.Autentisering.Bruker;

namespace HyFive.Tjenester.Tests
{

    public abstract class TjenesteTests
    {
        protected HandhygieneContext DatabaseContext;
        protected IMapper Mapper;
        protected SqliteConnection _connection;
        protected IBrukerService BrukerService;

        [SetUp]
        public async Task Setup()
        {
            DatabaseContext = GetSQLiteInMemoryContext();

            BrukerService = new Mock<IBrukerService>().Object;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModellerV1TilDomene>();
                cfg.AddProfile<DomeneTilModellerV1>();
            });

            Mapper = new Mapper(config);

            var seed = new Seed(DatabaseContext);
            seed.SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            DatabaseContext.Database.CloseConnection();
            DatabaseContext.Database.EnsureDeleted();
        }

        protected HandhygieneContext GetSQLiteInMemoryContext()
        {
            var connectionString = new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString();
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<HandhygieneContext>().UseSqlite(_connection).Options;
            var databaseContext = new HandhygieneContext(options);
            databaseContext.Database.EnsureDeleted();
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        protected async Task<(Modeller.V1.Institusjon.Institusjon, Modeller.V1.Bruker.Bruker)> OpprettInstitusjon()
        {
            var opprettInstitusjonHandler = new OpprettInstitusjon.Handler(DatabaseContext, Mapper);

            DatabaseContext.Rolle.AddRange(new Domene.Observasjon.Rolle("Lege"), new Domene.Observasjon.Rolle("Sykepleier"));
            DatabaseContext.SaveChanges();
            var institusjon = await opprettInstitusjonHandler.Handle(new OpprettInstitusjon.Command()
            {
                Request = new OpprettInstitusjonRequest()
                {
                    KoordinatorHPRnummer = Seed.SeedKoordinatorHprNummer,
                    KoordinatorEtternavn = Seed.SeedKoordinatorFornavn,
                    KoordinatorFornavn = Seed.SeedKoordinatorFornavn,
                    Forkortelse = "FHI",
                    HERId = "85217",
                    InstitusjonTypeId = DatabaseContext.InstitusjonType.First().Id,
                    Institusjonsnavn = "FOLKEHELSEINSTITUTTET",
                    RegionId = DatabaseContext.Region.First().Id
                }
            }, CancellationToken.None);

            var opprettObservatorHandler = new OpprettObservator.Handler(DatabaseContext, Mapper);

            var observator = await opprettObservatorHandler.Handle(new OpprettObservator.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    HPRNummer = Seed.SeedObservatorHprNummer,
                    InstitusjonId = institusjon.Id,
                    ErDeaktivert = false,
                    Etternavn = "Stangeland",
                    Fornavn = "Stian Pål",
                }
            }, CancellationToken.None);

            return (institusjon, observator);
        }

        protected async Task<Guid> OpprettFireIndikasjonerSesjon(
            Guid sesjonId,
            Guid observasjonId,
            Domene.Sted.Avdeling avdeling,
            string hprnummer,
            bool brukDefaultAktivitet = true,
            Aktivitet aktivitet = null,
            bool brukDefaultRolle = true,
            Rolle rolle = null,
            List<IndikasjonType> indikasjontyper = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institusjon.Avdeling>(
                avdeling ?? DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institusjon.First(x => x.Id == avdelingModell.InstitusjonId);
            var aktivitetTyper = DatabaseContext.AktivitetType.ToList();
            var indikasjonTyper = DatabaseContext.IndikasjonType.ToList();

            var lagreFireIndikasjonSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var observasjon = new FireIndikasjonerObservasjon()
            {
                Aktivitet = brukDefaultAktivitet
                    ? new Aktivitet()
                    {
                        AktivitetType = new AktivitetType()
                        {
                            Id = aktivitetTyper.FirstOrDefault(x => x.Kode == AktivitetTypeKonstanter.Handvask).Id
                        },
                        BenyttetHanske = null,
                        SekunderBrukt = 3,
                        TidtakingBleUtfort = true
                    }
                    : aktivitet,
                Id = observasjonId.ToString(),
                Indikasjonstyper = indikasjontyper ?? new List<IndikasjonType>()
                {
                    new IndikasjonType()
                    {
                        Id = indikasjonTyper.FirstOrDefault(x => x.Kode == IndikasjonTypeKonstanter.EtterPasient).Id
                    }
                },
                Kommentar = "Kommentar til observasjonen",
                Registrerttidspunkt = DateTime.UtcNow,
                Rolle = brukDefaultRolle ? avdelingModell.Roller.First() : rolle,
                SesjonId = sesjonId.ToString()
            };

            var fireIndikasjonerSesjonGuid = await lagreFireIndikasjonSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new FireIndikasjonerSesjon
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Navn,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<FireIndikasjonerObservasjon>()
                    {
                        observasjon
                    },
                    Kommentar = "Kommentar til sesjonen",
                    Starttidspunkt = DateTime.UtcNow
                },
                HPRNummer = hprnummer
            }, CancellationToken.None);

            return fireIndikasjonerSesjonGuid;
        }
    }
}