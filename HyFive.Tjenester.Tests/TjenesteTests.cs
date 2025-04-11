using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Tjenester.AutoMapperProfiler.V1;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institution;
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
        protected HandHygieneContext DatabaseContext;
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

        protected HandHygieneContext GetSQLiteInMemoryContext()
        {
            var connectionString = new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString();
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<HandHygieneContext>().UseSqlite(_connection).Options;
            var databaseContext = new HandHygieneContext(options);
            databaseContext.Database.EnsureDeleted();
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        protected async Task<(Modeller.V1.Institution.Institution, Modeller.V1.User.User)> OpprettInstitusjon()
        {
            var opprettInstitusjonHandler = new OpprettInstitusjon.Handler(DatabaseContext, Mapper);

            DatabaseContext.Role.AddRange(new Domain.Observation.Role("Lege"), new Domain.Observation.Role("Sykepleier"));
            DatabaseContext.SaveChanges();
            var institusjon = await opprettInstitusjonHandler.Handle(new OpprettInstitusjon.Command()
            {
                Request = new CreateInstitutionRequest()
                {
                    CoordinatorHPRNumber = Seed.SeedKoordinatorHprNummer,
                    CoordinatorLastName = Seed.SeedKoordinatorFornavn,
                    CoordinatorFirstName = Seed.SeedKoordinatorFornavn,
                    Abbreviation = "FHI",
                    HERId = "85217",
                    InstitutionTypeId = DatabaseContext.InstitutionType.First().Id,
                    InstitutionName = "FOLKEHELSEINSTITUTTET",
                    RegionId = DatabaseContext.Region.First().Id
                }
            }, CancellationToken.None);

            var opprettObservatorHandler = new OpprettObservator.Handler(DatabaseContext, Mapper);

            var observator = await opprettObservatorHandler.Handle(new OpprettObservator.Command()
            {
                Bruker = new Modeller.V1.User.User()
                {
                    HPRNumber = Seed.SeedObservatorHprNummer,
                    InstitutionId = institusjon.Id,
                    IsDisabled = false,
                    Surname = "Stangeland",
                    FirstName = "Stian Pål",
                }
            }, CancellationToken.None);

            return (institusjon, observator);
        }

        protected async Task<Guid> OpprettFireIndikasjonerSesjon(
            Guid sesjonId,
            Guid observasjonId,
            Domain.Place.Avdeling avdeling,
            string hprnummer,
            bool brukDefaultAktivitet = true,
            Activity aktivitet = null,
            bool brukDefaultRolle = true,
            Role rolle = null,
            List<IndicationType> indikasjontyper = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institution.Department>(
                avdeling ?? DatabaseContext.Department.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitusjonId);
            var aktivitetTyper = DatabaseContext.ActivityType.ToList();
            var indikasjonTyper = DatabaseContext.IndicationTypes.ToList();

            var lagreFireIndikasjonSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var observasjon = new FireIndikasjonerObservasjon()
            {
                Aktivitet = brukDefaultAktivitet
                    ? new Activity()
                    {
                        ActivityType = new ActivityType()
                        {
                            Id = aktivitetTyper.FirstOrDefault(x => x.Code == AktivitetTypeKonstanter.Handvask).Id
                        },
                        GloveUsed = null,
                        TimeSpent = 3,
                        TimeRecordingWasDone = true
                    }
                    : aktivitet,
                Id = observasjonId.ToString(),
                Indikasjonstyper = indikasjontyper ?? new List<IndicationType>()
                {
                    new IndicationType()
                    {
                        Id = indikasjonTyper.FirstOrDefault(x => x.Code == IndikasjonTypeKonstanter.EtterPasient).Id
                    }
                },
                Kommentar = "Kommentar til observasjonen",
                Registrerttidspunkt = DateTime.Now,
                Rolle = brukDefaultRolle ? avdelingModell.Roller.First() : rolle,
                SesjonId = sesjonId.ToString()
            };

            var fireIndikasjonerSesjonGuid = await lagreFireIndikasjonSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new FireIndikasjonerSesjon
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Name,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<FireIndikasjonerObservasjon>()
                    {
                        observasjon
                    },
                    Kommentar = "Kommentar til sesjonen",
                    Starttidspunkt = DateTime.Now
                },
                HPRNummer = hprnummer
            }, CancellationToken.None);

            return fireIndikasjonerSesjonGuid;
        }
    }
}