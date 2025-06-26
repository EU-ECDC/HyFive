using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.HandJewelry;
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

namespace HyFive.Services.Tests.Handsmykke
{
    public class HandsmykkeTests : ServiceTests
    {
        private Guid sesjonId = Guid.NewGuid();
        private Guid observasjonId = Guid.NewGuid();
        private readonly string hprnummer = "9383840";

        #region HandsmykkeSesjon

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var opprettetSesjonGuid = await OpprettSesjon();
        //    var opprettetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    //Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(opprettetSesjonFraDatabase, Is.Not.Null);
        //        Assert.That(opprettetSesjonFraDatabase.Id, Is.EqualTo(opprettetSesjonGuid.ToString()));
        //    });
        //}

        //[Test]
        //public async Task HentSesjonTest()
        //{
        //    //Arrange and act
        //    var department = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First();
        //    var opprettetSesjonGuid = await OpprettSesjon(department);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].HandJewelries.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Roles.Name, Is.EqualTo(department.Roles.First().Name));
        //    });
        //}

        private async Task<HandJewelrySession> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentHentHandsmykkeSesjonHandler = new GetHandJewelrySession.Handler(DatabaseContext, Mapper, UserService);
            var handsmykkeSesjon = await hentHentHandsmykkeSesjonHandler.Handle(new GetHandJewelrySession.Query()
            {
                HPRNumber = hprnummer,
                SessionId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return handsmykkeSesjon;
        }

        private async Task<Guid> OpprettSesjon(Domain.Place.Department department = null)
        {
            var logger = new Mock<ILogger<SaveSession.Handler>>();

            var avdelingModell = Mapper.Map<Models.V1.Institution.Department>(department ?? DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Roles).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitutionId);
            var handsmykkeTyper = DatabaseContext.HandJewelryType.ToList();

            var lagreHandsmykkeSesjonHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);
            var handsmykkeSesjonGuid = await lagreHandsmykkeSesjonHandler.Handle(new SaveSession.Command()
            {
                Session = new HandJewelrySession()
                {
                    Id = sesjonId.ToString(),
                    Department = avdelingModell,
                    InstitutionsName = institusjon.Name,
                    InstitutionId = institusjon.Id,
                    Observations = new List<HandJewelryObservation>()
                    {
                        new HandJewelryObservation()
                        {
                            Id = observasjonId.ToString(),
                            Comment = "Observasjon kommentar",
                            RegistrationTime = DateTime.UtcNow,
                            Role = avdelingModell.Roles.First(),
                            SessionId = sesjonId.ToString(),
                            HandJewelries = new List<HandJewelryType>()
                            {
                                new HandJewelryType()
                                {
                                    Id = handsmykkeTyper.FirstOrDefault(x => x.Code == HandJewelryTypeConstants.WatchBracelet).Id
                                }
                            }
                        }
                    },
                    Comment = "Sesjon kommentar",
                    CreatedDate = DateTime.UtcNow
                },
                HprNumber = hprnummer
            }, CancellationToken.None);

            return handsmykkeSesjonGuid;
        }

        #endregion

        #region HandsmykkeType

        [Test]
        public async Task HentHandsmykkeTyper_Test()
        {
            // Arrange
            var eksisterendeTyper = DatabaseContext.HandJewelryType.Where(x => x.IsActive).Select(x => x.Id).ToList();
            var hentHandsmykkeTyper = new GetHandJewelryTypes.Handler(DatabaseContext, Mapper);
            var query = new GetHandJewelryTypes.Query();

            // Act
            var res = await hentHandsmykkeTyper.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(eksisterendeTyper.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(eksisterendeTyper.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task HentHandsmykkeType_Test()
        {
            // Arrange
            var opprettetHandsmykkeType = await OpprettHandsmykkeType();
            var hentHandsmykkeTypeHandler = new GetHandJewelryType.Handler(DatabaseContext, Mapper);
            var query = new GetHandJewelryType.Query() { Id = opprettetHandsmykkeType.Id };

            // Act
            var hentHandsmykkeTypeResultat = await hentHandsmykkeTypeHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(hentHandsmykkeTypeResultat, Is.Not.Null);
                Assert.That(hentHandsmykkeTypeResultat, Has.Property(nameof(HandJewelryType.Code)).EqualTo(opprettetHandsmykkeType.Code));
                Assert.That(hentHandsmykkeTypeResultat, Has.Property(nameof(HandJewelryType.Name)).EqualTo(opprettetHandsmykkeType.Name));
            });
        }

        [Test]
        public async Task HentHandsmykkeType_IdEksistererIkke_ReturnererNull()
        {
            // Arrange
            var opprettetHandsmykkeType = await OpprettHandsmykkeType();
            var hentHandsmykkeTypeHandler = new GetHandJewelryType.Handler(DatabaseContext, Mapper);
            var query = new GetHandJewelryType.Query() { Id = 123456789 };

            // Act
            var hentHandsmykkeTypeResultat = await hentHandsmykkeTypeHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(hentHandsmykkeTypeResultat, Is.Null);
            });
        }

        [Test]
        public async Task OppdaterHandsmykkeTypeTest()
        {
            // Arrange
            var opprettetHandsmykkeType = await OpprettHandsmykkeType();
            var oppdaterHandsmykkeTypeHandler = new UpdateHandJewelryType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateHandJewelryType.Command()
            {
                HandJewelryType = new Models.V1.Observation.HandJewelryType()
                {
                    Id = opprettetHandsmykkeType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHandsmykkeTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHandsmykkeType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.HandJewelryType.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.HandJewelryType.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetHandsmykkeType.Code));
            });
        }

        [Test]
        public void OppdaterHandsmykkeType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHandsmykkeTypeHandler = new UpdateHandJewelryType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateHandJewelryType.Command()
            {
                HandJewelryType = new Models.V1.Observation.HandJewelryType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke handsmykketype"),
                async () =>
                {
                    await oppdaterHandsmykkeTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.HandJewelryType> OpprettHandsmykkeType(string kode = null)
        {
            var handsmykkeType = new Domain.Observation.HandJewelryType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.HandJewelryType.Add(handsmykkeType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.HandJewelryType>(handsmykkeType);
        }

        #endregion
    }
}