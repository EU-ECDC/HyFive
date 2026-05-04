using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.FiveIndication;
using HyFive.Services.FiveIndication.Helpers;
using HyFive.Services.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.FiveIndications
{
    public class FiveIndicationsTests : ServiceTests
    {
        private Guid _sessionId = Guid.NewGuid();
        private Guid _observationId = Guid.NewGuid();
        private readonly string _hprnumber = "9383840";

        #region FiveIndicationsSession

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("minst en indikasjontype"),
        //        async () =>
        //        {
        //            await CreateFiveIndicatorsSession(
        //                _sessionId, _observationId, avdeling, _hprnumber,
        //                indikasjontyper: new List<IndicationTypes>());
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_IngenAktivitet_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("Activity må registreres"),
        //        async () =>
        //        {
        //            await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber,
        //                aktivitet: null, brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_IngenAktivitetType_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("ActivityType mangler"),
        //        async () =>
        //        {
        //            await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber,
        //                aktivitet: new Activity(), brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_TidtakingUtfortTrueMenManglerTid_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("ingen tid ble registrert"),
        //        async () =>
        //        {
        //            await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber,
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FiveIndicatorsObservationValidationException>().And.Message.Contains("Roles må registreres"),
        //        async () =>
        //        {
        //            await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber,
        //                rolle: null, brukDefaultRolle: false);
        //        }
        //    );
        //}

        //[Test]
        //public async Task HentSesjonTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var hentetSesjonFraDatabase = await GetSession(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].IndicationTypes.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Activity.ActivityType.Code, Is.EqualTo(AktivitetTypeKonstanter.Handwash));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.City, Is.EqualTo(avdeling.Roles.First().City));
        //    });
        //}

        private async Task<FiveIndicationsSession> GetSession(Guid sessionGuidFromRequestGuid)
        {
            var getFiveIndicationsSessionHandler = new GetFiveIndicationsSession.Handler(DatabaseContext, Mapper);
            var fiveIndicationsSession = await getFiveIndicationsSessionHandler.Handle(new GetFiveIndicationsSession.Query()
            {
                Email = _hprnumber,
                SessionId = sessionGuidFromRequestGuid
            }, CancellationToken.None);

            return fiveIndicationsSession;
        }

        #endregion

        #region UpdateObservation

        //[Test]
        //public async Task OppdaterObservasjon_Test()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Unit).Include(x => x.Roles).First();
        //    var opprettetSesjonId = await CreateFiveIndicatorsSession(_sessionId, _observationId, avdeling, _hprnumber);
        //    var opprettetSesjon = await GetSession(opprettetSesjonId);
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

        #region DeleteObservation

        // TODO

        #endregion

        #region ActivityType

        [Test]
        public async Task GetActivityTypesTest()
        {
            // Arrange
            if (!await DatabaseContext.ActivityType.AnyAsync())
            {
                DatabaseContext.ActivityType.AddRange(
                    new Domain.Observation.ActivityType { Name = "Hand hygiene before patient contact", Code = "Act1" },
                    new Domain.Observation.ActivityType { Name = "After patient contact", Code = "Act2" }
                );
                await DatabaseContext.SaveChangesAsync();
            }


            var existingTypes = await DatabaseContext.ActivityType.Select(x => x.Id).ToListAsync();
            var getActivityTypes = new GetActivityTypes.Handler(DatabaseContext, Mapper);
            var query = new GetActivityTypes.Query();

            // Act
            var res = await getActivityTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(x => x.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateActivityTypeTest()
        {
            // Arrange
            var createdActivityType = await CreatedActivityType();
            var updateActivityTypeHandler = new UpdateActivityType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateActivityType.Command()
            {
                ActivityType = new Models.V1.Observation.ActivityType()
                {
                    Id = createdActivityType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateActivityTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdActivityType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.ActivityType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.ActivityType.Code));
            });
        }

        [Test]
        public void UpdateDepartmentType_NonExistentId()
        {
            // Arrange
            var updateActivityTypeHandler = new UpdateActivityType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateActivityType.Command()
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
                Is.TypeOf<DomainException>().And.Message.Contains("ActivityTypeNotFound"),
                async () =>
                {
                    await updateActivityTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region IndicationType

        [Test]
        public async Task GetIndicationTypesTest()
        {
            // Arrange
            if (!await DatabaseContext.IndicationTypes.AnyAsync())
            {
                DatabaseContext.IndicationTypes.AddRange(
                    new Domain.Observation.IndicationTypes { Name = "Before patient contact", Code="ind1", Number = "1" },
                    new Domain.Observation.IndicationTypes { Name = "After patient contact", Code = "ind2", Number = "2" },
                    new Domain.Observation.IndicationTypes { Name = "After exposure to body fluids", Code = "ind3", Number = "3" }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.IndicationTypes.Select(x => x.Id).ToListAsync();
            var getIndicationTypes = new GetIndicationTypes.Handler(DatabaseContext, Mapper);
            var query = new GetIndicationTypes.Query();

            // Act
            var res = await getIndicationTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateIndicationTypeTest()
        {
            // Arrange
            var createIndicationType = await CreateIndicationType();
            var updateIndicationTypeHandler = new UpdateIndicationType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateIndicationType.Command()
            {
                IndicationType = new Models.V1.Observation.IndicationType()
                {
                    Id = createIndicationType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateIndicationTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createIndicationType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.IndicationType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.IndicationType.Code));
            });
        }

        [Test]
        public void UpdateIndicationType_NonExistentId()
        {
            // Arrange
            var updateIndicationTypeHandler = new UpdateIndicationType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateIndicationType.Command()
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
                Is.TypeOf<DomainException>().And.Message.Contains("IndicationTypeNotFound"),
                async () =>
                {
                    await updateIndicationTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.ActivityType> CreatedActivityType(string code = null)
        {
            var activityType = new Domain.Observation.ActivityType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.ActivityType.Add(activityType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ActivityType>(activityType);
        }

        private async Task<Models.V1.Observation.IndicationType> CreateIndicationType(string code = null)
        {
            var indicationType = new Domain.Observation.IndicationTypes() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.IndicationTypes.Add(indicationType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.IndicationType>(indicationType);
        }

        #endregion
    }
}
