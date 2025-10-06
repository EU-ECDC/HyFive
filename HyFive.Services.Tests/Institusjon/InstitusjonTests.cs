using HyFive.Models.V1.Facility;
using HyFive.Models.V1.Session;
using HyFive.Services.Facility;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.Institution
{
    public class InstitusjonTests : ServiceTests
    {
        [Test]
        public async Task OpprettOgHentInstitusjonTest()
        {
            // Arrange
            var institusjon = (await CreateInstitution()).Item1;
            var hentInstitusjonHandler = new GetFacility.Handler(DatabaseContext, Mapper);
            var query = new GetFacility.Query() { FacilityId = institusjon.Id };

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
            var institusjon = (await CreateInstitution()).Item1;

            var hentInstitusjonerHandler = new GetFacilities.Handler(DatabaseContext, Mapper);
            var query = new GetFacilities.Query() { };

            // Act
            var res = await hentInstitusjonerHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.Select(r => r.Id).ToList(), Contains.Item(institusjon.Id));
                Assert.That(res.Count(), Is.EqualTo(DatabaseContext.Facility.ToList().Count));
            });
        }

        [Test]
        public async Task HentInstitusjonerForKoordinatorTest()
        {
            // Arrange
            var hentInstitusjonerForKoordinatorHandler =
                new GetFacilitiesForCoordinator.Handler(DatabaseContext, Mapper);

            // Act
            var institusjoner = await hentInstitusjonerForKoordinatorHandler.Handle(
                new GetFacilitiesForCoordinator.Query()
                {
                    CoordinatorHprNumber = Seed.SeedKoordinatorHprNummer
                }, CancellationToken.None);


            // Assert
            Assert.That(institusjoner.Length, Is.GreaterThan(0));

        }

        [Test]
        public async Task HentInstitusjonstyperTest()
        {
            // Arrange
            var handler = new GetFacilityTypes.Handler(DatabaseContext, Mapper);
            var eksisterendeTypeKoder = DatabaseContext.FacilityType.Select(i => i.Code).ToList();

            // Act
            var typer = await handler.Handle(new GetFacilityTypes.Query(), CancellationToken.None);

            // Assert
            Assert.That(eksisterendeTypeKoder.OrderBy(x => x).SequenceEqual(typer.OrderBy(x => x.Code).Select(t => t.Code)));
        }

        [Test]
        public async Task HentKoordinatorerForInstitusjonTest()
        {
            // Arrange
            (var institusjon, _) = await CreateInstitution();
            var handler = new GetCoordinatorsForFacility.Handler(DatabaseContext, Mapper);
            var antallKoordinatorerTilknyttetInstitusjon =
                DatabaseContext.Coordinator
                    .Include(k => k.Facility)
                    .Count(k => k.Facility.Id == institusjon.Id);

            // Act
            var koordinatorer =
                await handler.Handle(new GetCoordinatorsForFacility.Query() { FacilityId = institusjon.Id },
                    CancellationToken.None);

            // Assert
            Assert.That(koordinatorer.ToList(), Has.Count.EqualTo(antallKoordinatorerTilknyttetInstitusjon));
        }


        [Test]
        public async Task HentObservatorerForInstitusjonTest()
        {
            // Arrange
            (var institusjon, var observator) = await CreateInstitution();
            var handler = new GetObserversForFacility.Handler(DatabaseContext, Mapper);
            var antallObservatorerTilknyttetInstitusjon =
                DatabaseContext.Observer
                    .Include(k => k.Facility)
                    .Count(k => k.Facility.Id == institusjon.Id);

            // Act
            var observatorer =
                await handler.Handle(new GetObserversForFacility.Query() { FacilityId = institusjon.Id },
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
            var hentKommentarerHandler = new GetPredefinedComments.Handler(DatabaseContext);
            var institusjon = (await new GetFacilitiesForCoordinator.Handler(DatabaseContext, Mapper).Handle(
                new GetFacilitiesForCoordinator.Query()
                { CoordinatorHprNumber = Seed.SeedKoordinatorHprNummer }, CancellationToken.None)).First();

            var opprettKommentarerHandler = new CreatePredefinedComment.Handler(DatabaseContext);

            var kommentarRequest = new CreatePredefinedCommentRequest()
            {
                Comment = $"{Guid.NewGuid()}"
            };

            // Act

            var kunneOppretteKommentar = await opprettKommentarerHandler.Handle(
                new CreatePredefinedComment.Command() { NewPredefinedComment = kommentarRequest, FacilityId = institusjon.Id }, CancellationToken.None);

            var kommentarer = await hentKommentarerHandler.Handle(new GetPredefinedComments.Query()
            {
                FacilityId = institusjon.Id,
                SessionType = SessionType.ProtectiveEquipment
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
            (var institusjon, _) = await CreateInstitution();
            var oppdaterInstitusjonHandler = new UpdateFacility.Handler(DatabaseContext, Mapper);
            var nyttNavn = $"Et nytt navn og en tilfeldig verdi:{Guid.NewGuid()}";
            institusjon.Name = nyttNavn;

            // Act
            await oppdaterInstitusjonHandler.Handle(new UpdateFacility.Command()
            { Facility = institusjon }, CancellationToken.None);

            // Assert
            Assert.That(DatabaseContext.Facility.First(i => i.Id == institusjon.Id).Name, Is.EqualTo(nyttNavn));

        }

        [Test]
        public async Task OppdaterInstitusjonstypeTest()
        {
            // Arrange
            var oppdaterInstitusjonstypeHandler = new UpdateFacilityType.Handler(DatabaseContext, Mapper);
            var opprinneligType = DatabaseContext.FacilityType.First();
            var nyttNavn = $"NAVN{Guid.NewGuid()}";

            // Act
            await oppdaterInstitusjonstypeHandler.Handle(new UpdateFacilityType.Command()
            {
                FacilityType = new FacilityType()
                {
                    Id = opprinneligType.Id,
                    Code = "KODE",
                    Name = nyttNavn
                }
            }, CancellationToken.None);

            // Assert
            var typeEtterOppdatering = DatabaseContext.FacilityType.First(k => k.Id == opprinneligType.Id);
            Assert.That(typeEtterOppdatering.Name, Is.EqualTo(nyttNavn));
            Assert.That(typeEtterOppdatering.Code, Is.EqualTo(opprinneligType.Code));
        }

        [Test]
        public async Task OpprettInstitusjonTest()
        {
            // Arrange and Act
            (var opprettetInstitusjon, _) = await CreateInstitution();
            var opprettetInstitusjonFraDatabase = DatabaseContext.Facility.Include(i => i.Departments).FirstOrDefault(i => i.Id == opprettetInstitusjon.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetInstitusjonFraDatabase, Is.Not.Null);
                Assert.That(opprettetInstitusjonFraDatabase.Name, Is.EqualTo(opprettetInstitusjon.Name));
                Assert.That(opprettetInstitusjonFraDatabase.Abbreviation, Is.EqualTo(opprettetInstitusjon.Abbreviation));
            });

        }

        [Test]
        public async Task OpprettInstitusjonstypeTest()
        {
            // Arrange and Act
            var opprettetType = await OpprettInstitusjonType();
            var opprettetTypeFraDatabase = DatabaseContext.FacilityType.First(i => i.Id == opprettetType.Id);

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

        private async Task<Models.V1.Facility.FacilityType> OpprettInstitusjonType(string kode = null)
        {
            var opprettInstitusjonTypeHandler = new CreateFacilityType.Handler(DatabaseContext, Mapper);
            var opprettCommand = new CreateFacilityType.Command()
            {
                FacilityType = new CreateFacilityTypeRequest()
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