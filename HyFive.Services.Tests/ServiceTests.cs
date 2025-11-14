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
using HyFive.Models.V1.Facility;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.User;
using HyFive.Services.FiveIndication;
using HyFive.Services.Facility;
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

            var city = new HyFive.Domain.Place.City { Id = 666, Name = "Oslo" };
            var facilityType = new HyFive.Domain.Place.FacilityType { Code = "HOSP", Name = "Hospital" };
            var deptType = new HyFive.Domain.Place.DepartmentType { Code = "SURGERY", Name = "Surgery" };

            DatabaseContext.City.Add(city);
            DatabaseContext.FacilityType.Add(facilityType);
            DatabaseContext.DepartmentType.Add(deptType);

            await DatabaseContext.SaveChangesAsync();
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

        protected async Task<(Models.V1.Facility.Facility, Models.V1.User.User)> CreateFacility()
        {
            var CreateInstitutionHandler = new CreateFacility.Handler(DatabaseContext, Mapper);

            DatabaseContext.Role.AddRange(new Domain.Observation.Role("Doctor"), new Domain.Observation.Role("Nurse"));
            await DatabaseContext.SaveChangesAsync();

            var facilityTypeId = (await DatabaseContext.FacilityType.FirstAsync()).Id;
            var cityId = (await DatabaseContext.City.FirstAsync()).Id;

            var facility = await CreateInstitutionHandler.Handle(new CreateFacility.Command()
            {
                Request = new CreateFacilityRequest()
                {
                    CoordinatorEmail = "test@gmail.com",
                    CoordinatorLastName = "Test",
                    CoordinatorFirstName = "User",
                    Abbreviation = "test1",
                    FacilityTypeId = facilityTypeId,
                    FacilityName = "FacilityTest",
                    CityId = cityId
                }
            }, CancellationToken.None);

            var CreateObserverHandler = new CreateObserver.Handler(DatabaseContext, Mapper);

            var observer = await CreateObserverHandler.Handle(new CreateObserver.Command()
            {
                User = new Models.V1.User.User()
                {
                    Email = "test@gmail.com",
                    FacilityId = facility.Id,
                    IsDisabled = false,
                    LastName = "Stangeland",
                    FirstName = "Stian Pål",
                }
            }, CancellationToken.None);

            return (facility, observer);
        }

        protected async Task<Guid> CreateFiveIndicatorsSession(
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

            var departmentModel = Mapper.Map<Models.V1.Facility.Department>(
                department ?? await DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).FirstAsync());
            var institution = await DatabaseContext.Facility.FirstAsync(x => x.Id == departmentModel.FacilityId);
            var activityTypes = await DatabaseContext.ActivityType.ToListAsync();
            var indicationTypesList = await DatabaseContext.IndicationTypes.ToListAsync();

            var saveFiveIndicatorsSessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var observation = new FiveIndicatorsObservation()
            {
                Activity = useDefaultActivity
                    ? new Activity()
                    {
                        ActivityType = new ActivityType()
                        {
                            Id = activityTypes.FirstOrDefault(x => x.Code == ActivityTypeConstants.Handwash).Id
                        },
                        GlovesUsed = null,
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
                Comment = "Cooment for observation",
                RegisteredTime = DateTime.UtcNow,
                Role = useDefaultRole ? departmentModel.Roles[0] : role,
                SessionId = sessionId.ToString()
            };

            var fiveIndicatorsSessionGuid = await saveFiveIndicatorsSessionHandler.Handle(new SaveSession.Command()
            {
                Session = new FiveIndicationsSession
                {
                    Id = sessionId.ToString(),
                    Department = departmentModel,
                    FacilityName = institution.Name,
                    FacilityId = institution.Id,
                    Observations = new List<FiveIndicatorsObservation>()
                    {
                        observation
                    },
                    Comment = "Comment for Session",
                    CreatedDate = DateTime.UtcNow
                }
            }, CancellationToken.None);

            return fiveIndicatorsSessionGuid;
        }
    }
}