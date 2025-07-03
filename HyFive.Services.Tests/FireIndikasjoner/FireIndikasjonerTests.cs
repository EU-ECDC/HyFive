using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.FiveIndication;
using HyFive.Services.FiveIndication.Helpers;
using HyFive.Services.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace HyFive.Services.Tests.FireIndikasjoner
{
    public class FireIndikasjonerTests : ServiceTests
    {
        private Guid _sesjonId = Guid.NewGuid();
        private Guid _observasjonId = Guid.NewGuid();
        private readonly string _hprnummer = "9383840";

        #region FireIndikasjonerSesjon

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    //Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(opprettetSesjonFraDatabase, Is.Not.Null);
        //        Assert.That(opprettetSesjonFraDatabase.Id, Is.EqualTo(opprettetSesjonGuid.ToString()));
        //    });
        //}

        //[Test]
        //public void LagreSesjon_IngenIndikasjonTyper_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("minst en indikasjontype"),
        //        async () =>
        //        {
        //            await CreateFourIndicatorsSession(
        //                _sesjonId, _observasjonId, avdeling, _hprnummer,
        //                indikasjontyper: new List<IndicationTypes>());
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_IngenAktivitet_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("Activity må registreres"),
        //        async () =>
        //        {
        //            await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                aktivitet: null, brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_IngenAktivitetType_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("ActivityType mangler"),
        //        async () =>
        //        {
        //            await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                aktivitet: new Activity(), brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_TidtakingUtfortTrueMenManglerTid_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("ingen tid ble registrert"),
        //        async () =>
        //        {
        //            await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                aktivitet: new Activity()
        //                {
        //                    ActivityType = Mapper.Map<Models.V1.Observation.ActivityType>(DatabaseContext.ActivityType.AsNoTracking().First()),
        //                    TimingWasPerformed = true
        //                }, brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_ManglerRolle_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("Roles må registreres"),
        //        async () =>
        //        {
        //            await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                rolle: null, brukDefaultRolle: false);
        //        }
        //    );
        //}

        //[Test]
        //public async Task HentSesjonTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].IndicationTypes.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Activity.ActivityType.Code, Is.EqualTo(AktivitetTypeKonstanter.Handwash));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(avdeling.Roles.First().Name));
        //    });
        //}

        private async Task<FiveIndicationsSession> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentFireIndikasjonSesjonHandler = new GetFiveIndicationsSession.Handler(DatabaseContext, Mapper, UserService);
            var fireIndikasjonSesjon = await hentFireIndikasjonSesjonHandler.Handle(new GetFiveIndicationsSession.Query()
            {
                HPRNumber = _hprnummer,
                SessionId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return fireIndikasjonSesjon;
        }

        #endregion

        #region OppdaterObservasjon

        //[Test]
        //public async Task OppdaterObservasjon_Test()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var observasjon = opprettetSesjon.Observations.First();
        //    observasjon.Municipality = "Oppdatert";
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = observasjon };

        //    // Act
        //    await handler.Handle(command, CancellationToken.None);
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First(x => x.Id == observasjon.Id);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(oppdatertObservasjon.Id, Is.EqualTo(observasjon.Id));
        //        Assert.That(oppdatertObservasjon.Municipality, Is.EqualTo(observasjon.Municipality));
        //    });
        //}

        //[Test]
        //public async Task OppdaterObservasjon_IkkeEksisterendeObservasjonId_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First();
        //    oppdatertObservasjon.Id = new Guid().ToString();
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<Exception>().And.Message.Contains("ikke finne observasjon med ID"),
        //        async () =>
        //        {
        //            await handler.Handle(command, CancellationToken.None);
        //        }
        //    );
        //}

        //[Test]
        //public async Task OppdaterObservasjon_IngenIndikasjonTyper_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First();
        //    oppdatertObservasjon.IndicationTypes.Clear();
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("minst en indikasjontype"),
        //        async () =>
        //        {
        //            await handler.Handle(command, CancellationToken.None);
        //        }
        //    );
        //}

        //[Test]
        //public async Task OppdaterObservasjon_IngenAktivitet_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First();
        //    oppdatertObservasjon.Activity = null;
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("Activity må registreres"),
        //        async () =>
        //        {
        //            await handler.Handle(command, CancellationToken.None);
        //        }
        //    );
        //}

        //[Test]
        //public async Task OppdaterObservasjon_IngenAktivitetType_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First();
        //    oppdatertObservasjon.Activity = new Activity();
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("ActivityType mangler"),
        //        async () =>
        //        {
        //            await handler.Handle(command, CancellationToken.None);
        //        }
        //    );
        //}

        //[Test]
        //public async Task OppdaterObservasjon_TidtakingUtfortTrueMenManglerTid_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First();
        //    oppdatertObservasjon.Activity = new Activity()
        //    {
        //        ActivityType = Mapper.Map<Models.V1.Observation.ActivityType>(DatabaseContext.ActivityType.AsNoTracking().First()),
        //        TimingWasPerformed = true
        //    };
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("ingen tid ble registrert"),
        //        async () =>
        //        {
        //            await handler.Handle(command, CancellationToken.None);
        //        }
        //    );
        //}

        //[Test]
        //public async Task OppdaterObservasjon_ManglerRolle_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFourIndicatorsSession(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new UpdateFiveIndicationsObservation.Handler(DatabaseContext, Mapper, new NullLogger<UpdateFiveIndicationsObservation.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observations.First();
        //    oppdatertObservasjon.Roles = null;
        //    var command = new UpdateFiveIndicationsObservation.Command() { Observation = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("Roles må registreres"),
        //        async () =>
        //        {
        //            await handler.Handle(command, CancellationToken.None);
        //        }
        //    );
        //}

        #endregion

        #region SlettObservasjon

        // TODO

        #endregion

        #region AktivitetType

        [Test]
        public async Task HentAktivitetTyperTest()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.ActivityType.Select(x => x.Id).ToList();
            var hentAktivitetTyper = new GetActivityTypes.Handler(DatabaseContext, Mapper);
            var query = new GetActivityTypes.Query();

            // Act
            var res = await hentAktivitetTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(x => x.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterAktivitetTypeTest()
        {
            // Arrange
            var opprettetAktivitetType = await OpprettAktivitetType();
            var oppdaterAktivitetTypeHandler = new UpdateActivityType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateActivityType.Command()
            {
                ActivityType = new Models.V1.Observation.ActivityType()
                {
                    Id = opprettetAktivitetType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterAktivitetTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetAktivitetType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.ActivityType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.ActivityType.Code));
            });
        }

        [Test]
        public void OppdaterAvdelingType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterAktivitetTypeHandler = new UpdateActivityType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateActivityType.Command()
            {
                ActivityType = new Models.V1.Observation.ActivityType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke aktivitettype"),
                async () =>
                {
                    await oppdaterAktivitetTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region IndikasjonType

        [Test]
        public async Task HentIndikasjonTyperTest()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.IndicationTypes.Select(x => x.Id).ToList();
            var hentIndikasjonTyper = new GetIndicationTypes.Handler(DatabaseContext, Mapper);
            var query = new GetIndicationTypes.Query();

            // Act
            var res = await hentIndikasjonTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task OppdaterIndikasjonTypeTest()
        {
            // Arrange
            var opprettetIndikasjonType = await OpprettIndikasjonType();
            var oppdaterIndikasjonTypeHandler = new UpdateIndicationType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateIndicationType.Command()
            {
                IndicationType = new Models.V1.Observation.IndicationType()
                {
                    Id = opprettetIndikasjonType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetIndikasjonType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.IndicationType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.IndicationType.Code));
            });
        }

        [Test]
        public void OppdaterIndikasjonType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterIndikasjonTypeHandler = new UpdateIndicationType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateIndicationType.Command()
            {
                IndicationType = new Models.V1.Observation.IndicationType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke indikasjontype"),
                async () =>
                {
                    await oppdaterIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.ActivityType> OpprettAktivitetType(string kode = null)
        {
            var aktivitetType = new Domain.Observation.ActivityType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.ActivityType.Add(aktivitetType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ActivityType>(aktivitetType);
        }

        private async Task<Models.V1.Observation.IndicationType> OpprettIndikasjonType(string kode = null)
        {
            var indikasjonType = new Domain.Observation.IndicationTypes() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.IndicationTypes.Add(indikasjonType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.IndicationType>(indikasjonType);
        }

        #endregion
    }
}
