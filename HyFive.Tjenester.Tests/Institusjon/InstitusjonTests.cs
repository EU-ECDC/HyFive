using HyFive.Modeller.V1.Institution;
using HyFive.Modeller.V1.Session;
using HyFive.Services.Institusjon;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.Institusjon
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
                Assert.That(res.Abbreviation, Is.EqualTo(institusjon.Abbreviation));
                Assert.That(res.Name, Is.EqualTo(institusjon.Name));
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
                Assert.That(res.Count(), Is.EqualTo(DatabaseContext.Institution.ToList().Count));
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
            var eksisterendeTypeKoder = DatabaseContext.InstitutionType.Select(i => i.Code).ToList();

            // Act
            var typer = await handler.Handle(new HentInstitusjonstyper.Query(), CancellationToken.None);

            // Assert
            Assert.That(eksisterendeTypeKoder.OrderBy(x => x).SequenceEqual(typer.OrderBy(x => x.Code).Select(t => t.Code)));
        }

        [Test]
        public async Task HentKoordinatorerForInstitusjonTest()
        {
            // Arrange
            (var institusjon, _) = await OpprettInstitusjon();
            var handler = new HentKoordinatorerForInstitusjon.Handler(DatabaseContext, Mapper);
            var antallKoordinatorerTilknyttetInstitusjon =
                DatabaseContext.Coordinator
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
                DatabaseContext.Observer
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

            var kommentarRequest = new CreatePredefinedCommentRequest()
            {
                Comment = $"{Guid.NewGuid()}"
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
                Assert.That(kommentarer, Contains.Item(kommentarRequest.Comment));
            });

        }

        [Test]
        public async Task OppdaterInstitusjonTest()
        {
            // Arrange
            (var institusjon, _) = await OpprettInstitusjon();
            var oppdaterInstitusjonHandler = new OppdaterInstitusjon.Handler(DatabaseContext, Mapper);
            var nyttNavn = $"Et nytt navn og en tilfeldig verdi:{Guid.NewGuid()}";
            institusjon.Name = nyttNavn;

            // Act
            await oppdaterInstitusjonHandler.Handle(new OppdaterInstitusjon.Command()
            { Institusjon = institusjon }, CancellationToken.None);

            // Assert
            Assert.That(DatabaseContext.Institution.First(i => i.Id == institusjon.Id).Name, Is.EqualTo(nyttNavn));

        }

        [Test]
        public async Task OppdaterInstitusjonstypeTest()
        {
            // Arrange
            var oppdaterInstitusjonstypeHandler = new OppdaterInstitusjonstype.Handler(DatabaseContext, Mapper);
            var opprinneligType = DatabaseContext.InstitutionType.First();
            var nyttNavn = $"NAVN{Guid.NewGuid()}";

            // Act
            await oppdaterInstitusjonstypeHandler.Handle(new OppdaterInstitusjonstype.Command()
            {
                Institusjonstype = new InstitutionType()
                {
                    Id = opprinneligType.Id,
                    Code = "KODE",
                    Name = nyttNavn
                }
            }, CancellationToken.None);

            // Assert
            var typeEtterOppdatering = DatabaseContext.InstitutionType.First(k => k.Id == opprinneligType.Id);
            Assert.That(typeEtterOppdatering.Name, Is.EqualTo(nyttNavn));
            Assert.That(typeEtterOppdatering.Code, Is.EqualTo(opprinneligType.Code));
        }

        [Test]
        public async Task OpprettInstitusjonTest()
        {
            // Arrange and Act
            (var opprettetInstitusjon, _) = await OpprettInstitusjon();
            var opprettetInstitusjonFraDatabase = DatabaseContext.Institution.Include(i => i.Departments).FirstOrDefault(i => i.Id == opprettetInstitusjon.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetInstitusjonFraDatabase, Is.Not.Null);
                Assert.That(opprettetInstitusjonFraDatabase.Navn, Is.EqualTo(opprettetInstitusjon.Name));
                Assert.That(opprettetInstitusjonFraDatabase.Abbreviation, Is.EqualTo(opprettetInstitusjon.Abbreviation));
            });

        }

        [Test]
        public async Task OpprettInstitusjonstypeTest()
        {
            // Arrange and Act
            var opprettetType = await OpprettInstitusjonType();
            var opprettetTypeFraDatabase = DatabaseContext.InstitutionType.First(i => i.Id == opprettetType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetTypeFraDatabase.Name == opprettetType.Name);
                Assert.That(opprettetTypeFraDatabase.Code == opprettetType.Code);
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

        private async Task<Modeller.V1.Institution.InstitutionType> OpprettInstitusjonType(string kode = null)
        {
            var opprettInstitusjonTypeHandler = new OpprettInstitusjonstype.Handler(DatabaseContext, Mapper);
            var opprettCommand = new OpprettInstitusjonstype.Command()
            {
                Institusjonstype = new CreateInstitutionTypeRequest()
                {
                    Code = kode ?? "TEST",
                    Name = "Test"
                }
            };

            var resOpprett = await opprettInstitusjonTypeHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion

    }
}