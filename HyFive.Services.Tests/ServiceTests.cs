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
using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.User;
using HyFive.Services.FiveIndication;
using HyFive.Services.Institution;
using Moq;
using Microsoft.Extensions.Logging;
using HyFive.Services.Authentication.User;

namespace HyFive.Services.Tests
{

    public abstract class ServiceTests
    {
        protected HandHygieneContext DatabaseContext;
        protected IMapper Mapper;
        protected SqliteConnection _connection;
        protected IUserService UserService;

        [SetUp]
        public async Task Setup()
        {
            DatabaseContext = GetSQLiteInMemoryContext();

            UserService = new Mock<IUserService>().Object;

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

        protected async Task<(Models.V1.Institution.Institution, Models.V1.User.User)> CreateInstitution()
        {
            var CreateInstitutionHandler = new CreateInstitution.Handler(DatabaseContext, Mapper);

            DatabaseContext.Role.AddRange(new Domain.Observation.Role("Doctor"), new Domain.Observation.Role("Nurse"));
            DatabaseContext.SaveChanges();
            var institution = await CreateInstitutionHandler.Handle(new CreateInstitution.Command()
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

            var CreateObserverHandler = new CreateObserver.Handler(DatabaseContext, Mapper);

            var observer = await CreateObserverHandler.Handle(new CreateObserver.Command()
            {
                User = new Models.V1.User.User()
                {
                    HPRNumber = Seed.SeedObservatorHprNummer,
                    InstitutionId = institution.Id,
                    IsDisabled = false,
                    LastName = "Stangeland",
                    FirstName = "Stian Pål",
                }
            }, CancellationToken.None);

            return (institution, observer);
        }

        protected async Task<Guid> CreateFourIndicatorsSession(
            Guid sessionId,
            Guid observationId,
            Domain.Place.Department department,
            string hprNumber,
            bool useDefaultActivity = true,
            Activity activity = null,
            bool useDefaultRole = true,
            Role role = null,
            List<IndicationType> indicationTypes = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var departmentModel = Mapper.Map<Models.V1.Institution.Department>(
                department ?? DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First());
            var institution = DatabaseContext.Institution.First(x => x.Id == departmentModel.InstitutionId);
            var activityTypes = DatabaseContext.ActivityType.ToList();
            var indicationTypesList = DatabaseContext.IndicationTypes.ToList();

            var SaveFourIndicatorsSessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var observation = new FiveIndicatorsObservation()
            {
                Activity = useDefaultActivity
                    ? new Activity()
                    {
                        ActivityType = new ActivityType()
                        {
                            Id = activityTypes.FirstOrDefault(x => x.Code == ActivityTypeConstants.Handwash).Id
                        },
                        GloveUsed = null,
                        SecondsUsed = 3,
                        TimingWasPerformed = true
                    }
                    : activity,
                Id = observationId.ToString(),
                IndicationTypes = indicationTypes ?? new List<IndicationType>()
                {
                    new IndicationType()
                    {
                        Id = indicationTypesList.FirstOrDefault(x => x.Code == IndicationTypeConstants.AfterPatient).Id
                    }
                },
                Comment = "Comment til observasjonen",
                RegistrationTime = DateTime.UtcNow,
                Role = useDefaultRole ? departmentModel.Roles.First() : role,
                SessionId = sessionId.ToString()
            };

            var fourIndicatorsSessionGuid = await SaveFourIndicatorsSessionHandler.Handle(new SaveSession.Command()
            {
                Session = new FiveIndicationsSession
                {
                    Id = sessionId.ToString(),
                    Department = departmentModel,
                    InstitutionName = institution.Name,
                    InstitutionId = institution.Id,
                    Observations = new List<FiveIndicatorsObservation>()
                    {
                        observation
                    },
                    Comment = "Comment til sesjonen",
                    StartTime = DateTime.UtcNow
                },
                HprNumber = hprNumber
            }, CancellationToken.None);

            return fourIndicatorsSessionGuid;
        }
    }
}