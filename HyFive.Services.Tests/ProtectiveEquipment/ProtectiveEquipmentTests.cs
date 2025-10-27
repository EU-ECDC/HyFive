using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Session;
using HyFive.Services.ProtectiveEquipment;
using HyFive.Services.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace HyFive.Services.Tests.ProtectiveEquipment
{
    public class ProtectiveEquipmentTests : ServiceTests
    {
        private Guid sessionId = Guid.NewGuid();
        private Guid observationId = Guid.NewGuid();
        private readonly string hprnumber = "9383840";

        #region ProtectiveEquipmentSession


        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await CreateSession();
        //    var opprettetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    //Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(opprettetSesjonFraDatabase, Is.Not.Null);
        //        Assert.That(opprettetSesjonFraDatabase.Id, Is.EqualTo(opprettetSesjonId.ToString()));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonTest()
        //{
        //    //Arrange and act
        //    var department = DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateSession(department);
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(department.Roles.First().Name));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].ProtectiveEquipmentList.Count, Is.EqualTo(8));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIndikertForFeilbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .SettingType.Code, Is.EqualTo(ProtectiveEquipmentSettingTypeConstants.ContactTransmission));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Gloves)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Gloves)
        //            .WasUsedCorrectly, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Gloves)
        //            .IsRequired, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Gloves)
        //            .MisuseTypes.Count, Is.EqualTo(1));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIndikertForRiktigbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .SettingType.Code, Is.EqualTo(ProtectiveEquipmentSettingTypeConstants.ContactTransmission));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.InfectionGown)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.InfectionGown)
        //            .WasUsedCorrectly, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.InfectionGown)
        //            .IsRequired, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.InfectionGown)
        //            .MisuseTypes.Count, Is.EqualTo(0));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIkkeIndikertForRiktigbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .SettingType
        //            .Code, Is.EqualTo(ProtectiveEquipmentSettingTypeConstants.ContactTransmission));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.FaceMask)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.FaceMask)
        //            .WasUsedCorrectly, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.FaceMask)
        //            .IsRequired, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.FaceMask)
        //            .MisuseTypes.Count, Is.EqualTo(0));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIkkeIndikertForFeilbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .SettingType
        //            .Code, Is.EqualTo(ProtectiveEquipmentSettingTypeConstants.ContactTransmission));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hood)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hood)
        //            .WasUsedCorrectly, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hood)
        //            .IsRequired, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hood)
        //            .MisuseTypes.Count, Is.EqualTo(2));
        //    });
        //}

        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanOppdatereBeskyttelsesutstyrKommentar()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations.First();
        //    var nyKommentar = Guid.NewGuid()+" bla bla";
        //    var beskyttelsesutstyrSomSkalEndres = observasjonSomSkalEndres.ProtectiveEquipmentList.First(b => b.Id == 1);
        //    var gammelKommentar = beskyttelsesutstyrSomSkalEndres.Municipality;
        //    beskyttelsesutstyrSomSkalEndres.Municipality = nyKommentar;

        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);

        //    var hentetSesjonFraDatabaseEtterEndring = await GetSession(opprettetSesjonId);
        //    var oppdatertBeskyttelsesutstyr = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id).ProtectiveEquipmentList
        //        .First(b => b.Id == beskyttelsesutstyrSomSkalEndres.Id);

        //    var oppdatertKommentar =  oppdatertBeskyttelsesutstyr.Municipality;

        //    // Assert
        //    Assert.That(oppdatertKommentar, Is.EqualTo(nyKommentar));
        //}

        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanOppdatereBeskyttelsesutstyrObservasjonKommentar()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations.First();
        //    var nyKommentar = Guid.NewGuid()+" bla bla";
        //    observasjonSomSkalEndres.Municipality = nyKommentar;

        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);

        //    var hentetSesjonFraDatabaseEtterEndring = await GetSession(opprettetSesjonId);
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
        //    var oppdatertKommentar =  oppdatertObservasjon.Municipality;

        //    // Assert
        //    Assert.That(oppdatertKommentar, Is.EqualTo(nyKommentar));
        //}

        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanFjerneFeilbrukType()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations
        //        .First(o => o.ProtectiveEquipmentList
        //            .Any(u => u.MisuseTypes
        //                .Any()));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.ProtectiveEquipmentList
        //            .First(u => u.MisuseTypes
        //                .Any());
        //    var feilbrukTypeSomSkalFjernes = utstyrSomSkalEndres.MisuseTypes.First();
        //    utstyrSomSkalEndres.MisuseTypes.Remove(feilbrukTypeSomSkalFjernes);


        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);

        //    var hentetSesjonFraDatabaseEtterEndring = await GetSession(opprettetSesjonId);

        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);

        //    var oppdatertUtstyr = oppdatertObservasjon.ProtectiveEquipmentList
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);

        //    // Assert
        //    Assert.That(oppdatertUtstyr.MisuseTypes.Select(f => f.Id), Does.Not.Contain(feilbrukTypeSomSkalFjernes.Id));
        //}


        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanLeggeTilFeilbrukType()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await CreateSession();
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations
        //        .First(o => o.ProtectiveEquipmentList
        //            .Any(u => u.MisuseTypes
        //                .Any() == false));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.ProtectiveEquipmentList.First();
        //    var feilbrukTypeSomSkalLeggesTil = Mapper.Map<MisuseType>(DatabaseContext.MisuseType.First());
        //    utstyrSomSkalEndres.MisuseTypes.Add(feilbrukTypeSomSkalLeggesTil);


        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);

        //    var hentetSesjonFraDatabaseEtterEndring = await GetSession(opprettetSesjonId);


        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);

        //    var oppdatertUtstyr = oppdatertObservasjon.ProtectiveEquipmentList
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);

        //    // Assert
        //    Assert.That(oppdatertUtstyr.MisuseTypes.Select(f => f.Id), Contains.Item(feilbrukTypeSomSkalLeggesTil.Id));
        //}

        protected async Task<ProtectiveEquipmentSession> GetSession(Guid sessionGuidFromRequestGuid)
        {
            var getProtectiveEquipmentSessionHandler = new GetProtectiveEquipmentSession.Handler(DatabaseContext, Mapper, UserService);
            var protectiveEquipmentSession = await getProtectiveEquipmentSessionHandler.Handle(new GetProtectiveEquipmentSession.Query()
            {
                Email = hprnumber,
                SessionId = sessionGuidFromRequestGuid
            }, CancellationToken.None);

            return protectiveEquipmentSession;
        }

        protected async Task<Guid> CreateSession(Domain.Place.Department department = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var departmentModel = Mapper.Map<Models.V1.Facility.Department>(
                department ?? DatabaseContext.Department.Include(x => x.Facility).Include(x => x.Roles).First());
            var facility = DatabaseContext.Facility.First(x => x.Id == departmentModel.FacilityId);
            var settingTypes = DatabaseContext.ProtectiveEquipmentSettingType.ToList();
            var equipmentTypes = DatabaseContext.ProtectiveEquipmentType.ToList();

            var saveProtectiveEquipmentSessionHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var protectiveEquipmentSessionGuid = await saveProtectiveEquipmentSessionHandler.Handle(new SaveSession.Command()
            {
                Session = new ProtectiveEquipmentSession()
                {
                    Id = sessionId.ToString(),
                    Department = departmentModel,
                    FacilityName = facility.Name,
                    FacilityId = facility.Id,
                    Comment = "Session comment",
                    CreatedDate = DateTime.UtcNow,
                    Observations = new List<ProtectiveEquipmentObservation>()
                    {
                        new ProtectiveEquipmentObservation()
                        {
                            Id = observationId.ToString(),
                            SessionId = sessionId.ToString(),
                            Comment = "Observation comment",
                            RegisteredTime = DateTime.UtcNow,
                            Role = departmentModel.Roles.First(),
                            SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                            {
                                Id = settingTypes.First(x => x.Code == Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.ContactTransmission).Id,
                            },
                            ProtectiveEquipmentList = new List<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment>()
                            {
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "I",
                                    WasUsed = true,
                                    WasUsedCorrectly = false,
                                    IsRequired = true,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.Gloves).Id,
                                    },
                                    MisuseTypes = new List<MisuseType>
                                    {
                                        new MisuseType
                                        {
                                            Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.Gloves).MisuseTypes[0].Id,
                                            IsSelected = true
                                        }
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "II",
                                    WasUsed = true,
                                    WasUsedCorrectly = true,
                                    IsRequired = true,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.CareGown).Id,
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "III",
                                    WasUsed = true,
                                    WasUsedCorrectly = true,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.CareGown).Id,
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "IV",
                                    WasUsed = true,
                                    WasUsedCorrectly = true,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.FaceMask).Id,
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "V",
                                    WasUsed = true,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.Hood).Id,
                                    },
                                    MisuseTypes = new List<MisuseType>
                                    {
                                        new MisuseType
                                        {
                                            Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.Hood).MisuseTypes[0].Id,
                                            IsSelected = true
                                        },
                                        new MisuseType
                                        {
                                            Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.Hood).MisuseTypes[1].Id,
                                            IsSelected = true
                                        }
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "VI",
                                    WasUsed = false,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.EyeProtection).Id,
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "VII",
                                    WasUsed = false,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.RespiratoryProtection).Id,
                                    }
                                },
                                new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment()
                                {
                                    Comment = "VIII",
                                    WasUsed = false,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = equipmentTypes.First(x => x.Code == ProtectiveEquipmentTypeConstants.PlasticApron).Id,
                                    }
                                }
                            }
                        }
                    }
                },
                Email = hprnumber
            }, CancellationToken.None);

            return protectiveEquipmentSessionGuid;
        }

        #endregion

        #region ProtectiveEquipmentType

        [Test]
        public async Task GetProtectiveEquipmentTypes_Test()
        {
            // Arrange
            var existingTypes = DatabaseContext.ProtectiveEquipmentType.Select(x => x.Id).ToList();
            var getProtectiveEquipmentTypes = new GetProtectiveEquipmentTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentTypes.Query();

            // Act
            var res = await getProtectiveEquipmentTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateProtectiveEquipmentType_Test()
        {
            // Arrange
            var createdProtectiveEquipmentType = await CreatedProtectiveEquipmentType();
            var updateProtectiveEquipmentTypeHandler = new UpdateProtectiveEquipmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentType.Command()
            {
                EquipmentType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType()
                {
                    Id = createdProtectiveEquipmentType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateProtectiveEquipmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdProtectiveEquipmentType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.EquipmentType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.EquipmentType.Code));
                Assert.That(updateResults.Code, Is.EqualTo(createdProtectiveEquipmentType.Code));
            });
        }

        [Test]
        public void UpdateProtectiveEquipmentType_NonExistentId()
        {
            // Arrange
            var updateProtectiveEquipmentTypeHandler = new UpdateProtectiveEquipmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentType.Command()
            {
                EquipmentType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Did not find protectiveEquipmentType"),
                async () =>
                {
                    await updateProtectiveEquipmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region ProtectiveEquipmentSettingType

        [Test]
        public async Task GetProtectiveEquipmentSettingTypes_Test()
        {
            // Arrange
            var existingTypes = DatabaseContext.ProtectiveEquipmentSettingType.Select(x => x.Id).ToList();
            var getProtectiveEquipmentSettingTypes = new GetProtectiveEquipmentSettingTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentSettingTypes.Query();

            // Act
            var res = await getProtectiveEquipmentSettingTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateProtectiveEquipmentSettingType_Test()
        {
            // Arrange
            var createdProtectiveEquipmentSettingType = await CreateProtectiveEquipmentSettingType();
            var updateProtectiveEquipmentSettingTypeHandler = new UpdateProtectiveEquipmentSettingType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentSettingType.Command()
            {
                SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                {
                    Id = createdProtectiveEquipmentSettingType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateProtectiveEquipmentSettingTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdProtectiveEquipmentSettingType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.SettingType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.SettingType.Code));
                Assert.That(updateResults.Code, Is.EqualTo(createdProtectiveEquipmentSettingType.Code));
            });
        }

        [Test]
        public void UpdateProtectiveEquipmentSettingType_NonExistentId()
        {
            // Arrange
            var updateProtectiveEquipmentSettingTypeHandler = new UpdateProtectiveEquipmentSettingType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentSettingType.Command()
            {
                SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Did not find protectiveEquipmentSettingType"),
                async () =>
                {
                    await updateProtectiveEquipmentSettingTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region MisusedType

        [Test]
        public async Task GetMisuseTypes_Test()
        {
            // Arrange
            var existingMisuseTypesForEquipment = DatabaseContext.ProtectiveEquipmentType
                .Include(x => x.MisuseTypes)
                .First().MisuseTypes
                .Select(x => x.Id)
                .ToList();
            var getMisuseTypes = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query() { EquipmentTypeId = DatabaseContext.ProtectiveEquipmentType.First().Id };

            // Act
            var res = await getMisuseTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingMisuseTypesForEquipment.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingMisuseTypesForEquipment.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task GetMisuseTypes_NonExistentEquipment()
        {
            // Arrange
            var getMisuseTypes = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query() { EquipmentTypeId = 123456789 };

            // Act
            var res = await getMisuseTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Zero);
            });
        }

        [Test]
        public async Task CreateMisuseType_Test()
        {
            // Arrange and Act
            var name = "test";
            var equipmentType = DatabaseContext.ProtectiveEquipmentType.First();
            var createdMisuseType = await CreatedMisuseType(name: name, equipmentTypeId: equipmentType.Id);
            var createdMisuseTypeFromDatabase = DatabaseContext.MisuseType
                .FirstOrDefault(a => a.Id == createdMisuseType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createdMisuseType.Id, Is.GreaterThan(0));
                Assert.That(createdMisuseType.Name, Is.EqualTo(name));
                Assert.That(createdMisuseType.Name, Is.EqualTo(createdMisuseTypeFromDatabase.Name));
                Assert.That(createdMisuseTypeFromDatabase.ProtectiveEquipmentType.Id, Is.EqualTo(equipmentType.Id));
            });
        }

        [Test]
        public void CreateMisuseType_NonExistentEquipmentType_ShouldFail()
        {
            // Act
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("equipment type not found"),
                async () =>
                {
                    await CreatedMisuseType(equipmentTypeId: 123456789);
                }
            );
        }

        [Test]
        public async Task UpdateMisuseType_Test()
        {
            // Arrange
            var equipmentType = DatabaseContext.ProtectiveEquipmentType.First();
            var createdMisuseType = await CreatedMisuseType(equipmentTypeId: equipmentType.Id);
            var updateMisuseTypeHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateMisuseType.Command()
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType()
                {
                    Id = createdMisuseType.Id,
                    Name = "Da Vinci",
                },
                EquipmentTypeId = equipmentType.Id
            };

            // Act
            var updateResults = await updateMisuseTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdMisuseType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.MisuseType.Name));
            });
        }

        [Test]
        public void UpdateMisuseType_NonExistentEquipmentTypeId()
        {
            // Arrange
            var updateMisuseTypeHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateMisuseType.Command()
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType()
                {
                    Id = DatabaseContext.MisuseType.First().Id,
                    Name = "Da Vinci",
                },
                EquipmentTypeId = 123456789
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke finne utstyrType"),
                async () =>
                {
                    await updateMisuseTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public void UpdateMisuseType_NonExistentMisuseTypeId()
        {
            // Arrange
            var equipmentType = DatabaseContext.ProtectiveEquipmentType.First();
            var updateMisuseTypeHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateMisuseType.Command()
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType()
                {
                    Id = 123456789,
                    Name = "Da Vinci",
                },
                EquipmentTypeId = equipmentType.Id
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke finne feilbruktype"),
                async () =>
                {
                    await updateMisuseTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType> CreatedProtectiveEquipmentType(string code = null)
        {
            var protectiveEquipmentType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentType.Add(protectiveEquipmentType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(protectiveEquipmentType);
        }

        private async Task<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType> CreateProtectiveEquipmentSettingType(string code = null)
        {
            var protectiveEquipmentSettingType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentSettingType.Add(protectiveEquipmentSettingType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>(protectiveEquipmentSettingType);
        }

        private async Task<Models.V1.Observation.ProtectiveEquipment.MisuseType> CreatedMisuseType(string name = null, int equipmentTypeId = 0)
        {
            var createMisuseTypeHandler = new CreateMisuseType.Handler(DatabaseContext, Mapper);
            var createCommand = new CreateMisuseType.Command()
            {
                MisuseType = new CreateIncorrectUseTypeRequest() { Name = name ?? "Test" },
                EquipmentTypeId = equipmentTypeId != 0 ? equipmentTypeId : DatabaseContext.ProtectiveEquipmentType.First().Id
            };

            var createResults = await createMisuseTypeHandler.Handle(createCommand, new System.Threading.CancellationToken());

            return createResults;
        }

        #endregion
    }
}
