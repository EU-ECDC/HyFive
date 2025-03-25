using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.FireIndikasjoner;
using HyFive.Tjenester.FireIndikasjoner.Helpers;
using HyFive.Tjenester.Sesjon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace HyFive.Tjenester.Tests.FireIndikasjoner
{
    public class FireIndikasjonerTests : TjenesteTests
    {
        private Guid _sesjonId = Guid.NewGuid();
        private Guid _observasjonId = Guid.NewGuid();
        private readonly string _hprnummer = "9383840";

        #region FireIndikasjonerSesjon

        //[Test]
        //public async Task LagreSesjonTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonGuid = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("minst en indikasjontype"),
        //        async () =>
        //        {
        //            await OpprettFireIndikasjonerSesjon(
        //                _sesjonId, _observasjonId, avdeling, _hprnummer,
        //                indikasjontyper: new List<IndikasjonType>());
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_IngenAktivitet_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("Aktivitet må registreres"),
        //        async () =>
        //        {
        //            await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                aktivitet: null, brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_IngenAktivitetType_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("AktivitetType mangler"),
        //        async () =>
        //        {
        //            await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                aktivitet: new Aktivitet(), brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_TidtakingUtfortTrueMenManglerTid_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("ingen tid ble registrert"),
        //        async () =>
        //        {
        //            await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                aktivitet: new Aktivitet()
        //                {
        //                    AktivitetType = Mapper.Map<Modeller.V1.Observasjon.AktivitetType>(DatabaseContext.AktivitetType.AsNoTracking().First()),
        //                    TidtakingBleUtfort = true
        //                }, brukDefaultAktivitet: false);
        //        }
        //    );
        //}

        //[Test]
        //public void LagreSesjon_ManglerRolle_KasterException()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("Rolle må registreres"),
        //        async () =>
        //        {
        //            await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer,
        //                rolle: null, brukDefaultRolle: false);
        //        }
        //    );
        //}

        //[Test]
        //public async Task HentSesjonTest()
        //{
        //    //Arrange and act
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonGuid = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var hentetSesjonFraDatabase = await HentSesjon(opprettetSesjonGuid);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(hentetSesjonFraDatabase?.Id, Is.Not.Null);
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Indikasjonstyper.Count, Is.EqualTo(1));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Aktivitet.AktivitetType.Kode, Is.EqualTo(AktivitetTypeKonstanter.Handvask));
        //        Assert.That(hentetSesjonFraDatabase.Observasjoner[0].Rolle.Navn, Is.EqualTo(avdeling.Roller.First().Navn));
        //    });
        //}

        private async Task<FireIndikasjonerSesjon> HentSesjon(Guid sesjonGuidFraRequestGuid)
        {
            var hentFireIndikasjonSesjonHandler = new HentFireIndikasjonerSesjon.Handler(DatabaseContext, Mapper, BrukerService);
            var fireIndikasjonSesjon = await hentFireIndikasjonSesjonHandler.Handle(new HentFireIndikasjonerSesjon.Query()
            {
                HPRNummer = _hprnummer,
                SesjonId = sesjonGuidFraRequestGuid
            }, CancellationToken.None);

            return fireIndikasjonSesjon;
        }

        #endregion

        #region OppdaterObservasjon

        //[Test]
        //public async Task OppdaterObservasjon_Test()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var observasjon = opprettetSesjon.Observasjoner.First();
        //    observasjon.Kommentar = "Oppdatert";
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = observasjon };

        //    // Act
        //    await handler.Handle(command, CancellationToken.None);
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First(x => x.Id == observasjon.Id);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(oppdatertObservasjon.Id, Is.EqualTo(observasjon.Id));
        //        Assert.That(oppdatertObservasjon.Kommentar, Is.EqualTo(observasjon.Kommentar));
        //    });
        //}

        //[Test]
        //public async Task OppdaterObservasjon_IkkeEksisterendeObservasjonId_KasterException()
        //{
        //    //Arrange
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First();
        //    oppdatertObservasjon.Id = new Guid().ToString();
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = oppdatertObservasjon };

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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First();
        //    oppdatertObservasjon.Indikasjonstyper.Clear();
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("minst en indikasjontype"),
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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First();
        //    oppdatertObservasjon.Aktivitet = null;
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("Aktivitet må registreres"),
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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First();
        //    oppdatertObservasjon.Aktivitet = new Aktivitet();
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("AktivitetType mangler"),
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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First();
        //    oppdatertObservasjon.Aktivitet = new Aktivitet()
        //    {
        //        AktivitetType = Mapper.Map<Modeller.V1.Observasjon.AktivitetType>(DatabaseContext.AktivitetType.AsNoTracking().First()),
        //        TidtakingBleUtfort = true
        //    };
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("ingen tid ble registrert"),
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
        //    var avdeling = DatabaseContext.Avdeling.Include(x => x.Institusjon).Include(x => x.Roller).First();
        //    var opprettetSesjonId = await OpprettFireIndikasjonerSesjon(_sesjonId, _observasjonId, avdeling, _hprnummer);
        //    var opprettetSesjon = await HentSesjon(opprettetSesjonId);
        //    var handler = new OppdaterFireIndikasjonerObservasjon.Handler(DatabaseContext, Mapper, new NullLogger<OppdaterFireIndikasjonerObservasjon.Handler>());
        //    var oppdatertObservasjon = opprettetSesjon.Observasjoner.First();
        //    oppdatertObservasjon.Rolle = null;
        //    var command = new OppdaterFireIndikasjonerObservasjon.Command() { Observasjon = oppdatertObservasjon };

        //    // Act and Assert
        //    Assert.ThrowsAsync(
        //        Is.TypeOf<FireIndikasjonerObservasjonValidationException>().And.Message.Contains("Rolle må registreres"),
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
            var eksisterendeTyper = DatabaseContext.AktivitetType.Select(x => x.Id).ToList();
            var hentAktivitetTyper = new HentAktivitetTyper.Handler(DatabaseContext, Mapper);
            var query = new HentAktivitetTyper.Query();

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
            var oppdaterAktivitetTypeHandler = new OppdaterAktivitetType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterAktivitetType.Command()
            {
                Aktivitettype = new Modeller.V1.Observasjon.AktivitetType()
                {
                    Id = opprettetAktivitetType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterAktivitetTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetAktivitetType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.Aktivitettype.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.Aktivitettype.Kode));
            });
        }

        [Test]
        public void OppdaterAvdelingType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterAktivitetTypeHandler = new OppdaterAktivitetType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterAktivitetType.Command()
            {
                Aktivitettype = new Modeller.V1.Observasjon.AktivitetType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
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
            var eksisterendeTyper = DatabaseContext.IndikasjonType.Select(x => x.Id).ToList();
            var hentIndikasjonTyper = new HentIndikasjonstyper.Handler(DatabaseContext, Mapper);
            var query = new HentIndikasjonstyper.Query();

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
            var oppdaterIndikasjonTypeHandler = new OppdaterIndikasjonstype.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterIndikasjonstype.Command()
            {
                Indikasjonstype = new Modeller.V1.Observasjon.IndikasjonType()
                {
                    Id = opprettetIndikasjonType.Id,
                    Kode = "DV",
                    Navn = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterIndikasjonTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetIndikasjonType.Id));
                Assert.That(resultatOppdater.Navn, Is.EqualTo(oppdaterCommand.Indikasjonstype.Navn));
                Assert.That(resultatOppdater.Kode, Is.Not.EqualTo(oppdaterCommand.Indikasjonstype.Kode));
            });
        }

        [Test]
        public void OppdaterIndikasjonType_IkkeEksisterendeId()
        {
            // Arrange
            var oppdaterIndikasjonTypeHandler = new OppdaterIndikasjonstype.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterIndikasjonstype.Command()
            {
                Indikasjonstype = new Modeller.V1.Observasjon.IndikasjonType()
                {
                    Id = 99999999,
                    Kode = "DV",
                    Navn = "Da Vinci",
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

        private async Task<Modeller.V1.Observasjon.AktivitetType> OpprettAktivitetType(string kode = null)
        {
            var aktivitetType = new Domene.Observasjon.AktivitetType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.AktivitetType.Add(aktivitetType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.AktivitetType>(aktivitetType);
        }

        private async Task<Modeller.V1.Observasjon.IndikasjonType> OpprettIndikasjonType(string kode = null)
        {
            var indikasjonType = new Domene.Observasjon.IndikasjonType() { Kode = kode ?? "TEST", Navn = "test" };
            DatabaseContext.IndikasjonType.Add(indikasjonType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Modeller.V1.Observasjon.IndikasjonType>(indikasjonType);
        }

        #endregion
    }
}
