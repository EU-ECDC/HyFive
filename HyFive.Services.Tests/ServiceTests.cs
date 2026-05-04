using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Place;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.AutoMapperProfiler.V1;
using HyFive.Services.Facility;
using HyFive.Services.FiveIndication;
using HyFive.Services.User;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            var defaults = DatabaseContext.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                    .Where(p => p.GetDefaultValueSql() != null)
                    .Select(p => new
                    {
                        Entity = e.ClrType.Name,
                        Property = p.Name,
                        DefaultSql = p.GetDefaultValueSql()
                    }))
                .ToList();

            UserService = new Mock<IUserService>().Object;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModelsToDomainV1>();
                cfg.AddProfile<DomainToModelsV1>();
            });

            Mapper = new Mapper(config);

            // Seed Unit levels
            if (!await DatabaseContext.OrganisationUnitLevel.AnyAsync())
            {
                DatabaseContext.OrganisationUnitLevel.AddRange(
                    new HyFive.Domain.Place.OrganisationUnitLevel
                    {
                        Level = OrganisationUnitLevels.Facility
                    },
                    new HyFive.Domain.Place.OrganisationUnitLevel
                    {
                        Level = OrganisationUnitLevels.Department
                    },
                    new HyFive.Domain.Place.OrganisationUnitLevel
                    {
                        Level = OrganisationUnitLevels.Unit
                    }
                );
            }

            // Seed Unit types
            if (!await DatabaseContext.OrganisationUnitType.AnyAsync())
            {
                DatabaseContext.OrganisationUnitType.AddRange(
                    new HyFive.Domain.Place.OrganisationUnitType
                    {
                        Code = "HOSP",
                        Name = "Hospital"
                    },
                    new HyFive.Domain.Place.OrganisationUnitType
                    {
                        Code = "GEN",
                        Name = "General"
                    }
                );
            }

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

        protected async Task<(Models.V1.OrganisationUnit.OrganisationUnit FacilityOu, Domain.User.User)> CreateFacility()
        {
            await EnsureOrganisationUnitLevel(OrganisationUnitLevels.Facility);
            var facilityType = await EnsureOrganisationUnitType("HOSP", "Hospital");

            var handler = new CreateFacility.Handler(DatabaseContext, Mapper);

            var facility = await handler.Handle(new CreateFacility.Command
            {
                Request = new CreateOrganisationUnitRequest
                {
                    Name = "FacilityTest",
                    Abbreviation = "FT",
                    OrganisationUnitTypeId = facilityType.Id,
                    City = "Oslo",
                    FirstName = "Coord",
                    LastName = "User",
                    Email = "coord@test.com"
                }
            }, CancellationToken.None);

            // create observer user + permission
            var observer = new Domain.User.User
            {
                FirstName = "Obs",
                LastName = "User",
                Email = "observer@test.com",
                IsDeactivated = false,
                CreatedTime = DateTime.UtcNow,
                IdentityPseudonym = null
            };

            var obsPerm = new Domain.User.UserPermission
            {
                User = observer,
                OrganisationUnitId = facility.Id,
                PermissionLevel = PermissionLevelConstants.Observer
            };

            DatabaseContext.User.Add(observer);
            DatabaseContext.UserPermission.Add(obsPerm);
            await DatabaseContext.SaveChangesAsync();

            return (facility, observer);
        }

        protected async Task<Guid> CreateFiveIndicatorsSession(
            Guid sessionId,
            Guid observationId,
            Domain.Place.OrganisationUnit department,
            string hprNumber,
            bool useDefaultActivity = true,
            Activity activity = null,
            bool useDefaultRole = true,
            Role role = null,
            List<IndicationType> indicationTypes = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var domainOrganisationUnit = department
                ?? await DatabaseContext.OrganisationUnit
                    .Include(x => x.Parent)
                    .Include(x => x.OrganisationUnitRoles)
                        .ThenInclude(x => x.Role)
                    .FirstAsync(x => x.ParentId != null);

            var organisationUnitModel = Mapper.Map<Models.V1.OrganisationUnit.OrganisationUnit>(domainOrganisationUnit);

            var activityTypes = await DatabaseContext.ActivityType.ToListAsync();
            var indicationTypesList = await DatabaseContext.IndicationTypes.ToListAsync();

            var saveFiveIndicatorsSessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);

            var observation = new FiveIndicatorsObservation
            {
                Activity = useDefaultActivity
                    ? new Activity
                    {
                        ActivityType = new ActivityType
                        {
                            Id = activityTypes.First(x => x.Code == ActivityTypeConstants.Handwash).Id
                        },
                        GlovesUsed = null,
                        SecondsUsed = 3,
                        TimingWasPerformed = true
                    }
                    : activity,
                Id = observationId.ToString(),
                IndicationTypes = indicationTypes ?? new List<IndicationType>
        {
            new IndicationType
            {
                Id = indicationTypesList.First(x => x.Code == IndicationTypeConstants.AfterPatient).Id
            }
        },
                Comment = "Cooment for observation",
                RegisteredTime = DateTime.UtcNow,
                Role = useDefaultRole ? organisationUnitModel.Roles.First() : role,
                SessionId = sessionId.ToString()
            };

            var fiveIndicatorsSessionGuid = await saveFiveIndicatorsSessionHandler.Handle(
                new SaveSession.Command
                {
                    Session = new FiveIndicationsSession
                    {
                        Id = sessionId,
                        UnitId = organisationUnitModel.Id,
                        Unit = organisationUnitModel,
                        Observations = new List<FiveIndicatorsObservation>
                        {
                    observation
                        },
                        Comment = "Comment for Session",
                        CreatedDate = DateTime.UtcNow
                    }
                },
                CancellationToken.None);

            return fiveIndicatorsSessionGuid;
        }

        private async Task<Domain.Place.OrganisationUnitLevel> EnsureOrganisationUnitLevel(string levelName)
        {
            var lvl = await DatabaseContext.OrganisationUnitLevel.FirstOrDefaultAsync(x => x.Level == levelName);
            if (lvl != null) return lvl;

            lvl = new Domain.Place.OrganisationUnitLevel { Level = levelName };
            DatabaseContext.OrganisationUnitLevel.Add(lvl);
            await DatabaseContext.SaveChangesAsync();
            return lvl;
        }

        private async Task<Domain.Place.OrganisationUnitType> EnsureOrganisationUnitType(string code, string name)
        {
            var type = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync(x => x.Code == code);
            if (type != null) return type;

            type = new Domain.Place.OrganisationUnitType { Code = code, Name = name };
            DatabaseContext.OrganisationUnitType.Add(type);
            await DatabaseContext.SaveChangesAsync();
            return type;
        }
    }
}