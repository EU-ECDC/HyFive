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
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                DatabaseContext.ProtectiveEquipmentType.AddRange(
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                    {
                        Name = "Gloves",
                        Code = "PE-G"
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                    {
                        Name = "Mask",
                        Code = "PE-M"
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                    {
                        Name = "Gown",
                        Code = "PE-GW"
                    }
                );

                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.ProtectiveEquipmentType
         .Select(x => x.Id)
         .ToListAsync();

            var handler = new GetProtectiveEquipmentTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentTypes.Query();

            // Act
            var res = await handler.Handle(query, new System.Threading.CancellationToken());

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
                Is.TypeOf<ArgumentException>().And.Message.Contains("Did not find protective equipment type with ID: 99999999"),
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
            if (!await DatabaseContext.ProtectiveEquipmentSettingType.AnyAsync())
            {
                DatabaseContext.ProtectiveEquipmentSettingType.AddRange(
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Name = "In patient room",
                        Code = "SET-1"
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Name = "Operating theatre",
                        Code = "SET-2"
                    }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.ProtectiveEquipmentSettingType
                .Select(x => x.Id)
                .ToListAsync();

            var handler = new GetProtectiveEquipmentSettingTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentSettingTypes.Query();

            // Act
            var res = await handler.Handle(query, new System.Threading.CancellationToken());

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
                Is.TypeOf<ArgumentException>().And.Message.Contains("Did not find protective equipment setting type with ID: 99999999"),
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
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                var gloves = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };

                gloves.MisuseTypes = new List<Domain.Observation.ProtectiveEquipment.MisuseType>
                    {
                        new Domain.Observation.ProtectiveEquipment.MisuseType { Name = "Worn incorrectly" },
                        new Domain.Observation.ProtectiveEquipment.MisuseType { Name = "Not replaced when damaged" }
                    };

                DatabaseContext.ProtectiveEquipmentType.Add(gloves);
                await DatabaseContext.SaveChangesAsync();
            }

            var equipment = await DatabaseContext.ProtectiveEquipmentType
                .Include(x => x.MisuseTypes)
                .FirstAsync();

            var existingMisuseTypesForEquipment = equipment.MisuseTypes
                .Select(x => x.Id)
                .ToList();

            var handler = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query { EquipmentTypeId = equipment.Id };

            // Act
            var res = await handler.Handle(query, CancellationToken.None);

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
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                DatabaseContext.ProtectiveEquipmentType.AddRange(
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType { Name = "Gloves", Code = "PE-1" },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType { Name = "Mask", Code = "PE-2" }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var name = "test";
            var equipmentType = await DatabaseContext.ProtectiveEquipmentType.FirstAsync();
            var createdMisuseType = await CreatedMisuseType(name: name, equipmentTypeId: equipmentType.Id);
            var createdMisuseTypeFromDatabase = await DatabaseContext.MisuseType
                .FirstOrDefaultAsync(a => a.Id == createdMisuseType.Id);

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
                Is.TypeOf<ArgumentException>().And.Message.Contains("Did not find equipment type with ID: 123456789"),
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
            // Ensure at least one ProtectiveEquipmentType exists
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                var gloves = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };
                DatabaseContext.ProtectiveEquipmentType.Add(gloves);
                await DatabaseContext.SaveChangesAsync();
            }

            var equipmentType = await DatabaseContext.ProtectiveEquipmentType.FirstAsync();

            // Create a misuse type linked via navigation property
            var misuse = new Domain.Observation.ProtectiveEquipment.MisuseType
            {
                Name = "Incorrect use",
                ProtectiveEquipmentType = equipmentType
            };
            DatabaseContext.MisuseType.Add(misuse);
            await DatabaseContext.SaveChangesAsync();

            var updateHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);

            var updateCommand = new UpdateMisuseType.Command
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType
                {
                    Id = misuse.Id,
                    Name = "Da Vinci"
                },
                EquipmentTypeId = equipmentType.Id
            };

            // Act
            var updateResults = await updateHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(misuse.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.MisuseType.Name));
            });
        }

        [Test]
        public async  Task UpdateMisuseType_NonExistentEquipmentTypeId()
        {
            // Arrange
            // Ensure there’s at least one MisuseType in the DB
            if (!await DatabaseContext.MisuseType.AnyAsync())
            {
                var fakeEquipmentType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };

                var misuse = new Domain.Observation.ProtectiveEquipment.MisuseType
                {
                    Name = "Incorrect use",
                    ProtectiveEquipmentType = fakeEquipmentType
                };

                DatabaseContext.MisuseType.Add(misuse);
                await DatabaseContext.SaveChangesAsync();
            }

            var misuseType = await DatabaseContext.MisuseType.FirstAsync();

            var handler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);

            var command = new UpdateMisuseType.Command
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType
                {
                    Id = misuseType.Id,
                    Name = "Da Vinci"
                },
                // Invalid equipment type ID
                EquipmentTypeId = 123456789
            };


            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("Did not find equipment type with ID: 123456789"),
                async () =>
                {
                    await handler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task UpdateMisuseType_NonExistentMisuseTypeId()
        {
            // Arrange
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                var gloves = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };
                DatabaseContext.ProtectiveEquipmentType.Add(gloves);
                await DatabaseContext.SaveChangesAsync();
            }

            var equipmentType = await DatabaseContext.ProtectiveEquipmentType.FirstAsync();

            var handler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);

            var command = new UpdateMisuseType.Command
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType
                {
                    Id = 123456789,   // non-existent MisuseType
                    Name = "Da Vinci"
                },
                EquipmentTypeId = equipmentType.Id
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("Did not find misuse type with ID: 123456789"),
                async () =>
                {
                    await handler.Handle(command, new System.Threading.CancellationToken());
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
