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

namespace HyFive.Services.Tests.Beskyttelsesutstyr
{
    public class BeskyttelsesutstyrTests : ServiceTests
    {
        private Guid sesjonId = Guid.NewGuid();
        private Guid observasjonId = Guid.NewGuid();
        private readonly string hprnummer = "9383840";

        #region BeskyttelsesutstyrSesjon

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var opprettetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await OpprettSesjon(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(avdeling.Roles.First().Name));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].ProtectiveEquipmentList.Count, Is.EqualTo(8));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIndikertForFeilbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

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
        //            .IncorrectTypes.Count, Is.EqualTo(1));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIndikertForRiktigbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

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
        //            .IncorrectTypes.Count, Is.EqualTo(0));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIkkeIndikertForRiktigbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

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
        //            .IncorrectTypes.Count, Is.EqualTo(0));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonOgSjekkerUtstyrSomErIkkeIndikertForFeilbrukTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

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
        //            .IncorrectTypes.Count, Is.EqualTo(2));
        //    });
        //}

        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanOppdatereBeskyttelsesutstyrKommentar()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations.First();
        //    var nyKommentar = Guid.NewGuid()+" bla bla";
        //    var beskyttelsesutstyrSomSkalEndres = observasjonSomSkalEndres.ProtectiveEquipmentList.First(b => b.Id == 1);
        //    var gammelKommentar = beskyttelsesutstyrSomSkalEndres.Comment;
        //    beskyttelsesutstyrSomSkalEndres.Comment = nyKommentar;
            
        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
        //    var oppdatertBeskyttelsesutstyr = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id).ProtectiveEquipmentList
        //        .First(b => b.Id == beskyttelsesutstyrSomSkalEndres.Id);
            
        //    var oppdatertKommentar =  oppdatertBeskyttelsesutstyr.Comment;
            
        //    // Assert
        //    Assert.That(oppdatertKommentar, Is.EqualTo(nyKommentar));
        //}
        
        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanOppdatereBeskyttelsesutstyrObservasjonKommentar()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations.First();
        //    var nyKommentar = Guid.NewGuid()+" bla bla";
        //    observasjonSomSkalEndres.Comment = nyKommentar;

        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
        //    var oppdatertKommentar =  oppdatertObservasjon.Comment;
            
        //    // Assert
        //    Assert.That(oppdatertKommentar, Is.EqualTo(nyKommentar));
        //}
        
        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanFjerneFeilbrukType()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations
        //        .First(o => o.ProtectiveEquipmentList
        //            .Any(u => u.IncorrectTypes
        //                .Any()));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.ProtectiveEquipmentList
        //            .First(u => u.IncorrectTypes
        //                .Any());
        //    var feilbrukTypeSomSkalFjernes = utstyrSomSkalEndres.IncorrectTypes.First();
        //    utstyrSomSkalEndres.IncorrectTypes.Remove(feilbrukTypeSomSkalFjernes);
            
            
        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
            
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
            
        //    var oppdatertUtstyr = oppdatertObservasjon.ProtectiveEquipmentList
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);
            
        //    // Assert
        //    Assert.That(oppdatertUtstyr.IncorrectTypes.Select(f => f.Id), Does.Not.Contain(feilbrukTypeSomSkalFjernes.Id));
        //}
        
        
        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanLeggeTilFeilbrukType()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations
        //        .First(o => o.ProtectiveEquipmentList
        //            .Any(u => u.IncorrectTypes
        //                .Any() == false));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.ProtectiveEquipmentList.First();
        //    var feilbrukTypeSomSkalLeggesTil = Mapper.Map<MisuseType>(DatabaseContext.MisuseType.First());
        //    utstyrSomSkalEndres.IncorrectTypes.Add(feilbrukTypeSomSkalLeggesTil);
            
            
        //    var handler = new UpdateProtectiveEquipmentObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateProtectiveEquipmentObservation.Handler>());
        //    await handler.Handle(new UpdateProtectiveEquipmentObservation.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
            
            
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
            
        //    var oppdatertUtstyr = oppdatertObservasjon.ProtectiveEquipmentList
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);
            
        //    // Assert
        //    Assert.That(oppdatertUtstyr.IncorrectTypes.Select(f => f.Id), Contains.Item(feilbrukTypeSomSkalLeggesTil.Id));
        //}

        protected async Task<ProtectiveEquipmentSession> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentBeskyttelsesutstyrSesjonHandler = new GetProtectiveEquipmentSession.Handler(DatabaseContext, Mapper, UserService);
            var beskyttelsesutstyrSesjon = await hentBeskyttelsesutstyrSesjonHandler.Handle(new GetProtectiveEquipmentSession.Query()
            {
                HPRNumber = hprnummer,
                SessionId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return beskyttelsesutstyrSesjon;
        }

        protected async Task<Guid> OpprettSesjon(Domain.Place.Department avdeling = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var avdelingModell = Mapper.Map<Models.V1.Institution.Department>(
                avdeling ?? DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitutionId);
            var settingTyper = DatabaseContext.ProtectiveEquipmentSettingType.ToList();
            var utstyrsTyper = DatabaseContext.ProtectiveEquipmentType.ToList();

            var lagreBeskyttelsesutstyrSesjonHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var beskyttelsesutstyrSesjonGuid = await lagreBeskyttelsesutstyrSesjonHandler.Handle(new SaveSession.Command()
            {
                Session = new ProtectiveEquipmentSession()
                {
                    Id = sesjonId.ToString(),
                    Department = avdelingModell,
                    InstitutionsName = institusjon.Name,
                    InstitutionId = institusjon.Id,
                    Comment = "Sesjon kommentar",
                    StartTime = DateTime.Now,
                    Observations = new List<ProtectiveEquipmentObservation>()
                    {
                        new ProtectiveEquipmentObservation()
                        {
                            Id = observasjonId.ToString(),
                            SessionId = sesjonId.ToString(),
                            Comment = "Observasjon kommentar",
                            RegistrationTime = DateTime.Now,
                            Role = avdelingModell.Roles.First(),
                            SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                            {
                                Id = settingTyper.First(x => x.Code == Models.V1.Constants.ProtectiveEquipmentSettingTypeConstants.ContactTransmission).Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.Gloves).Id,
                                    },
                                    IncorrectTypes = new List<MisuseType>
                                    {
                                        new MisuseType
                                        {
                                            Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.Gloves).MisuseTypes[0].Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.InfectionGown).Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.CareGown).Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.FaceMask).Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.Hood).Id,
                                    },
                                    IncorrectTypes = new List<MisuseType>
                                    {
                                        new MisuseType
                                        {
                                            Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.Hood).MisuseTypes[0].Id,
                                            IsSelected = true
                                        },
                                        new MisuseType
                                        {
                                            Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.Hood).MisuseTypes[1].Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.EyeProtection).Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.RespiratoryProtection).Id,
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
                                        Id = utstyrsTyper.First(x => x.Code == ProtectiveEquipmentTypeConstants.PlasticApron).Id,
                                    }
                                }
                            }
                        }
                    }
                },
                HPRNumber = hprnummer
            }, CancellationToken.None);

            return beskyttelsesutstyrSesjonGuid;
        }

        #endregion

        #region BeskyttelsesutstyrType

        [Test]
        public async Task HentBeskyttelsesutstyrTyper_Test()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.ProtectiveEquipmentType.Select(x => x.Id).ToList();
            var hentBeskyttelsesutstyrTyper = new GetProtectiveEquipmentTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentTypes.Query();

            // Act
            var res = await hentBeskyttelsesutstyrTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterBeskyttelsesutstyrType_Test()
        {
            // Arrange
            var opprettetBeskyttelsesutstyrType = await OpprettBeskyttelsesutstyrType();
            var oppdaterBeskyttelsesutstyrTypeHandler = new UpdateProtectiveEquipmentType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateProtectiveEquipmentType.Command()
            {
                EquipmentType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType()
                {
                    Id = opprettetBeskyttelsesutstyrType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterBeskyttelsesutstyrTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetBeskyttelsesutstyrType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.EquipmentType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.EquipmentType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetBeskyttelsesutstyrType.Code));
            });
        }

        [Test]
        public void OppdaterBeskyttelsesutstyrType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterBeskyttelsesutstyrTypeHandler = new UpdateProtectiveEquipmentType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateProtectiveEquipmentType.Command()
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
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke beskyttelsesutstyrType"),
                async () =>
                {
                    await oppdaterBeskyttelsesutstyrTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region BeskyttelsesutstyrsettingType

        [Test]
        public async Task HentBeskyttelsesutstyrsettingTyper_Test()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.ProtectiveEquipmentSettingType.Select(x => x.Id).ToList();
            var hentBeskyttelsesutstyrsettingTyper = new GetProtectiveEquipmentSettingTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentSettingTypes.Query();

            // Act
            var res = await hentBeskyttelsesutstyrsettingTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterBeskyttelsesutstyrsettingType_Test()
        {
            // Arrange
            var opprettetBeskyttelsesutstyrsettingType = await OpprettBeskyttelsesutstyrsettingType();
            var oppdaterBeskyttelsesutstyrsettingTypeHandler = new UpdateProtectiveEquipmentSettingType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateProtectiveEquipmentSettingType.Command()
            {
                SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                {
                    Id = opprettetBeskyttelsesutstyrsettingType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterBeskyttelsesutstyrsettingTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetBeskyttelsesutstyrsettingType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.SettingType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.SettingType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetBeskyttelsesutstyrsettingType.Code));
            });
        }

        [Test]
        public void OppdaterBeskyttelsesutstyrsettingType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterBeskyttelsesutstyrsettingTypeHandler = new UpdateProtectiveEquipmentSettingType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateProtectiveEquipmentSettingType.Command()
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
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke beskyttelsesutstyrsettingType"),
                async () =>
                {
                    await oppdaterBeskyttelsesutstyrsettingTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region FeilbrukType

        [Test]
        public async Task HentFeilbrukTyper_Test()
        {
            // Arrange
            var eksisterendeFeilbrukTyperForUtstyr = DatabaseContext.ProtectiveEquipmentType
                .Include(x => x.MisuseTypes)
                .First().MisuseTypes
                .Select(x => x.Id)
                .ToList();
            var hentFeilbrukTyper = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query() { EquipmentTypeId = DatabaseContext.ProtectiveEquipmentType.First().Id };

            // Act
            var res = await hentFeilbrukTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeFeilbrukTyperForUtstyr.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeFeilbrukTyperForUtstyr.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task HentFeilbrukTyper_IkkeEksisterendeUtstyr()
        {
            // Arrange
            var hentFeilbrukTyper = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query() { EquipmentTypeId = 123456789 };

            // Act
            var res = await hentFeilbrukTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Zero);
            });
        }

        [Test]
        public async Task OpprettFeilbrukType_Test()
        {
            // Arrange and Act
            var navn = "test";
            var utstyrtype = DatabaseContext.ProtectiveEquipmentType.First();
            var opprettetFeilbrukType = await OpprettFeilbrukType(navn: navn, utstyrtypeId: utstyrtype.Id);
            var opprettetFeilbrukTypeFraDatabase = DatabaseContext.MisuseType
                .FirstOrDefault(a => a.Id == opprettetFeilbrukType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetFeilbrukType.Id, Is.GreaterThan(0));
                Assert.That(opprettetFeilbrukType.Name, Is.EqualTo(navn));
                Assert.That(opprettetFeilbrukType.Name, Is.EqualTo(opprettetFeilbrukTypeFraDatabase.Name));
                Assert.That(opprettetFeilbrukTypeFraDatabase.ProtectiveEquipmentType.Id, Is.EqualTo(utstyrtype.Id));
            });
        }

        [Test]
        public void OpprettFeilbrukType_IkkeEksisterendeUtstyrType_SkalFeile()
        {
            // Act
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke finne utstyrType"),
                async () =>
                {
                    await OpprettFeilbrukType(utstyrtypeId: 123456789);
                }
            );
        }

        [Test]
        public async Task OppdaterFeilbrukType_Test()
        {
            // Arrange
            var utstyrtype = DatabaseContext.ProtectiveEquipmentType.First();
            var opprettetFeilbrukType = await OpprettFeilbrukType(utstyrtypeId: utstyrtype.Id);
            var oppdaterFeilbrukTypeHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateMisuseType.Command()
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType()
                {
                    Id = opprettetFeilbrukType.Id,
                    Name = "Da Vinci",
                },
                EquipmentTypeId = utstyrtype.Id
            };

            // Act
            var resultatOppdater = await oppdaterFeilbrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetFeilbrukType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.MisuseType.Name));
            });
        }

        [Test]
        public void OppdaterFeilbrukType_IkkeEksisterendeUtstyrTypeId()
        {
            // Arrange
            var oppdaterFeilbrukTypeHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateMisuseType.Command()
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
                    await oppdaterFeilbrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public void OppdaterFeilbrukType_IkkeEksisterendeFeilbruktTypeId()
        {
            // Arrange
            var utstyrtype = DatabaseContext.ProtectiveEquipmentType.First();
            var oppdaterFeilbrukTypeHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateMisuseType.Command()
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType()
                {
                    Id = 123456789,
                    Name = "Da Vinci",
                },
                EquipmentTypeId = utstyrtype.Id
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke finne feilbruktype"),
                async () =>
                {
                    await oppdaterFeilbrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType> OpprettBeskyttelsesutstyrType(string kode = null)
        {
            var beskyttelsesutstyrType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentType.Add(beskyttelsesutstyrType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(beskyttelsesutstyrType);
        }

        private async Task<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType> OpprettBeskyttelsesutstyrsettingType(string kode = null)
        {
            var beskyttelsesutstyrsettingType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentSettingType.Add(beskyttelsesutstyrsettingType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>(beskyttelsesutstyrsettingType);
        }

        private async Task<Models.V1.Observation.ProtectiveEquipment.MisuseType> OpprettFeilbrukType(string navn = null, int utstyrtypeId = 0)
        {
            var opprettFeilbrukTypeHandler = new CreateMisuseType.Handler(DatabaseContext, Mapper);
            var opprettCommand = new CreateMisuseType.Command()
            {
                MisuseType = new CreateIncorrectUseTypeRequest() { Name = navn ?? "Test" },
                EquipmentTypeId = utstyrtypeId != 0 ? utstyrtypeId : DatabaseContext.ProtectiveEquipmentType.First().Id
            };

            var resOpprett = await opprettFeilbrukTypeHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}
