using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Beskyttelsesutstyr;
using HyFive.Tjenester.Sesjon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace HyFive.Tjenester.Tests.Beskyttelsesutstyr
{
    public class BeskyttelsesutstyrTests : TjenesteTests
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Role).First();
        //    var opprettetSesjonId = await OpprettSesjon(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Role.Name, Is.EqualTo(avdeling.Role.First().Name));
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
        //            .SettingType.Code, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
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
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .SettingType.Code, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .WasUsedCorrectly, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .IsRequired, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .MisuseTypes.Count, Is.EqualTo(0));
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
        //            .Code, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .WasUsedCorrectly, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .IsRequired, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .MisuseTypes.Count, Is.EqualTo(0));
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
        //            .Code, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .WasUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .WasUsedCorrectly, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .IsRequired, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0]
        //            .ProtectiveEquipmentList.First(x => x.EquipmentType.Code == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .MisuseTypes.Count, Is.EqualTo(2));
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
            
        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
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

        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
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
        //            .Any(u => u.MisuseTypes
        //                .Any()));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.ProtectiveEquipmentList
        //            .First(u => u.MisuseTypes
        //                .Any());
        //    var feilbrukTypeSomSkalFjernes = utstyrSomSkalEndres.MisuseTypes.First();
        //    utstyrSomSkalEndres.MisuseTypes.Remove(feilbrukTypeSomSkalFjernes);
            
            
        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
            
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
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observations
        //        .First(o => o.ProtectiveEquipmentList
        //            .Any(u => u.MisuseTypes
        //                .Any() == false));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.ProtectiveEquipmentList.First();
        //    var feilbrukTypeSomSkalLeggesTil = Mapper.Map<MisuseType>(DatabaseContext.MisuseType.First());
        //    utstyrSomSkalEndres.MisuseTypes.Add(feilbrukTypeSomSkalLeggesTil);
            
            
        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
        //    {
        //        Observation = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
            
            
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observations
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
            
        //    var oppdatertUtstyr = oppdatertObservasjon.ProtectiveEquipmentList
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);
            
        //    // Assert
        //    Assert.That(oppdatertUtstyr.MisuseTypes.Select(f => f.Id), Contains.Item(feilbrukTypeSomSkalLeggesTil.Id));
        //}

        protected async Task<BeskyttelsesutstyrSesjon> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentBeskyttelsesutstyrSesjonHandler = new HentBeskyttelsesutstyrSesjon.Handler(DatabaseContext, Mapper, BrukerService);
            var beskyttelsesutstyrSesjon = await hentBeskyttelsesutstyrSesjonHandler.Handle(new HentBeskyttelsesutstyrSesjon.Query()
            {
                HPRNummer = hprnummer,
                SesjonId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return beskyttelsesutstyrSesjon;
        }

        protected async Task<Guid> OpprettSesjon(Domain.Place.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institution.Department>(
                avdeling ?? DatabaseContext.Department.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitusjonId);
            var settingTyper = DatabaseContext.ProtectiveEquipmentSettingType.ToList();
            var utstyrsTyper = DatabaseContext.ProtectiveEquipmentType.ToList();

            var lagreBeskyttelsesutstyrSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var beskyttelsesutstyrSesjonGuid = await lagreBeskyttelsesutstyrSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new BeskyttelsesutstyrSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Name,
                    InstitusjonId = institusjon.Id,
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.Now,
                    Observasjoner = new List<ProtectiveEquipmentObservation>()
                    {
                        new ProtectiveEquipmentObservation()
                        {
                            Id = observasjonId.ToString(),
                            SesjonId = sesjonId.ToString(),
                            Kommentar = "Observasjon kommentar",
                            Registrerttidspunkt = DateTime.Now,
                            Rolle = avdelingModell.Roller.First(),
                            SettingType = new ProtectiveEquipmentSettingType()
                            {
                                Id = settingTyper.First(x => x.Code == BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte).Id,
                            },
                            ProtectiveEquipmentList = new List<Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment>()
                            {
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "I",
                                    WasUsed = true,
                                    WasUsedCorrectly = false,
                                    IsRequired = true,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Hansker).Id,
                                    },
                                    MisuseTypes = new List<MisuseType>
                                    {
                                        new MisuseType
                                        {
                                            Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Hansker).MisuseTypes[0].Id,
                                            IsSelected = true
                                        }
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "II",
                                    WasUsed = true,
                                    WasUsedCorrectly = true,
                                    IsRequired = true,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Smittefrakk).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "III",
                                    WasUsed = true,
                                    WasUsedCorrectly = true,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Stellefrakk).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "IV",
                                    WasUsed = true,
                                    WasUsedCorrectly = true,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Munnbind).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "V",
                                    WasUsed = true,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Hette).Id,
                                    },
                                    MisuseTypes = new List<MisuseType>
                                    {
                                        new MisuseType
                                        {
                                            Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Hette).MisuseTypes[0].Id,
                                            IsSelected = true
                                        },
                                        new MisuseType
                                        {
                                            Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Hette).MisuseTypes[1].Id,
                                            IsSelected = true
                                        }
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "VI",
                                    WasUsed = false,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "VII",
                                    WasUsed = false,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Andedrettsvern).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment()
                                {
                                    Comment = "VIII",
                                    WasUsed = false,
                                    WasUsedCorrectly = false,
                                    IsRequired = false,
                                    EquipmentType = new ProtectiveEquipmentType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Code == BeskyttelsesutstyrTypeKonstanter.Plastforkle).Id,
                                    }
                                }
                            }
                        }
                    }
                },
                HPRNummer = hprnummer
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
            var hentBeskyttelsesutstyrTyper = new HentBeskyttelsesutstyrTyper.Handler(DatabaseContext, Mapper);
            var query = new HentBeskyttelsesutstyrTyper.Query();

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
            var oppdaterBeskyttelsesutstyrTypeHandler = new OppdaterBeskyttelsesutstyrType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterBeskyttelsesutstyrType.Command()
            {
                UtstyrType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType()
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
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.UtstyrType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.UtstyrType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetBeskyttelsesutstyrType.Code));
            });
        }

        [Test]
        public void OppdaterBeskyttelsesutstyrType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterBeskyttelsesutstyrTypeHandler = new OppdaterBeskyttelsesutstyrType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterBeskyttelsesutstyrType.Command()
            {
                UtstyrType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType()
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
            var hentBeskyttelsesutstyrsettingTyper = new HentBeskyttelsesutstyrsettingTyper.Handler(DatabaseContext, Mapper);
            var query = new HentBeskyttelsesutstyrsettingTyper.Query();

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
            var oppdaterBeskyttelsesutstyrsettingTypeHandler = new OppdaterBeskyttelsesutstyrsettingType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterBeskyttelsesutstyrsettingType.Command()
            {
                SettingType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentSettingType()
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
            var oppdaterBeskyttelsesutstyrsettingTypeHandler = new OppdaterBeskyttelsesutstyrsettingType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterBeskyttelsesutstyrsettingType.Command()
            {
                SettingType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentSettingType()
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
            var hentFeilbrukTyper = new HentFeilbrukTyper.Handler(DatabaseContext, Mapper);
            var query = new HentFeilbrukTyper.Query() { UtstyrTypeId = DatabaseContext.ProtectiveEquipmentType.First().Id };

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
            var hentFeilbrukTyper = new HentFeilbrukTyper.Handler(DatabaseContext, Mapper);
            var query = new HentFeilbrukTyper.Query() { UtstyrTypeId = 123456789 };

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
                Assert.That(opprettetFeilbrukTypeFraDatabase.BeskyttelsesutstyrType.Id, Is.EqualTo(utstyrtype.Id));
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
            var oppdaterFeilbrukTypeHandler = new OppdaterFeilbrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterFeilbrukType.Command()
            {
                FeilbrukType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.MisuseType()
                {
                    Id = opprettetFeilbrukType.Id,
                    Name = "Da Vinci",
                },
                UtstyrTypeId = utstyrtype.Id
            };

            // Act
            var resultatOppdater = await oppdaterFeilbrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetFeilbrukType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.FeilbrukType.Name));
            });
        }

        [Test]
        public void OppdaterFeilbrukType_IkkeEksisterendeUtstyrTypeId()
        {
            // Arrange
            var oppdaterFeilbrukTypeHandler = new OppdaterFeilbrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterFeilbrukType.Command()
            {
                FeilbrukType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.MisuseType()
                {
                    Id = DatabaseContext.MisuseType.First().Id,
                    Name = "Da Vinci",
                },
                UtstyrTypeId = 123456789
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
            var oppdaterFeilbrukTypeHandler = new OppdaterFeilbrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterFeilbrukType.Command()
            {
                FeilbrukType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.MisuseType()
                {
                    Id = 123456789,
                    Name = "Da Vinci",
                },
                UtstyrTypeId = utstyrtype.Id
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

        private async Task<Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType> OpprettBeskyttelsesutstyrType(string kode = null)
        {
            var beskyttelsesutstyrType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentType.Add(beskyttelsesutstyrType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType>(beskyttelsesutstyrType);
        }

        private async Task<Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentSettingType> OpprettBeskyttelsesutstyrsettingType(string kode = null)
        {
            var beskyttelsesutstyrsettingType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentSettingType.Add(beskyttelsesutstyrsettingType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentSettingType>(beskyttelsesutstyrsettingType);
        }

        private async Task<Modeller.V1.Observasjon.Beskyttelsesutstyr.MisuseType> OpprettFeilbrukType(string navn = null, int utstyrtypeId = 0)
        {
            var opprettFeilbrukTypeHandler = new OpprettFeilbrukType.Handler(DatabaseContext, Mapper);
            var opprettCommand = new OpprettFeilbrukType.Command()
            {
                FeilbrukType = new OpprettFeilbrukTypeRequest() { Navn = navn ?? "Test" },
                UtstyrTypeId = utstyrtypeId != 0 ? utstyrtypeId : DatabaseContext.ProtectiveEquipmentType.First().Id
            };

            var resOpprett = await opprettFeilbrukTypeHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}
