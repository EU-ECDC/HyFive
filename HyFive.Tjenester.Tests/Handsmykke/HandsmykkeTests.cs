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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonGuid = await OpprettSesjon(avdeling);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Handsmykker.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Rolle.Navn, Is.EqualTo(avdeling.Roller.First().Navn));
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

        private async Task<Guid> OpprettSesjon(Domene.Sted.Avdeling avdeling = null)
        {
            var logger = new Mock<ILogger<LagreSesjon.Handler>>();

            var avdelingModell = Mapper.Map<Modeller.V1.Institusjon.Avdeling>(avdeling ?? DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First());
            var institusjon = DatabaseContext.Institusjon.First(x => x.Id == avdelingModell.InstitusjonId);
            var handsmykkeTyper = DatabaseContext.HandsmykkeType.ToList();

            var lagreHandsmykkeSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);
            var handsmykkeSesjonGuid = await lagreHandsmykkeSesjonHandler.Handle(new LagreSesjon.Command()
            {
                Sesjon = new HandsmykkeSesjon()
                {
                    Id = sesjonId.ToString(),
                    Avdeling = avdelingModell,
                    Institusjonsnavn = institusjon.Navn,
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
                            Handsmykker = new List<HandsmykkeType>()
                            {
                                new HandsmykkeType()
                                {
                                    Id = handsmykkeTyper.FirstOrDefault(x => x.Kode == HandsmykkeTypeKonstanter.KlokkeArmband).Id
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
            var eksisterendeTyper = DatabaseContext.HandsmykkeType.Where(x => x.ErAktiv).Select(x => x.Id).ToList();
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
                Assert.That(hentHandsmykkeTypeResultat, Has.Property(nameof(HandsmykkeType.Kode)).EqualTo(opprettetHandsmykkeType.Kode));
                Assert.That(hentHandsmykkeTypeResultat, Has.Property(nameof(HandsmykkeType.Navn)).EqualTo(opprettetHandsmykkeType.Navn));
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
                Handsmykketype = new Modeller.V1.Observasjon.HandsmykkeType()
                {
                    Id = opprettetHandsmykkeType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterHandsmykkeTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetHandsmykkeType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.Handsmykketype.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.Handsmykketype.Kode));
                Assert.That(resultatOppdater.Kode, Is.EqualTo(opprettetHandsmykkeType.Kode));
            });
        }

        [Test]
        public void OppdaterHandsmykkeType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterHandsmykkeTypeHandler = new OppdaterHandsmykkeType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterHandsmykkeType.Command()
            {
                Handsmykketype = new Modeller.V1.Observasjon.HandsmykkeType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
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

        private async Task<Modeller.V1.Observasjon.HandsmykkeType> OpprettHandsmykkeType(string kode = null)
        {
            var handsmykkeType = new Domene.Observasjon.HandsmykkeType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.HandsmykkeType.Add(handsmykkeType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.HandsmykkeType>(handsmykkeType);
        }

        #endregion
    }
}