using HyFive.Modeller.V1.Institusjon;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Institusjon;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Tests.Institusjon
{
    public class InstitusjonTests : TjenesteTests
    {
        [Test]
        public async Task OpprettOgHentInstitusjonTest()
        {
            // Arrange
            var institusjon = (await OpprettInstitusjon()).Item1;
            var hentInstitusjonHandler = new HentInstitusjon.Handler(DatabaseContext, Mapper);
            var query = new HentInstitusjon.Query() { InstitusjonId = institusjon.Id };

            // Act
            var res = await hentInstitusjonHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.Forkortelse, Is.EqualTo(institusjon.Forkortelse));
                Assert.That(res.Navn, Is.EqualTo(institusjon.Navn));
                Assert.That(res.HERId, Is.EqualTo(institusjon.HERId));
            });
        }

        [Test]
        public async Task HentInstitusjonerTest()
        {
            // Arrange
            var institusjon = (await OpprettInstitusjon()).Item1;

            var hentInstitusjonerHandler = new HentInstitusjoner.Handler(DatabaseContext, Mapper);
            var query = new HentInstitusjoner.Query() { };

            // Act
            var res = await hentInstitusjonerHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.Select(r => r.Id).ToList(), Contains.Item(institusjon.Id));
                Assert.That(res.Count(), Is.EqualTo(DatabaseContext.Institusjon.ToList().Count));
            });
        }

        [Test]
        public async Task HentInstitusjonerForKoordinatorTest()
        {
            // Arrange
            var hentInstitusjonerForKoordinatorHandler =
                new HentInstitusjonerForKoordinator.Handler(DatabaseContext, Mapper);

            // Act
            var institusjoner = await hentInstitusjonerForKoordinatorHandler.Handle(
                new HentInstitusjonerForKoordinator.Query()
                {
                    KoordinatorHprNummer = Seed.SeedKoordinatorHprNummer
                }, CancellationToken.None);


            // Assert
            Assert.That(institusjoner.Length, Is.GreaterThan(0));

        }

        [Test]
        public async Task HentInstitusjonstyperTest()
        {
            // Arrange
            var handler = new HentInstitusjonstyper.Handler(DatabaseContext, Mapper);
            var eksisterendeTypeKoder = DatabaseContext.InstitusjonType.Select(i => i.Kode).ToList();

            // Act
            var typer = await handler.Handle(new HentInstitusjonstyper.Query(), CancellationToken.None);

            // Assert
            Assert.That(eksisterendeTypeKoder.OrderBy(x => x).SequenceEqual(typer.OrderBy(x => x.Kode).Select(t => t.Kode)));
        }

        [Test]
        public async Task HentKoordinatorerForInstitusjonTest()
        {
            // Arrange
            (var institusjon, _) = await OpprettInstitusjon();
            var handler = new HentKoordinatorerForInstitusjon.Handler(DatabaseContext, Mapper);
            var antallKoordinatorerTilknyttetInstitusjon =
                DatabaseContext.Koordinator
                    .Include(k => k.Institusjon)
                    .Count(k => k.Institusjon.Id == institusjon.Id);

            // Act
            var koordinatorer =
                await handler.Handle(new HentKoordinatorerForInstitusjon.Query() { InstitusjonId = institusjon.Id },
                    CancellationToken.None);

            // Assert
            Assert.That(koordinatorer.ToList(), Has.Count.EqualTo(antallKoordinatorerTilknyttetInstitusjon));
        }


        [Test]
        public async Task HentObservatorerForInstitusjonTest()
        {
            // Arrange
            (var institusjon, var observator) = await OpprettInstitusjon();
            var handler = new HentObservatorerForInstitusjon.Handler(DatabaseContext, Mapper);
            var antallObservatorerTilknyttetInstitusjon =
                DatabaseContext.Observator
                    .Include(k => k.Institusjon)
                    .Count(k => k.Institusjon.Id == institusjon.Id);

            // Act
            var observatorer =
                await handler.Handle(new HentObservatorerForInstitusjon.Query() { InstitusjonId = institusjon.Id },
                    CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(observatorer.ToList(), Has.Count.EqualTo(antallObservatorerTilknyttetInstitusjon));
                Assert.That(observatorer.Select(o => o.Id).ToList(), Contains.Item(observator.Id));
            });
        }

        [Test]
        public async Task OpprettOgHentPredefinerteKommentarerTest()
        {
            // Arrange
            var hentKommentarerHandler = new HentPredefinerteKommentarer.Handler(DatabaseContext);
            var institusjon = (await new HentInstitusjonerForKoordinator.Handler(DatabaseContext, Mapper).Handle(
                new HentInstitusjonerForKoordinator.Query()
                { KoordinatorHprNummer = Seed.SeedKoordinatorHprNummer }, CancellationToken.None)).First();

            var opprettKommentarerHandler = new OpprettPredefinertKommentar.Handler(DatabaseContext);

            var kommentarRequest = new OpprettPredefinertKommentarRequest()
            {
                Kommentar = $"{Guid.NewGuid()}"
            };

            // Act

            var kunneOppretteKommentar = await opprettKommentarerHandler.Handle(
                new OpprettPredefinertKommentar.Command() { NyPredefinertKommentar = kommentarRequest, Institusjonid = institusjon.Id }, CancellationToken.None);

            var kommentarer = await hentKommentarerHandler.Handle(new HentPredefinerteKommentarer.Query()
            {
                InstitusjonId = institusjon.Id,
                Sesjontype = SesjonType.Beskyttelsesutstyr
            }, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(kunneOppretteKommentar);
                Assert.That(kommentarer.Any);
                Assert.That(kommentarer, Contains.Item(kommentarRequest.Kommentar));
            });

        }

        [Test]
        public async Task OppdaterInstitusjonTest()
        {
            // Arrange
            (var institusjon, _) = await OpprettInstitusjon();
            var oppdaterInstitusjonHandler = new OppdaterInstitusjon.Handler(DatabaseContext, Mapper);
            var nyttNavn = $"Et nytt navn og en tilfeldig verdi:{Guid.NewGuid()}";
            institusjon.Navn = nyttNavn;

            // Act
            await oppdaterInstitusjonHandler.Handle(new OppdaterInstitusjon.Command()
            { Institusjon = institusjon }, CancellationToken.None);

            // Assert
            Assert.That(DatabaseContext.Institusjon.First(i => i.Id == institusjon.Id).Navn, Is.EqualTo(nyttNavn));

        }

        [Test]
        public async Task OppdaterInstitusjonstypeTest()
        {
            // Arrange
            var oppdaterInstitusjonstypeHandler = new OppdaterInstitusjonstype.Handler(DatabaseContext, Mapper);
            var opprinneligType = DatabaseContext.InstitusjonType.First();
            var nyttNavn = $"NAVN{Guid.NewGuid()}";

            // Act
            await oppdaterInstitusjonstypeHandler.Handle(new OppdaterInstitusjonstype.Command()
            {
                Institusjonstype = new InstitusjonType()
                {
                    Id = opprinneligType.Id,
                    Kode = "KODE",
                    Navn = nyttNavn
                }
            }, CancellationToken.None);

            // Assert
            var typeEtterOppdatering = DatabaseContext.InstitusjonType.First(k => k.Id == opprinneligType.Id);
            Assert.That(typeEtterOppdatering.Navn, Is.EqualTo(nyttNavn));
            Assert.That(typeEtterOppdatering.Kode, Is.EqualTo(opprinneligType.Kode));
        }

        [Test]
        public async Task OpprettInstitusjonTest()
        {
            // Arrange and Act
            (var opprettetInstitusjon, _) = await OpprettInstitusjon();
            var opprettetInstitusjonFraDatabase = DatabaseContext.Institusjon.Include(i => i.Avdelinger).FirstOrDefault(i => i.Id == opprettetInstitusjon.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetInstitusjonFraDatabase, Is.Not.Null);
                Assert.That(opprettetInstitusjonFraDatabase.Navn, Is.EqualTo(opprettetInstitusjon.Navn));
                Assert.That(opprettetInstitusjonFraDatabase.Forkortelse, Is.EqualTo(opprettetInstitusjon.Forkortelse));
            });

        }

        [Test]
        public async Task OpprettInstitusjonstypeTest()
        {
            // Arrange and Act
            var opprettetType = await OpprettInstitusjonType();
            var opprettetTypeFraDatabase = DatabaseContext.InstitusjonType.First(i => i.Id == opprettetType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetTypeFraDatabase.Navn == opprettetType.Navn);
                Assert.That(opprettetTypeFraDatabase.Kode == opprettetType.Kode);
            });
        }

        [Test]
        public async Task OpprettInstitusjonstype_EksisterendeKode_KasterException()
        {
            // Arrange
            var opprettetType = await OpprettInstitusjonType(kode: "KODE");

            // Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await OpprettInstitusjonType(kode: "KODE");
            });
        }

        #region Helper-methods

        private async Task<Modeller.V1.Institusjon.InstitusjonType> OpprettInstitusjonType(string kode = null)
        {
            var opprettInstitusjonTypeHandler = new OpprettInstitusjonstype.Handler(DatabaseContext, Mapper);
            var opprettCommand = new OpprettInstitusjonstype.Command()
            {
                Institusjonstype = new OpprettInstitusjonstypeRequest()
                {
                    Kode = kode ?? "TEST",
                    Navn = "Test"
                }
            };

            var resOpprett = await opprettInstitusjonTypeHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion

    }
}