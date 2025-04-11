using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Handsmykke;
using HyFive.Tjenester.Sesjon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Tests.Handsmykke
{
    public class HandsmykkeTests : TjenesteTests
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
        //    var avdeling = DatabaseContext.Department.Include(x => x.Institution).Include(x => x.Role).First();
        //    var opprettetSesjonGuid = await OpprettSesjon(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observations.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].HandJewelry.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observations[0].Role.Name, Is.EqualTo(avdeling.Role.First().Name));
        //    });
        //}

        private async Task<HandsmykkeSesjon> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentHentHandsmykkeSesjonHandler = new HentHandsmykkeSesjon.Handler(DatabaseContext, Mapper, BrukerService);
            var handsmykkeSesjon = await hentHentHandsmykkeSesjonHandler.Handle(new HentHandsmykkeSesjon.Query()
            {
                HPRNummer = hprnummer,
                SesjonId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return handsmykkeSesjon;
        }

        private async Task<Guid> OpprettSesjon(Domain.Place.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institution.Department>(avdeling ?? DatabaseContext.Department.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institution.First(x => x.Id == avdelingModell.InstitusjonId);
            var handsmykkeTyper = DatabaseContext.HandJewelryType.ToList();

            var lagreHandsmykkeSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var handsmykkeSesjonGuid = await lagreHandsmykkeSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new HandsmykkeSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Name,
                    InstitusjonId = institusjon.Id,
                    Observasjoner = new List<HandsmykkeObservasjon>()
                    {
                        new HandsmykkeObservasjon()
                        {
                            Id = observasjonId.ToString(),
                            Kommentar = "Observasjon kommentar",
                            Registrerttidspunkt = DateTime.Now,
                            Rolle = avdelingModell.Roller.First(),
                            SesjonId = sesjonId.ToString(),
                            Handsmykker = new List<HandJewelryType>()
                            {
                                new HandJewelryType()
                                {
                                    Id = handsmykkeTyper.FirstOrDefault(x => x.Code == HandsmykkeTypeKonstanter.KlokkeArmband).Id
                                }
                            }
                        }
                    },
                    Kommentar = "Sesjon kommentar",
                    Starttidspunkt = DateTime.Now
                },
                HPRNummer = hprnummer
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
            var hentHandsmykkeTyper = new HentHandsmykkeTyper.Handler(DatabaseContext, Mapper);
            var query = new HentHandsmykkeTyper.Query();

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
            var hentHandsmykkeTypeHandler = new HentHandsmykkeType.Handler(DatabaseContext, Mapper);
            var query = new HentHandsmykkeType.Query() { Id = opprettetHandsmykkeType.Id };

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
            var hentHandsmykkeTypeHandler = new HentHandsmykkeType.Handler(DatabaseContext, Mapper);
            var query = new HentHandsmykkeType.Query() { Id = 123456789 };

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
            var oppdaterHandsmykkeTypeHandler = new OppdaterHandsmykkeType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHandsmykkeType.Command()
            {
                Handsmykketype = new Modeller.V1.Observasjon.HandJewelryType()
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
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.Handsmykketype.Name));
                Assert.That(resultatOppdater.Code, Is.Not.EqualTo(oppdaterCommand.Handsmykketype.Code));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetHandsmykkeType.Code));
            });
        }

        [Test]
        public void OppdaterHandsmykkeType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHandsmykkeTypeHandler = new OppdaterHandsmykkeType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHandsmykkeType.Command()
            {
                Handsmykketype = new Modeller.V1.Observasjon.HandJewelryType()
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

        private async Task<Modeller.V1.Observasjon.HandJewelryType> OpprettHandsmykkeType(string kode = null)
        {
            var handsmykkeType = new Domain.Observation.HandJewelryType() { Code = kode ?? "TEST", Name = "test" };
            DatabaseContext.HandJewelryType.Add(handsmykkeType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.HandJewelryType>(handsmykkeType);
        }

        #endregion
    }
}