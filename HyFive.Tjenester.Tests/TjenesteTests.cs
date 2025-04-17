using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Services.AutoMapperProfiler.V1;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institution;
using HyFive.Modeller.V1.Constants;
using HyFive.Modeller.V1.Observation;
using HyFive.Modeller.V1.Session;
using HyFive.Services.User;
using HyFive.Services.FourIndication;
using HyFive.Services.Institution;
using Moq;
using Microsoft.Extensions.Logging;
using HyFive.Services.Authentication.User;

namespace HyFive.Services.Tests
{

    public abstract class TjenesteTests
    {
        protected HandHygieneContext DatabaseContext;
        protected IMapper Mapper;
        protected SqliteConnection _connection;
        protected IUserService BrukerService;

        [SetUp]
        public async Task Setup()
        {
            DatabaseContext = GetSQLiteInMemoryContext();

            BrukerService = new Mock<IUserService>().Object;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModelsToDomainV1>();
                cfg.AddProfile<DomainToModelsV1>();
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
            var opprettInstitusjonHandler = new CreateInstitution.Handler(DatabaseContext, Mapper);

            DatabaseContext.Role.AddRange(new Domain.Observation.Role("Lege"), new Domain.Observation.Role("Sykepleier"));
            DatabaseContext.SaveChanges();
            var institusjon = await opprettInstitusjonHandler.Handle(new CreateInstitution.Command()
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

            var opprettObservatorHandler = new CreateObserver.Handler(DatabaseContext, Mapper);

            var observator = await opprettObservatorHandler.Handle(new CreateObserver.Command()
            {
                User = new Modeller.V1.User.User()
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
            Domain.Place.Department avdeling,
            string hprnummer,
            bool brukDefaultAktivitet = true,
            Activity aktivitet = null,
            bool brukDefaultRolle = true,
            Role rolle = null,
            List<IndicationType> indikasjontyper = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institution.Department>(
                avdeling ?? DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitutionId);
            var aktivitetTyper = DatabaseContext.ActivityType.ToList();
            var indikasjonTyper = DatabaseContext.IndicationTypes.ToList();

            var lagreFireIndikasjonSesjonHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var observasjon = new FourIndicatorsObservation()
            {
                Activity = brukDefaultAktivitet
                    ? new Activity()
                    {
                        ActivityType = new ActivityType()
                        {
                            Id = aktivitetTyper.FirstOrDefault(x => x.Code == ActivityTypeConstants.Handwash).Id
                        },
                        GloveUsed = null,
                        TimeSpent = 3,
                        TimeRecordingWasDone = true
                    }
                    : aktivitet,
                Id = observasjonId.ToString(),
                IndicationTypes = indikasjontyper ?? new List<IndicationType>()
                {
                    new IndicationType()
                    {
                        Id = indikasjonTyper.FirstOrDefault(x => x.Code == IndicationTypeConstants.AfterPatient).Id
                    }
                },
                Comment = "Kommentar til observasjonen",
                RegistrationTime = DateTime.Now,
                Role = brukDefaultRolle ? avdelingModell.Roles.First() : rolle,
                SessionId = sesjonId.ToString()
            };

            var fireIndikasjonerSesjonGuid = await lagreFireIndikasjonSesjonHandler.Handle(new SaveSession.Command()
            {
                Session = new FourIndicationsSession
                {
                    Id = sesjonId.ToString(),
                    Department = avdelingModell,
                    Institusjonsnavn = institusjon.Name,
                    InstitutionId = institusjon.Id,
                    Observasjoner = new List<FourIndicatorsObservation>()
                    {
                        observasjon
                    },
                    Kommentar = "Kommentar til sesjonen",
                    Starttidspunkt = DateTime.Now
                },
                HprNumber = hprnummer
            }, CancellationToken.None);

            return fireIndikasjonerSesjonGuid;
        }
    }
}