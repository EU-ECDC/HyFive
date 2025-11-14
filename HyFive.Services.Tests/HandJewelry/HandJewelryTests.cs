using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.HandJewelry;
using HyFive.Services.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.HandJewelry
{
    public class HandJewelryTests : ServiceTests
    {
        private Guid sessionId = Guid.NewGuid();
        private Guid observationId = Guid.NewGuid();
        private readonly string hprnumber = "9383840";

        #region HandJewelrySession

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonGuid = await CreateSession();
        //    var opprettetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

        //    //Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(opprettetSesjonFraDatabase, Is.Not.Null);
        //        Assert.That(opprettetSesjonFraDatabase.Id, Is.EqualTo(opprettetSesjonGuid.ToString()));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonTest()
        //{
        //    //Arrange and act
        //    var department = DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateSession(department);
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].HandJewelries.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(department.Roles.First().Name));
        //    });
        //}

        private async Task<HandJewelrySession> GetSession(Guid sessionGuidFromRequestGuid)
        {
            var getHandJewelrySessionHandler = new GetHandJewelrySession.Handler(DatabaseContext, Mapper, UserService);
            var HandJewelrySession = await getHandJewelrySessionHandler.Handle(new GetHandJewelrySession.Query()
            {
                Email = hprnumber,
                SessionId = sessionGuidFromRequestGuid
            }, CancellationToken.None);

            return HandJewelrySession;
        }

        private async Task<Guid> CreateSession(Domain.Place.Department department = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var departmentModel = Mapper.Map<Models.V1.Facility.Department>(department ?? DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First());
            var facility = DatabaseContext.Facility.First(x => x.Id == departmentModel.FacilityId);
            var handJewelryTypes = DatabaseContext.HandJewelryType.ToList();

            var saveHandJewelrySessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var handJewelrySessionGuid = await saveHandJewelrySessionHandler.Handle(new SaveSession.Command()
            {
                Session = new HandJewelrySession()
                {
                    Id = sessionId.ToString(),
                    Department = departmentModel,
                    FacilityName = facility.Name,
                    FacilityId = facility.Id,
                    Observations = new List<HandJewelryObservation>()
                    {
                        new HandJewelryObservation()
                        {
                            Id = observationId.ToString(),
                            Comment = "Observation comment",
                            RegisteredTime = DateTime.UtcNow,
                            Role = departmentModel.Roles.First(),
                            SessionId = sessionId.ToString(),
                            HandJewelries = new List<HandJewelryType>()
                            {
                                new HandJewelryType()
                                {
                                    Id = handJewelryTypes.FirstOrDefault(x => x.Code == HandJewelryTypeConstants.WatchBracelet).Id
                                }
                            }
                        }
                    },
                    Comment = "Session comment",
                    CreatedDate = DateTime.UtcNow
                },
                Email = hprnumber
            }, CancellationToken.None);

            return handJewelrySessionGuid;
        }

        #endregion

        #region handJewelryType

        [Test]
        public async Task GetHandJewelryTypes_Test()
        {
            // Arrange
            if (!await DatabaseContext.HandJewelryType.AnyAsync())
            {
                DatabaseContext.HandJewelryType.AddRange(
                    new Domain.Observation.HandJewelryType
                    {
                        Name = "Rings",
                        Code = "HJ-R",
                        IsActive = true
                    },
                    new Domain.Observation.HandJewelryType
                    {
                        Name = "Bracelets",
                        Code = "HJ-B",
                        IsActive = true
                    },
                    new Domain.Observation.HandJewelryType
                    {
                        Name = "Necklace",
                        Code = "HJ-N",
                        IsActive = false // won’t be returned, since you filter by IsActive
                    }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.HandJewelryType.Where(x => x.IsActive).Select(x => x.Id).ToListAsync();
            var getHandJewelryTypes = new GetHandJewelryTypes.Handler(DatabaseContext, Mapper);
            var query = new GetHandJewelryTypes.Query();

            // Act
            var res = await getHandJewelryTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task GetHandJewelryType_Test()
        {
            // Arrange
            var createdHandJewelryType = await CreatedHandJewelryType();
            var getHandJewelryTypeHandler = new GetHandJewelryType.Handler(DatabaseContext, Mapper);
            var query = new GetHandJewelryType.Query() { Id = createdHandJewelryType.Id };

            // Act
            var getHandJewelryTypeResult = await getHandJewelryTypeHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(getHandJewelryTypeResult, Is.Not.Null);
                Assert.That(getHandJewelryTypeResult, Has.Property(nameof(HandJewelryType.Code)).EqualTo(createdHandJewelryType.Code));
                Assert.That(getHandJewelryTypeResult, Has.Property(nameof(HandJewelryType.Name)).EqualTo(createdHandJewelryType.Name));
            });
        }

        [Test]
        public async Task GetHandJewelryType_IdDoesNotExist_ReturnsNull()
        {
            // Arrange
            var createdHandJewelryType = await CreatedHandJewelryType();
            var getHandJewelryTypeHandler = new GetHandJewelryType.Handler(DatabaseContext, Mapper);
            var query = new GetHandJewelryType.Query() { Id = 123456789 };

            // Act
            var getHandJewelryTypeResult = await getHandJewelryTypeHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(getHandJewelryTypeResult, Is.Null);
            });
        }

        [Test]
        public async Task UpdateHandJewelryTypeTest()
        {
            // Arrange
            var createdHandJewelryType = await CreatedHandJewelryType();
            var updateHandJewelryTypeHandler = new UpdateHandJewelryType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateHandJewelryType.Command()
            {
                HandJewelryType = new Models.V1.Observation.HandJewelryType()
                {
                    Id = createdHandJewelryType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateHandJewelryTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdHandJewelryType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.HandJewelryType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.HandJewelryType.Code));
                Assert.That(updateResults.Code, Is.EqualTo(createdHandJewelryType.Code));
            });
        }

        [Test]
        public void UpdateHandJewelryType_NonExistentId()
        {
            // Arrange
            var updateHandJewelryTypeHandler = new UpdateHandJewelryType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateHandJewelryType.Command()
            {
                HandJewelryType = new Models.V1.Observation.HandJewelryType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("Did not find jewelry type with id 99999999"),
                async () =>
                {
                    await updateHandJewelryTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.HandJewelryType> CreatedHandJewelryType(string code = null)
        {
            var handJewelryType = new Domain.Observation.HandJewelryType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.HandJewelryType.Add(handJewelryType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.HandJewelryType>(handJewelryType);
        }

        #endregion
    }
}