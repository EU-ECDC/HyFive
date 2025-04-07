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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettSesjon(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Rolle.Navn, Is.EqualTo(avdeling.Roller.First().Navn));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Beskyttelsesutstyrliste.Count, Is.EqualTo(8));
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
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Settingtype.Kode, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hansker)
        //            .BleBenyttet, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hansker)
        //            .BleBenyttetRiktig, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hansker)
        //            .ErIndikert, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hansker)
        //            .Feilbruktyper.Count, Is.EqualTo(1));
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
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Settingtype.Kode, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .BleBenyttet, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .BleBenyttetRiktig, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .ErIndikert, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Smittefrakk)
        //            .Feilbruktyper.Count, Is.EqualTo(0));
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
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Settingtype
        //            .Kode, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .BleBenyttet, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .BleBenyttetRiktig, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .ErIndikert, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Munnbind)
        //            .Feilbruktyper.Count, Is.EqualTo(0));
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
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Settingtype
        //            .Kode, Is.EqualTo(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .BleBenyttet, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .BleBenyttetRiktig, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .ErIndikert, Is.EqualTo(false));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0]
        //            .Beskyttelsesutstyrliste.First(x => x.Utstyrstype.Kode == BeskyttelsesutstyrTypeKonstanter.Hette)
        //            .Feilbruktyper.Count, Is.EqualTo(2));
        //    });
        //}

        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanOppdatereBeskyttelsesutstyrKommentar()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observasjoner.First();
        //    var nyKommentar = Guid.NewGuid()+" bla bla";
        //    var beskyttelsesutstyrSomSkalEndres = observasjonSomSkalEndres.Beskyttelsesutstyrliste.First(b => b.Id == 1);
        //    var gammelKommentar = beskyttelsesutstyrSomSkalEndres.Kommentar;
        //    beskyttelsesutstyrSomSkalEndres.Kommentar = nyKommentar;
            
        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
        //    {
        //        Observasjon = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
        //    var oppdatertBeskyttelsesutstyr = hentetSesjonFraDatabaseEtterEndring.Observasjoner
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id).Beskyttelsesutstyrliste
        //        .First(b => b.Id == beskyttelsesutstyrSomSkalEndres.Id);
            
        //    var oppdatertKommentar =  oppdatertBeskyttelsesutstyr.Kommentar;
            
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
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observasjoner.First();
        //    var nyKommentar = Guid.NewGuid()+" bla bla";
        //    observasjonSomSkalEndres.Kommentar = nyKommentar;

        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
        //    {
        //        Observasjon = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observasjoner
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
        //    var oppdatertKommentar =  oppdatertObservasjon.Kommentar;
            
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
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observasjoner
        //        .First(o => o.Beskyttelsesutstyrliste
        //            .Any(u => u.Feilbruktyper
        //                .Any()));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.Beskyttelsesutstyrliste
        //            .First(u => u.Feilbruktyper
        //                .Any());
        //    var feilbrukTypeSomSkalFjernes = utstyrSomSkalEndres.Feilbruktyper.First();
        //    utstyrSomSkalEndres.Feilbruktyper.Remove(feilbrukTypeSomSkalFjernes);
            
            
        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
        //    {
        //        Observasjon = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
            
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observasjoner
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
            
        //    var oppdatertUtstyr = oppdatertObservasjon.Beskyttelsesutstyrliste
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);
            
        //    // Assert
        //    Assert.That(oppdatertUtstyr.Feilbruktyper.Select(f => f.Id), Does.Not.Contain(feilbrukTypeSomSkalFjernes.Id));
        //}
        
        
        //[Test]
        //public async Task OppdaterBeskyttelsesutstyrObservasjon_KanLeggeTilFeilbrukType()
        //{
        //    //Arrange
        //    var opprettetSesjonId = await OpprettSesjon();
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonId);

        //    // Act
        //    var observasjonSomSkalEndres = hentetSesjonFraDatabase.Observasjoner
        //        .First(o => o.Beskyttelsesutstyrliste
        //            .Any(u => u.Feilbruktyper
        //                .Any() == false));
        //    var utstyrSomSkalEndres =
        //        observasjonSomSkalEndres.Beskyttelsesutstyrliste.First();
        //    var feilbrukTypeSomSkalLeggesTil = Mapper.Map<FeilbrukType>(DatabaseContext.FeilbrukType.First());
        //    utstyrSomSkalEndres.Feilbruktyper.Add(feilbrukTypeSomSkalLeggesTil);
            
            
        //    var handler = new OppdaterBeskyttelsesutstyrObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterBeskyttelsesutstyrObservasjon.Handler>());
        //    await handler.Handle(new OppdaterBeskyttelsesutstyrObservasjon.Command()
        //    {
        //        Observasjon = observasjonSomSkalEndres
        //    }, CancellationToken.None);
            
        //    var hentetSesjonFraDatabaseEtterEndring = await HentSesjon(opprettetSesjonId);
            
            
        //    var oppdatertObservasjon = hentetSesjonFraDatabaseEtterEndring.Observasjoner
        //        .FirstOrDefault(o => o.Id == observasjonSomSkalEndres.Id);
            
        //    var oppdatertUtstyr = oppdatertObservasjon.Beskyttelsesutstyrliste
        //        .First(b => b.Id == utstyrSomSkalEndres.Id);
            
        //    // Assert
        //    Assert.That(oppdatertUtstyr.Feilbruktyper.Select(f => f.Id), Contains.Item(feilbrukTypeSomSkalLeggesTil.Id));
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

        protected async Task<Guid> OpprettSesjon(Domene.Sted.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institusjon.Avdeling>(
                avdeling ?? DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institusjon.First(x => x.Id == avdelingModell.InstitusjonId);
            var settingTyper = DatabaseContext.BeskyttelsesutstyrsettingType.ToList();
            var utstyrsTyper = DatabaseContext.BeskyttelsesutstyrType.ToList();

            var lagreBeskyttelsesutstyrSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var beskyttelsesutstyrSesjonGuid = await lagreBeskyttelsesutstyrSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new BeskyttelsesutstyrSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Navn,
                    InstitusjonId = institusjon.Id,
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.UtcNow,
                    Observasjoner = new List<BeskyttelsesutstyrObservasjon>()
                    {
                        new BeskyttelsesutstyrObservasjon()
                        {
                            Id = observasjonId.ToString(),
                            SesjonId = sesjonId.ToString(),
                            Kommentar = "Observasjon kommentar",
                            Registrerttidspunkt = DateTime.UtcNow,
                            Rolle = avdelingModell.Roller.First(),
                            Settingtype = new BeskyttelsesutstyrsettingType()
                            {
                                Id = settingTyper.First(x => x.Kode == BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte).Id,
                            },
                            Beskyttelsesutstyrliste = new List<Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr>()
                            {
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "I",
                                    BleBenyttet = true,
                                    BleBenyttetRiktig = false,
                                    ErIndikert = true,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Hansker).Id,
                                    },
                                    Feilbruktyper = new List<FeilbrukType>
                                    {
                                        new FeilbrukType
                                        {
                                            Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Hansker).Feilbruktyper[0].Id,
                                            ErValgt = true
                                        }
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "II",
                                    BleBenyttet = true,
                                    BleBenyttetRiktig = true,
                                    ErIndikert = true,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Smittefrakk).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "III",
                                    BleBenyttet = true,
                                    BleBenyttetRiktig = true,
                                    ErIndikert = false,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Stellefrakk).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "IV",
                                    BleBenyttet = true,
                                    BleBenyttetRiktig = true,
                                    ErIndikert = false,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Munnbind).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "V",
                                    BleBenyttet = true,
                                    BleBenyttetRiktig = false,
                                    ErIndikert = false,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Hette).Id,
                                    },
                                    Feilbruktyper = new List<FeilbrukType>
                                    {
                                        new FeilbrukType
                                        {
                                            Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Hette).Feilbruktyper[0].Id,
                                            ErValgt = true
                                        },
                                        new FeilbrukType
                                        {
                                            Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Hette).Feilbruktyper[1].Id,
                                            ErValgt = true
                                        }
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "VI",
                                    BleBenyttet = false,
                                    BleBenyttetRiktig = false,
                                    ErIndikert = false,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "VII",
                                    BleBenyttet = false,
                                    BleBenyttetRiktig = false,
                                    ErIndikert = false,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Andedrettsvern).Id,
                                    }
                                },
                                new Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr()
                                {
                                    Kommentar = "VIII",
                                    BleBenyttet = false,
                                    BleBenyttetRiktig = false,
                                    ErIndikert = false,
                                    Utstyrstype = new BeskyttelsesutstyrType()
                                    {
                                        Id = utstyrsTyper.First(x => x.Kode == BeskyttelsesutstyrTypeKonstanter.Plastforkle).Id,
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
            var eksisterendeTyper = DatabaseContext.BeskyttelsesutstyrType.Select(x => x.Id).ToList();
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
                UtstyrType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType()
                {
                    Id = opprettetBeskyttelsesutstyrType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterBeskyttelsesutstyrTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetBeskyttelsesutstyrType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.UtstyrType.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.UtstyrType.Kode));
                Assert.That(resultatOppdater.Kode, Is.EqualTo(opprettetBeskyttelsesutstyrType.Kode));
            });
        }

        [Test]
        public void OppdaterBeskyttelsesutstyrType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterBeskyttelsesutstyrTypeHandler = new OppdaterBeskyttelsesutstyrType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterBeskyttelsesutstyrType.Command()
            {
                UtstyrType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
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
            var eksisterendeTyper = DatabaseContext.BeskyttelsesutstyrsettingType.Select(x => x.Id).ToList();
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
                SettingType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType()
                {
                    Id = opprettetBeskyttelsesutstyrsettingType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterBeskyttelsesutstyrsettingTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetBeskyttelsesutstyrsettingType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.SettingType.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.SettingType.Kode));
                Assert.That(resultatOppdater.Kode, Is.EqualTo(opprettetBeskyttelsesutstyrsettingType.Kode));
            });
        }

        [Test]
        public void OppdaterBeskyttelsesutstyrsettingType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterBeskyttelsesutstyrsettingTypeHandler = new OppdaterBeskyttelsesutstyrsettingType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterBeskyttelsesutstyrsettingType.Command()
            {
                SettingType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
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
            var eksisterendeFeilbrukTyperForUtstyr = DatabaseContext.BeskyttelsesutstyrType
                .Include(x => x.Feilbruktyper)
                .First().Feilbruktyper
                .Select(x => x.Id)
                .ToList();
            var hentFeilbrukTyper = new HentFeilbrukTyper.Handler(DatabaseContext, Mapper);
            var query = new HentFeilbrukTyper.Query() { UtstyrTypeId = DatabaseContext.BeskyttelsesutstyrType.First().Id };

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
            var utstyrtype = DatabaseContext.BeskyttelsesutstyrType.First();
            var opprettetFeilbrukType = await OpprettFeilbrukType(navn: navn, utstyrtypeId: utstyrtype.Id);
            var opprettetFeilbrukTypeFraDatabase = DatabaseContext.FeilbrukType
                .FirstOrDefault(a => a.Id == opprettetFeilbrukType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetFeilbrukType.Id, Is.GreaterThan(0));
                Assert.That(opprettetFeilbrukType.Navn, Is.EqualTo(navn));
                Assert.That(opprettetFeilbrukType.Navn, Is.EqualTo(opprettetFeilbrukTypeFraDatabase.Navn));
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
            var utstyrtype = DatabaseContext.BeskyttelsesutstyrType.First();
            var opprettetFeilbrukType = await OpprettFeilbrukType(utstyrtypeId: utstyrtype.Id);
            var oppdaterFeilbrukTypeHandler = new OppdaterFeilbrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterFeilbrukType.Command()
            {
                FeilbrukType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.FeilbrukType()
                {
                    Id = opprettetFeilbrukType.Id,
                    Navn = "Da Vinci",
                },
                UtstyrTypeId = utstyrtype.Id
            };

            // Act
            var resultatOppdater = await oppdaterFeilbrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetFeilbrukType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.FeilbrukType.Navn));
            });
        }

        [Test]
        public void OppdaterFeilbrukType_IkkeEksisterendeUtstyrTypeId()
        {
            // Arrange
            var oppdaterFeilbrukTypeHandler = new OppdaterFeilbrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterFeilbrukType.Command()
            {
                FeilbrukType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.FeilbrukType()
                {
                    Id = DatabaseContext.FeilbrukType.First().Id,
                    Navn = "Da Vinci",
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
            var utstyrtype = DatabaseContext.BeskyttelsesutstyrType.First();
            var oppdaterFeilbrukTypeHandler = new OppdaterFeilbrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterFeilbrukType.Command()
            {
                FeilbrukType = new Modeller.V1.Observasjon.Beskyttelsesutstyr.FeilbrukType()
                {
                    Id = 123456789,
                    Navn = "Da Vinci",
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

        private async Task<Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType> OpprettBeskyttelsesutstyrType(string kode = null)
        {
            var beskyttelsesutstyrType = new Domene.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.BeskyttelsesutstyrType.Add(beskyttelsesutstyrType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType>(beskyttelsesutstyrType);
        }

        private async Task<Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType> OpprettBeskyttelsesutstyrsettingType(string kode = null)
        {
            var beskyttelsesutstyrsettingType = new Domene.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.BeskyttelsesutstyrsettingType.Add(beskyttelsesutstyrsettingType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType>(beskyttelsesutstyrsettingType);
        }

        private async Task<Modeller.V1.Observasjon.Beskyttelsesutstyr.FeilbrukType> OpprettFeilbrukType(string navn = null, int utstyrtypeId = 0)
        {
            var opprettFeilbrukTypeHandler = new OpprettFeilbrukType.Handler(DatabaseContext, Mapper);
            var opprettCommand = new OpprettFeilbrukType.Command()
            {
                FeilbrukType = new OpprettFeilbrukTypeRequest() { Navn = navn ?? "Test" },
                UtstyrTypeId = utstyrtypeId != 0 ? utstyrtypeId : DatabaseContext.BeskyttelsesutstyrType.First().Id
            };

            var resOpprett = await opprettFeilbrukTypeHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}
