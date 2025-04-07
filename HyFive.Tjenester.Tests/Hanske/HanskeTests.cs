using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon.Hansker;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Hanske;
using HyFive.Tjenester.Sesjon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HyFive.Tjenester.Tests.Hanske
{
    public class HanskeTests : TjenesteTests
    {
        private Guid sesjonId = Guid.NewGuid();
        private Guid observasjonId = Guid.NewGuid();
        private readonly string hprnummer = "9383840";

        #region HanskeSesjon

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonGuid = await OpprettSesjonMedIndikasjonTyper();
        //    var opprettetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonGuid = await OpprettSesjonMedIndikasjonTyper(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].HanskeMedIndikasjonTyper, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].HanskeMedIndikasjonTyper.Count, Is.EqualTo(2));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].BenyttetHanske, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].HandhygieneEtterHanskebrukType.Kode,
        //            Is.EqualTo(HandhygieneEtterHanskebrukTypeKonstanter.Ja));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Rolle.Navn, Is.EqualTo(avdeling.Roller.First().Navn));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonUtenIndikasjonTyperTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonGuid = await OpprettSesjonUtenIndikasjonTyper(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].HanskeUtenIndikasjonTyper, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].HanskeUtenIndikasjonTyper.Count, Is.EqualTo(2));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].BenyttetHanske, Is.True);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].HandhygieneEtterHanskebrukType.Kode,
        //            Is.EqualTo(HandhygieneEtterHanskebrukTypeKonstanter.Nei));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Rolle.Navn, Is.EqualTo(avdeling.Roller.First().Navn));
        //    });
        //}

        private async Task<HanskeSesjon> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentHentHanskeSesjonHandler = new HentHanskeSesjon.Handler(DatabaseContext, Mapper, BrukerService);
            var handsmykkeSesjon = await hentHentHanskeSesjonHandler.Handle(new HentHanskeSesjon.Query()
            {
                HPRNummer = hprnummer,
                SesjonId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return handsmykkeSesjon;
        }

        private async Task<Guid> OpprettSesjonMedIndikasjonTyper(Domene.Sted.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var lagreHanskeSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var avdelingModell = Mapper.Map<Modeller.V1.Institusjon.Avdeling>(
                avdeling ?? DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institusjon.First(x => x.Id == avdelingModell.InstitusjonId);
            var hanskeMedIndikasjonTyper = DatabaseContext.HanskeMedIndikasjonType.ToList();
            var handhygieneEtterHanskebrukTyper = DatabaseContext.HandhygieneEtterHanskebrukType.ToList();

            var hanskeSesjonGuid = await lagreHanskeSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new HanskeSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Navn,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<HanskeObservasjon>()
                    {
                        new HanskeObservasjon()
                        {
                            Id = observasjonId.ToString(),
                            Kommentar = "Observasjon kommentar",
                            Registrerttidspunkt = DateTime.UtcNow,
                            Rolle = avdelingModell.Roller.First(),
                            SesjonId = sesjonId.ToString(),
                            HanskeMedIndikasjonTyper = new List<HanskeMedIndikasjonType>()
                            {
                                new HanskeMedIndikasjonType()
                                {
                                    ErValgt = true,
                                    Id = hanskeMedIndikasjonTyper.FirstOrDefault(x => x.Kode == HanskeMedIndikasjonTypeKonstanter.Smitte).Id
                                },
                                new HanskeMedIndikasjonType()
                                {
                                    ErValgt = true,
                                    Id = hanskeMedIndikasjonTyper.FirstOrDefault(x => x.Kode == HanskeMedIndikasjonTypeKonstanter.Kroppvaesker).Id
                                }
                            },
                            BenyttetHanske = true,
                            HandhygieneEtterHanskebrukType = new HandhygieneEtterHanskebrukType()
                            {
                                Id = handhygieneEtterHanskebrukTyper.FirstOrDefault(x => x.Kode == HandhygieneEtterHanskebrukTypeKonstanter.Ja).Id
                            }
                        }
                    },
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.UtcNow
                },
                HPRNummer = hprnummer
            }, CancellationToken.None);

            return hanskeSesjonGuid;
        }

        private async Task<Guid> OpprettSesjonUtenIndikasjonTyper(Domene.Sted.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var lagreHanskeSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var avdelingModell = Mapper.Map<Modeller.V1.Institusjon.Avdeling>(
                avdeling ?? DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institusjon.First(x => x.Id == avdelingModell.InstitusjonId);
            var hanskeUtenIndikasjonTyper = DatabaseContext.HanskeUtenIndikasjonType.ToList();
            var handhygieneEtterHanskebrukTyper = DatabaseContext.HandhygieneEtterHanskebrukType.ToList();

            var hanskeSesjonGuid = await lagreHanskeSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new HanskeSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Navn,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<HanskeObservasjon>()
                    {
                        new HanskeObservasjon()
                        {
                            Id = observasjonId.ToString(),
                            Kommentar = "Observasjon kommentar",
                            Registrerttidspunkt = DateTime.UtcNow,
                            Rolle = avdelingModell.Roller.First(),
                            SesjonId = sesjonId.ToString(),
                            HanskeUtenIndikasjonTyper = new List<HanskeUtenIndikasjonType>()
                            {
                                new HanskeUtenIndikasjonType()
                                {
                                    ErValgt = true,
                                    Id = hanskeUtenIndikasjonTyper.FirstOrDefault(x => x.Kode == HanskeUtenIndikasjonTypeKonstanter.Mat).Id
                                },
                                new HanskeUtenIndikasjonType()
                                {
                                    ErValgt = true,
                                    Id = hanskeUtenIndikasjonTyper.FirstOrDefault(x => x.Kode == HanskeUtenIndikasjonTypeKonstanter.StellUtenKroppvaesker).Id
                                }
                            },
                            BenyttetHanske = true,
                            HandhygieneEtterHanskebrukType = new HandhygieneEtterHanskebrukType()
                            {
                                Id = handhygieneEtterHanskebrukTyper.FirstOrDefault(x => x.Kode == HandhygieneEtterHanskebrukTypeKonstanter.Nei).Id
                            }
                        }
                    },
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.UtcNow
                },
                HPRNummer = hprnummer
            }, CancellationToken.None);

            return hanskeSesjonGuid;
        }

        #endregion

        #region HanskeMedIndikasjonType

        [Test]
        public async Task HentHanskeMedIndikasjonTyper_Test()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.HanskeMedIndikasjonType.Select(x => x.Id).ToList();
            var hentHanskeMedIndikasjonTyper = new HentHanskeMedIndikasjonTyper.Handler(DatabaseContext, Mapper);
            var query = new HentHanskeMedIndikasjonTyper.Query();

            // Act
            var res = await hentHanskeMedIndikasjonTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterHanskeMedIndikasjonType_Test()
        {
            // Arrange
            var opprettetHanskeMedIndikasjonType = await OpprettHanskeMedIndikasjonType();
            var oppdaterHanskeMedIndikasjonTypeHandler = new OppdaterHanskeMedIndikasjonType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHanskeMedIndikasjonType.Command()
            {
                HanskeMedIndikasjonType = new Modeller.V1.Observasjon.Hansker.HanskeMedIndikasjonType()
                {
                    Id = opprettetHanskeMedIndikasjonType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHanskeMedIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHanskeMedIndikasjonType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.HanskeMedIndikasjonType.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.HanskeMedIndikasjonType.Kode));
                Assert.That(resultatOppdater.Kode, Is.EqualTo(opprettetHanskeMedIndikasjonType.Kode));
            });
        }

        [Test]
        public void OppdaterHanskeMedIndikasjonType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHanskeMedIndikasjonTypeHandler = new OppdaterHanskeMedIndikasjonType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHanskeMedIndikasjonType.Command()
            {
                HanskeMedIndikasjonType = new Modeller.V1.Observasjon.Hansker.HanskeMedIndikasjonType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke hanskeMedIndikasjonType"),
                async () =>
                {
                    await oppdaterHanskeMedIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region HanskeUtenIndikasjonType

        [Test]
        public async Task HentHanskeUtenIndikasjonTyper_Test()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.HanskeUtenIndikasjonType.Select(x => x.Id).ToList();
            var hentHanskeUtenIndikasjonTyper = new HentHanskeUtenIndikasjonTyper.Handler(DatabaseContext, Mapper);
            var query = new HentHanskeUtenIndikasjonTyper.Query();

            // Act
            var res = await hentHanskeUtenIndikasjonTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterHanskeUtenIndikasjonType_Test()
        {
            // Arrange
            var opprettetHanskeUtenIndikasjonType = await OpprettHanskeUtenIndikasjonType();
            var oppdaterHanskeUtenIndikasjonTypeHandler = new OppdaterHanskeUtenIndikasjonType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHanskeUtenIndikasjonType.Command()
            {
                HanskeUtenIndikasjonType = new Modeller.V1.Observasjon.Hansker.HanskeUtenIndikasjonType()
                {
                    Id = opprettetHanskeUtenIndikasjonType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHanskeUtenIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHanskeUtenIndikasjonType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.HanskeUtenIndikasjonType.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.HanskeUtenIndikasjonType.Kode));
                Assert.That(resultatOppdater.Kode, Is.EqualTo(opprettetHanskeUtenIndikasjonType.Kode));
            });
        }

        [Test]
        public void OppdaterHanskeUtenIndikasjonType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHanskeUtenIndikasjonTypeHandler = new OppdaterHanskeUtenIndikasjonType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHanskeUtenIndikasjonType.Command()
            {
                HanskeUtenIndikasjonType = new Modeller.V1.Observasjon.Hansker.HanskeUtenIndikasjonType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke hanskeUtenIndikasjonType"),
                async () =>
                {
                    await oppdaterHanskeUtenIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region HandhygieneEtterHanskebrukType

        [Test]
        public async Task HentHandhygieneEtterHanskebrukTyper_Test()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.HandhygieneEtterHanskebrukType.Select(x => x.Id).ToList();
            var hentHandhygieneEtterHanskebrukTyper = new HentHandhygieneEtterHanskebrukTyper.Handler(DatabaseContext, Mapper);
            var query = new HentHandhygieneEtterHanskebrukTyper.Query();

            // Act
            var res = await hentHandhygieneEtterHanskebrukTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterHandhygieneEtterHanskebrukType_Test()
        {
            // Arrange
            var opprettetHandhygieneEtterHanskebrukType = await OpprettHandhygieneEtterHanskebrukType();
            var oppdaterHandhygieneEtterHanskebrukTypeHandler = new OppdaterHandhygieneEtterHanskebrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHandhygieneEtterHanskebrukType.Command()
            {
                HandhygieneEtterHanskebrukType = new Modeller.V1.Observasjon.Hansker.HandhygieneEtterHanskebrukType()
                {
                    Id = opprettetHandhygieneEtterHanskebrukType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHandhygieneEtterHanskebrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHandhygieneEtterHanskebrukType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.HandhygieneEtterHanskebrukType.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.HandhygieneEtterHanskebrukType.Kode));
                Assert.That(resultatOppdater.Kode, Is.EqualTo(opprettetHandhygieneEtterHanskebrukType.Kode));
            });
        }

        [Test]
        public void OppdaterHandhygieneEtterHanskebrukType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHandhygieneEtterHanskebrukTypeHandler = new OppdaterHandhygieneEtterHanskebrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHandhygieneEtterHanskebrukType.Command()
            {
                HandhygieneEtterHanskebrukType = new Modeller.V1.Observasjon.Hansker.HandhygieneEtterHanskebrukType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke handhygieneEtterHanskebrukType"),
                async () =>
                {
                    await oppdaterHandhygieneEtterHanskebrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Modeller.V1.Observasjon.Hansker.HanskeMedIndikasjonType> OpprettHanskeMedIndikasjonType(string kode = null)
        {
            var hanskeMedIndikasjonType = new Domene.Observasjon.Hansker.HanskeMedIndikasjonType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.HanskeMedIndikasjonType.Add(hanskeMedIndikasjonType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Hansker.HanskeMedIndikasjonType>(hanskeMedIndikasjonType);
        }

        private async Task<Modeller.V1.Observasjon.Hansker.HanskeUtenIndikasjonType> OpprettHanskeUtenIndikasjonType(string kode = null)
        {
            var hanskeUtenIndikasjonType = new Domene.Observasjon.Hansker.HanskeUtenIndikasjonType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.HanskeUtenIndikasjonType.Add(hanskeUtenIndikasjonType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Hansker.HanskeUtenIndikasjonType>(hanskeUtenIndikasjonType);
        }

        private async Task<Modeller.V1.Observasjon.Hansker.HandhygieneEtterHanskebrukType> OpprettHandhygieneEtterHanskebrukType(string kode = null)
        {
            var handhygieneEtterHanskebrukType = new Domene.Observasjon.Hansker.HandhygieneEtterHanskebrukType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.HandhygieneEtterHanskebrukType.Add(handhygieneEtterHanskebrukType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.Hansker.HandhygieneEtterHanskebrukType>(handhygieneEtterHanskebrukType);
        }

        #endregion
    }
}