using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using HyFive.Modeller.V1.Constants;
using HyFive.Modeller.V1.Observation.Gloves;
using HyFive.Modeller.V1.Session;
using HyFive.Services.Glove;
using HyFive.Services.Sesjon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HyFive.Services.Tests.Hanske
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Role).First();
        //    var opprettetSesjonGuid = await OpprettSesjonMedIndikasjonTyper(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].IndicatedGloveTypes, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].IndicatedGloveTypes.Count, Is.EqualTo(2));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GloveUsed, Is.EqualTo(true));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].PostGloveHandHygiene.Code,
        //            Is.EqualTo(HandhygieneEtterHanskebrukTypeKonstanter.Ja));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Role.Name, Is.EqualTo(avdeling.Role.First().Name));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonUtenIndikasjonTyperTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Role).First();
        //    var opprettetSesjonGuid = await OpprettSesjonUtenIndikasjonTyper(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GeneralPurposeGloveTypes, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GeneralPurposeGloveTypes.Count, Is.EqualTo(2));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].GloveUsed, Is.True);
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].PostGloveHandHygiene.Code,
        //            Is.EqualTo(HandhygieneEtterHanskebrukTypeKonstanter.Nei));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Role.Name, Is.EqualTo(avdeling.Role.First().Name));
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

        private async Task<Guid> OpprettSesjonMedIndikasjonTyper(Domain.Place.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var lagreHanskeSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var avdelingModell = Mapper.Map<Modeller.V1.Institution.Department>(
                avdeling ?? DatabaseContext.Department.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitusjonId);
            var hanskeMedIndikasjonTyper = DatabaseContext.IndicatedGloveType.ToList();
            var handhygieneEtterHanskebrukTyper = DatabaseContext.PostGloveHandHygiene.ToList();

            var hanskeSesjonGuid = await lagreHanskeSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new HanskeSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Name,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<GloveObservation>()
                    {
                        new GloveObservation()
                        {
                            Id = observasjonId.ToString(),
                            Comment = "Observasjon kommentar",
                            RegistrationTime = DateTime.Now,
                            Role = avdelingModell.Role.First(),
                            SessionId = sesjonId.ToString(),
                            IndicatedGloveTypes = new List<IndicatedGloveType>()
                            {
                                new IndicatedGloveType()
                                {
                                    IsSelected = true,
                                    Id = hanskeMedIndikasjonTyper.FirstOrDefault(x => x.Code == GloveWithIndicationTypeConstants.Infection).Id
                                },
                                new IndicatedGloveType()
                                {
                                    IsSelected = true,
                                    Id = hanskeMedIndikasjonTyper.FirstOrDefault(x => x.Code == GloveWithIndicationTypeConstants.BodyFluids).Id
                                }
                            },
                            BenyttetHanske = true,
                            PostGloveHandHygieneType = new PostGloveHandHygieneType()
                            {
                                Id = handhygieneEtterHanskebrukTyper.FirstOrDefault(x => x.Code == HandHygieneAfterGloveUseTypeConstants.Yes).Id
                            }
                        }
                    },
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.Now
                },
                HPRNummer = hprnummer
            }, CancellationToken.None);

            return hanskeSesjonGuid;
        }

        private async Task<Guid> OpprettSesjonUtenIndikasjonTyper(Domain.Place.Department department = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var lagreHanskeSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var avdelingModell = Mapper.Map<Modeller.V1.Institution.Department>(
                avdeling ?? DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Role).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitutionId);
            var hanskeUtenIndikasjonTyper = DatabaseContext.GeneralPurposeGloveType.ToList();
            var handhygieneEtterHanskebrukTyper = DatabaseContext.PostGloveHandHygiene.ToList();

            var hanskeSesjonGuid = await lagreHanskeSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new HanskeSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Name,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<GloveObservation>()
                    {
                        new GloveObservation()
                        {
                            Id = observasjonId.ToString(),
                            Comment = "Observasjon kommentar",
                            RegistrationTime = DateTime.Now,
                            Role = avdelingModell.Roller.First(),
                            SessionId = sesjonId.ToString(),
                            GeneralPurposeGloveTypes = new List<GeneralPurposeGloveType>()
                            {
                                new GeneralPurposeGloveType()
                                {
                                    IsSelected = true,
                                    Id = hanskeUtenIndikasjonTyper.FirstOrDefault(x => x.Code == GloveWithoutIndicationTypeConstants.Food).Id
                                },
                                new GeneralPurposeGloveType()
                                {
                                    IsSelected = true,
                                    Id = hanskeUtenIndikasjonTyper.FirstOrDefault(x => x.Code == GloveWithoutIndicationTypeConstants.CareWithoutBodyFluids).Id
                                }
                            },
                            BenyttetHanske = true,
                            PostGloveHandHygieneType = new PostGloveHandHygieneType()
                            {
                                Id = handhygieneEtterHanskebrukTyper.FirstOrDefault(x => x.Code == HandHygieneAfterGloveUseTypeConstants.No).Id
                            }
                        }
                    },
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.Now
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
            var eksisterendeTyper = DatabaseContext.IndicatedGloveType.Select(x => x.Id).ToList();
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
                HanskeMedIndikasjonType = new Modeller.V1.Observation.Gloves.IndicatedGloveType()
                {
                    Id = opprettetHanskeMedIndikasjonType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHanskeMedIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHanskeMedIndikasjonType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.HanskeMedIndikasjonType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.HanskeMedIndikasjonType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetHanskeMedIndikasjonType.Code));
            });
        }

        [Test]
        public void OppdaterHanskeMedIndikasjonType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHanskeMedIndikasjonTypeHandler = new OppdaterHanskeMedIndikasjonType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHanskeMedIndikasjonType.Command()
            {
                HanskeMedIndikasjonType = new Modeller.V1.Observation.Gloves.IndicatedGloveType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
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
            var eksisterendeTyper = DatabaseContext.GeneralPurposeGloveType.Select(x => x.Id).ToList();
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
                HanskeUtenIndikasjonType = new Modeller.V1.Observation.Gloves.GeneralPurposeGloveType()
                {
                    Id = opprettetHanskeUtenIndikasjonType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHanskeUtenIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHanskeUtenIndikasjonType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.HanskeUtenIndikasjonType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.HanskeUtenIndikasjonType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetHanskeUtenIndikasjonType.Code));
            });
        }

        [Test]
        public void OppdaterHanskeUtenIndikasjonType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHanskeUtenIndikasjonTypeHandler = new OppdaterHanskeUtenIndikasjonType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHanskeUtenIndikasjonType.Command()
            {
                HanskeUtenIndikasjonType = new Modeller.V1.Observation.Gloves.GeneralPurposeGloveType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
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
            var eksisterendeTyper = DatabaseContext.PostGloveHandHygiene.Select(x => x.Id).ToList();
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
                HandhygieneEtterHanskebrukType = new Modeller.V1.Observation.Gloves.PostGloveHandHygieneType()
                {
                    Id = opprettetHandhygieneEtterHanskebrukType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHandhygieneEtterHanskebrukTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHandhygieneEtterHanskebrukType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.HandhygieneEtterHanskebrukType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.HandhygieneEtterHanskebrukType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetHandhygieneEtterHanskebrukType.Code));
            });
        }

        [Test]
        public void OppdaterHandhygieneEtterHanskebrukType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHandhygieneEtterHanskebrukTypeHandler = new OppdaterHandhygieneEtterHanskebrukType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHandhygieneEtterHanskebrukType.Command()
            {
                HandhygieneEtterHanskebrukType = new Modeller.V1.Observation.Gloves.PostGloveHandHygieneType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
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

        private async Task<Modeller.V1.Observation.Gloves.IndicatedGloveType> OpprettHanskeMedIndikasjonType(string kode = null)
        {
            var hanskeMedIndikasjonType = new Domain.Observation.Gloves.IndicatedGloveType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.IndicatedGloveType.Add(hanskeMedIndikasjonType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observation.Gloves.IndicatedGloveType>(hanskeMedIndikasjonType);
        }

        private async Task<Modeller.V1.Observation.Gloves.GeneralPurposeGloveType> OpprettHanskeUtenIndikasjonType(string kode = null)
        {
            var hanskeUtenIndikasjonType = new Domain.Observation.Gloves.GeneralPurposeGloveType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.GeneralPurposeGloveType.Add(hanskeUtenIndikasjonType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observation.Gloves.GeneralPurposeGloveType>(hanskeUtenIndikasjonType);
        }

        private async Task<Modeller.V1.Observation.Gloves.PostGloveHandHygieneType> OpprettHandhygieneEtterHanskebrukType(string kode = null)
        {
            var handhygieneEtterHanskebrukType = new Domain.Observation.Gloves.PostGloveHandHygiene() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.PostGloveHandHygiene.Add(handhygieneEtterHanskebrukType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observation.Gloves.PostGloveHandHygieneType>(handhygieneEtterHanskebrukType);
        }

        #endregion
    }
}