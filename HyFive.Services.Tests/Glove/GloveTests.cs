using Castle.Core.Logging;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Models.V1.Session;
using HyFive.Services.Glove;
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

namespace HyFive.Services.Tests.Glove
{
    public class GloveTests : ServiceTests
    {
        private Guid sessionId = Guid.NewGuid();
        private Guid observationId = Guid.NewGuid();
        private readonly string email = "test@ecdc.com";

        #region GloveSession

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonGuid = await CreateSessionWithIndicationTypes();
        //    var opprettetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

        //    //Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(opprettetSesjonFraDatabase, Is.Not.Null);
        //        Assert.That(opprettetSesjonFraDatabase.Id, Is.EqualTo(opprettetSesjonGuid.ToString()));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonMedIndikasjonTyperTest()
        //{
        //    //Arrange and act
        //    var department = DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateSessionWithIndicationTypes(department);
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GloveWithIndicationTypes, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GloveWithIndicationTypes.Count, Is.EqualTo(2));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GlovesUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].PostGloveHandHygieneType.Code,
        //            Is.EqualTo(HandhygieneEtterHanskebrukTypeKonstanter.Ja));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(department.Roles.First().Name));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonUtenIndikasjonTyperTest()
        //{
        //    //Arrange and act
        //    var department = DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateSessionWithoutIndicatorTypes(department);
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GloveWithoutIndicationTypes, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GloveWithoutIndicationTypes.Count, Is.EqualTo(2));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GlovesUsed, Is.True);
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].PostGloveHandHygieneType.Code,
        //            Is.EqualTo(HandhygieneEtterHanskebrukTypeKonstanter.Nei));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(department.Roles.First().Name));
        //    });
        //}

        private async Task<GloveSession> GetSession(Guid sessionGuidFromRequestGuid)
        {
            var getGloveSessionHandler = new GetGloveSession.Handler(DatabaseContext, Mapper, UserService);
            var handJewelrySession = await getGloveSessionHandler.Handle(new GetGloveSession.Query()
            {
                Email = email,
                SessionId = sessionGuidFromRequestGuid
            }, CancellationToken.None);

            return handJewelrySession;
        }

        private async Task<Guid> CreateSessionWithIndicationTypes(Domain.Place.Department department = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var saveGloveSessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var departmentModel = Mapper.Map<Models.V1.Facility.Department>(
                department ?? DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First());
            var facility = DatabaseContext.Facility.First(x => x.Id == departmentModel.FacilityId);
            var gloveWithIndicationTypes = DatabaseContext.GloveWithIndicationType.ToList();
            var handHygieneAfterGloveUseTypes = DatabaseContext.HandHygieneAfterGloveUseType.ToList();

            var gloveSessionGuid = await saveGloveSessionHandler.Handle(new SaveSession.Command()
            {
                Session = new GloveSession()
                {
                    Id = sessionId.ToString(),
                    Department = departmentModel,
                    FacilityName = facility.Name,
                    FacilityId = facility.Id,
                    Observations = new List<GloveObservation>()
                    {
                        new GloveObservation()
                        {
                            Id = observationId.ToString(),
                            Comment = "Observation comment",
                            RegisteredTime = DateTime.UtcNow,
                            Role = departmentModel.Roles.First(),
                            SessionId = sessionId.ToString(),
                            GloveWithIndicationTypes = new List<GloveWithIndicationType>()
                            {
                                new GloveWithIndicationType()
                                {
                                    IsSelected = true,
                                    Id = gloveWithIndicationTypes.FirstOrDefault(x => x.Code == GloveWithIndicationTypeConstants.Transmission).Id
                                },
                                new GloveWithIndicationType()
                                {
                                    IsSelected = true,
                                    Id = gloveWithIndicationTypes.FirstOrDefault(x => x.Code == GloveWithIndicationTypeConstants.BodyFluids).Id
                                }
                            },
                            GlovesUsed = true,
                            PostGloveHandHygieneType = new PostGloveHandHygieneType()
                            {
                                Id = handHygieneAfterGloveUseTypes.FirstOrDefault(x => x.Code == HandHygieneAfterGloveUseTypeConstants.Yes).Id
                            }
                        }
                    },
                    Comment = "Session Number",
                    CreatedDate = DateTime.UtcNow
                },
                Email = email
            }, CancellationToken.None);

            return gloveSessionGuid;
        }

        private async Task<Guid> CreateSessionWithoutIndicatorTypes(Domain.Place.Department department = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var saveGloveSessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var departmentModel = Mapper.Map<Models.V1.Facility.Department>(
                department ?? DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First());
            var facility = DatabaseContext.Facility.First(x => x.Id == departmentModel.FacilityId);
            var gloveWithoutIndicationTypes = DatabaseContext.GloveWithoutIndicationType.ToList();
            var handHygieneAfterGloveUseTypes = DatabaseContext.HandHygieneAfterGloveUseType.ToList();

            var gloveSessionGuid = await saveGloveSessionHandler.Handle(new SaveSession.Command()
            {
                Session = new GloveSession()
                {
                    Id = sessionId.ToString(),
                    Department = departmentModel,
                    FacilityName = facility.Name,
                    FacilityId = facility.Id,
                    Observations = new List<GloveObservation>()
                    {
                        new GloveObservation()
                        {
                            Id = observationId.ToString(),
                            Comment = "Observation Comment",
                            RegisteredTime = DateTime.UtcNow,
                            Role = departmentModel.Roles.First(),
                            SessionId = sessionId.ToString(),
                            GloveWithoutIndicationTypes = new List<GloveWithoutIndicationType>()
                            {
                                new GloveWithoutIndicationType()
                                {
                                    IsSelected = true,
                                    Id = gloveWithoutIndicationTypes.FirstOrDefault(x => x.Code == GloveWithoutIndicationTypeConstants.Food).Id
                                },
                                new GloveWithoutIndicationType()
                                {
                                    IsSelected = true,
                                    Id = gloveWithoutIndicationTypes.FirstOrDefault(x => x.Code == GloveWithoutIndicationTypeConstants.CareWithoutBodyFluids).Id
                                }
                            },
                            GlovesUsed = true,
                            PostGloveHandHygieneType = new PostGloveHandHygieneType()
                            {
                                Id = handHygieneAfterGloveUseTypes.FirstOrDefault(x => x.Code == HandHygieneAfterGloveUseTypeConstants.No).Id
                            }
                        }
                    },
                    Comment = "Session comment",
                    CreatedDate = DateTime.UtcNow
                },
                Email = email
            }, CancellationToken.None);

            return gloveSessionGuid;
        }

        #endregion

        #region GloveWithIndicatorType

        [Test]
        public async Task GetGloveWithIndicationTypes_Test()
        {
            // Arrange
            if (!await DatabaseContext.GloveWithIndicationType.AnyAsync())
            {
                DatabaseContext.GloveWithIndicationType.AddRange(
                    new Domain.Observation.Gloves.GloveWithIndicationType
                    {
                        Name = "Glove with indication type A",
                        Code = "GWI-A"
                    },
                    new Domain.Observation.Gloves.GloveWithIndicationType
                    {
                        Name = "Glove with indication type B",
                        Code = "GWI-B"
                    }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.GloveWithIndicationType.Select(x => x.Id).ToListAsync();
            var getGloveWithIndicationTypes = new GetGloveWithIndicationTypes.Handler(DatabaseContext, Mapper);
            var query = new GetGloveWithIndicationTypes.Query();

            // Act
            var res = await getGloveWithIndicationTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateGloveWithIndicationType_Test()
        {
            // Arrange
            var createdGloveWithIndicationType = await CreatedGloveWithIndicationType();
            var updateGloveWithIndicationTypeHandler = new UpdateGloveWithIndicationType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateGloveWithIndicationType.Command()
            {
                GloveWithIndicationType = new Models.V1.Observation.Gloves.GloveWithIndicationType()
                {
                    Id = createdGloveWithIndicationType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResult = await updateGloveWithIndicationTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResult.Id, Is.EqualTo(createdGloveWithIndicationType.Id));
                Assert.That(updateResult.Name, Is.EqualTo(updateCommand.GloveWithIndicationType.Name));
                Assert.That(updateResult.Code, Is.Not.EqualTo(updateCommand.GloveWithIndicationType.Code));
                Assert.That(updateResult.Code, Is.EqualTo(createdGloveWithIndicationType.Code));
            });
        }

        [Test]
        public void UpdateGloveWithIndicatorType_NonExistentId()
        {
            // Arrange
            var UpdateGloveWithIndicationTypeHandler = new UpdateGloveWithIndicationType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateGloveWithIndicationType.Command()
            {
                GloveWithIndicationType = new Models.V1.Observation.Gloves.GloveWithIndicationType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("GloveWithIndicationTypeNotFound"),
                async () =>
                {
                    await UpdateGloveWithIndicationTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region GloveWithoutIndicatorType

        [Test]
        public async Task GetGloveWithoutIndicationTypes_Test()
        {
            // Arrange
            if (!await DatabaseContext.GloveWithoutIndicationType.AnyAsync())
            {
                DatabaseContext.GloveWithoutIndicationType.AddRange(
                    new Domain.Observation.Gloves.GloveWithoutIndicationType
                    {
                        Name = "Glove without indication type A",
                        Code = "GWIO-A"
                    },
                    new Domain.Observation.Gloves.GloveWithoutIndicationType
                    {
                        Name = "Glove without indication type B",
                        Code = "GWIO-B"
                    }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.GloveWithoutIndicationType.Select(x => x.Id).ToListAsync();
            var getGloveWithoutIndicationTypes = new GetGloveWithoutIndicationTypes.Handler(DatabaseContext, Mapper);
            var query = new GetGloveWithoutIndicationTypes.Query();

            // Act
            var res = await getGloveWithoutIndicationTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateGloveWithoutIndicationType_Test()
        {
            // Arrange
            var createdGloveWithoutIndicationType = await CreatedGloveWithoutIndicationType();
            var updateGloveWithoutIndicationTypeHandler = new UpdateGloveWithoutIndicationType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateGloveWithoutIndicationType.Command()
            {
                GloveWithoutIndicationType = new Models.V1.Observation.Gloves.GloveWithoutIndicationType()
                {
                    Id = createdGloveWithoutIndicationType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResult = await updateGloveWithoutIndicationTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResult.Id, Is.EqualTo(createdGloveWithoutIndicationType.Id));
                Assert.That(updateResult.Name, Is.EqualTo(updateCommand.GloveWithoutIndicationType.Name));
                Assert.That(updateResult.Code, Is.Not.EqualTo(updateCommand.GloveWithoutIndicationType.Code));
                Assert.That(updateResult.Code, Is.EqualTo(createdGloveWithoutIndicationType.Code));
            });
        }

        [Test]
        public void UpdateGloveWithoutIndicatorType_NonExistentId()
        {
            // Arrange
            var updateGloveWithoutIndicationType = new UpdateGloveWithoutIndicationType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateGloveWithoutIndicationType.Command()
            {
                GloveWithoutIndicationType = new Models.V1.Observation.Gloves.GloveWithoutIndicationType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("GloveWithoutIndicationTypeNotFound"),
                async () =>
                {
                    await updateGloveWithoutIndicationType.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region HandHygieneAfterGloveUseType

        [Test]
        public async Task GetHandHygieneAfterGloveUseTypes_Test()
        {
            // Arrange
            if (!await DatabaseContext.HandHygieneAfterGloveUseType.AnyAsync())
            {
                DatabaseContext.HandHygieneAfterGloveUseType.AddRange(
                    new Domain.Observation.Gloves.HandHygieneAfterGloveUseType
                    {
                        Name = "After glove removal",
                        Code = "HHGU-1"
                    },
                    new Domain.Observation.Gloves.HandHygieneAfterGloveUseType
                    {
                        Name = "After contact with patient surroundings",
                        Code = "HHGU-2"
                    }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.HandHygieneAfterGloveUseType.Select(x => x.Id).ToListAsync();
            var getHandHygieneAfterGloveUseTypes = new GetHandHygieneAfterGloveUseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetHandHygieneAfterGloveUseTypes.Query();

            // Act
            var res = await getHandHygieneAfterGloveUseTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateHandHygieneAfterGloveUseType_Test()
        {
            // Arrange
            var createdHandHygieneAfterGloveUseType = await CreatedHandHygieneAfterGloveUseType();
            var updateHandHygieneAfterGloveUseTypeHandler = new UpdateHandHygieneAfterGloveUseType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateHandHygieneAfterGloveUseType.Command()
            {
                HandHygieneAfterGloveUseType = new Models.V1.Observation.Gloves.PostGloveHandHygieneType()
                {
                    Id = createdHandHygieneAfterGloveUseType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateHandHygieneAfterGloveUseTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdHandHygieneAfterGloveUseType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.HandHygieneAfterGloveUseType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.HandHygieneAfterGloveUseType.Code));
                Assert.That(updateResults.Code, Is.EqualTo(createdHandHygieneAfterGloveUseType.Code));
            });
        }

        [Test]
        public void UpdateHandHygieneAfterGloveUseType_NonExistentId()
        {
            // Arrange
            var updateHandHygieneAfterGloveUseTypeHandler = new UpdateHandHygieneAfterGloveUseType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateHandHygieneAfterGloveUseType.Command()
            {
                HandHygieneAfterGloveUseType = new Models.V1.Observation.Gloves.PostGloveHandHygieneType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("HandHygieneAfterGloveUseTypeNotFound"),
                async () =>
                {
                    await updateHandHygieneAfterGloveUseTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.Gloves.GloveWithIndicationType> CreatedGloveWithIndicationType(string code = null)
        {
            var gloveWithIndicationType = new Domain.Observation.Gloves.GloveWithIndicationType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.GloveWithIndicationType.Add(gloveWithIndicationType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.Gloves.GloveWithIndicationType>(gloveWithIndicationType);
        }

        private async Task<Models.V1.Observation.Gloves.GloveWithoutIndicationType> CreatedGloveWithoutIndicationType(string code = null)
        {
            var gloveWithoutIndicationType = new Domain.Observation.Gloves.GloveWithoutIndicationType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.GloveWithoutIndicationType.Add(gloveWithoutIndicationType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.Gloves.GloveWithoutIndicationType>(gloveWithoutIndicationType);
        }

        private async Task<Models.V1.Observation.Gloves.PostGloveHandHygieneType> CreatedHandHygieneAfterGloveUseType(string code = null)
        {
            var handHygieneAfterGloveUseType = new Domain.Observation.Gloves.HandHygieneAfterGloveUseType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.HandHygieneAfterGloveUseType.Add(handHygieneAfterGloveUseType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.Gloves.PostGloveHandHygieneType>(handHygieneAfterGloveUseType);
        }

        #endregion
    }
}