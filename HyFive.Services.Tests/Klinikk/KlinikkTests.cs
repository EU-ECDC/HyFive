using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Services.Unit;
using System;

namespace HyFive.Services.Tests.Klinikk
{
    public class KlinikkTests : ServiceTests
    {
        [Test]
        public async Task HentKlinikkTest()
        {
            // Arrange
            var hentKlinikkHandler = new GetUnit.Handler(DatabaseContext, Mapper);

            var institusjon = new Domain.Place.Facility { Id = 9999 };
            var klinikk = new Domain.Place.Unit { Id = 9999, Facility = institusjon };
            DatabaseContext.Unit.Add(klinikk);
            DatabaseContext.SaveChanges();

            // Act
            var query = new GetUnit.Query() { Id = 9999, FacilityId = institusjon.Id };
            var res = await hentKlinikkHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(klinikk.Id, Is.EqualTo(res.Id));
        }

        [Test]
        public async Task HentKlinikkerForInstitusjonTest()
        {
            // Arrange
            var institusjon = new Domain.Place.Facility { Id = 9999 };
            var klinikk = new Domain.Place.Unit { Id = 9999, Facility = institusjon };
            var klinikk2 = new Domain.Place.Unit { Id = 99999, Facility = institusjon };
            DatabaseContext.Facility.Add(institusjon);
            DatabaseContext.Unit.Add(klinikk);
            DatabaseContext.Unit.Add(klinikk2);

            var annenInstitusjon = new Domain.Place.Facility { Id = 1111 };
            var annenKlinikk = new Domain.Place.Unit { Id = 1111, Facility = annenInstitusjon };
            var annenKlinikk2 = new Domain.Place.Unit { Id = 11111, Facility = annenInstitusjon };
            DatabaseContext.Facility.Add(annenInstitusjon);
            DatabaseContext.Unit.Add(annenKlinikk);
            DatabaseContext.Unit.Add(annenKlinikk2);

            DatabaseContext.SaveChanges();

            var hentKlinikkerForInstitusjon = new GetUnitsForFacility.Handler(DatabaseContext, Mapper);
            var query = new GetUnitsForFacility.Query() { FacilityId = institusjon.Id };

            // Act
            var res = await hentKlinikkerForInstitusjon.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res, Has.All.Property(nameof(Models.V1.Facility.Unit.FacilityId)).EqualTo(institusjon.Id));
                Assert.That(res.Any(x => x.Id == klinikk.Id));
                Assert.That(res.Any(x => x.Id == klinikk2.Id));
            });
        }

        [Test]
        public async Task OpprettKlinikkTest()
        {
            // Arrange and Act
            var institusjon = DatabaseContext.Facility.Include(i => i.Departments).First();
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var opprettetKlinikkFraDatabase = DatabaseContext.Unit
                .Include(k => k.Facility)
                .Include(k => k.Departments)
                .FirstOrDefault(k => k.Id == opprettetKlinikk.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetKlinikk.Id, Is.GreaterThan(0));
                Assert.That(opprettetKlinikk.Name, Is.EqualTo(opprettetKlinikkFraDatabase.Name));
                Assert.That(opprettetKlinikk.FacilityId, Is.EqualTo(opprettetKlinikkFraDatabase.Facility.Id));
                Assert.That(opprettetKlinikk.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(opprettetKlinikkFraDatabase.Departments.Select(a => a.Id).OrderBy(x => x)));
                Assert.That(opprettetKlinikk.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(institusjon.Departments.Select(a => a.Id).OrderBy(x => x)));
            });
        }

        [Test]
        public void OpprettKlinikkTest_IkkeEksisterendeInstitusjon()
        {
            // Arrange 
            var ikkeEksisterendeInstitusjonsId = 9999;

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Kunne ikke finne"),
                async () =>
                {
                    await OpprettKlinikk(ikkeEksisterendeInstitusjonsId);
                }
            );
        }

        [Test]
        public void OpprettKlinikkTest_KanIkkeOppretteKlinikkMedAvdelingTilAnnenInstitusjon()
        {
            // Arrange
            var institusjon = DatabaseContext.Facility.Include(i => i.Departments).First();
            var annenInstitusjon = DatabaseContext.Facility.Include(i => i.Departments).First(x => x.Id != institusjon.Id);

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<InvalidOperationException>().And.Message.Contains("ikke tilknyttet institusjon"),
                (AsyncTestDelegate)(async () =>
                {
                    await OpprettKlinikk(institusjon.Id, annenInstitusjon.Departments.ToList());
                })
            );
        }

        [Test]
        public async Task OppdaterKlinikkTest()
        {
            // Arrange
            var institusjon = DatabaseContext.Facility.Include(i => i.Departments).First();
            var avdelinger = institusjon.Departments.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    FacilityId = institusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(avdelinger)
                }
            };

            // Act
            var resultatOppdater = await oppdaterKlinikkHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetKlinikk.Id));
                Assert.That(resultatOppdater.Name, Is.Not.EqualTo(opprettetKlinikk.Name));
                Assert.That(resultatOppdater.FacilityId, Is.EqualTo(opprettetKlinikk.FacilityId));
                Assert.That(resultatOppdater.Departments.Count, Is.EqualTo(oppdaterCommand.Unit.Departments.Count));
            });
        }

        [Test]
        public async Task OppdaterKlinikkTest_KanIkkeOppdatereInstitusjonIdTilKlinikk()
        {
            // Arrange
            var institusjon = DatabaseContext.Facility.Include(i => i.Departments).First();
            var avdelinger = institusjon.Departments.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);
            var nyinstitusjon = DatabaseContext.Facility.Include(i => i.Departments).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new UpdateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    FacilityId = nyinstitusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(avdelinger)
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke tilknyttet institusjon"),
                async () =>
                {
                    await oppdaterKlinikkHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task OppdaterKlinikkTest_KanIkkeOppdatereKlinikkMedAvdelingTilAnnenInstitusjon()
        {
            // Arrange
            var institusjon = DatabaseContext.Facility.Include(i => i.Departments).First();
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);

            var annenInstitusjon = DatabaseContext.Facility.Include(i => i.Departments).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new UpdateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    FacilityId = institusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(annenInstitusjon.Departments)
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<InvalidOperationException>().And.Message.Contains("ikke tilknyttet institusjon"),
                async () =>
                {
                    await oppdaterKlinikkHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #region Helper-methods

        private async Task<Models.V1.Facility.Unit> OpprettKlinikk(int institusjonsId, List<Domain.Place.Department> avdelinger = null)
        {
            var opprettKlinikkHandler = new CreateUnit.Handler(DatabaseContext, Mapper);
            var avdelingerForInstitusjon = avdelinger ?? DatabaseContext.Facility
                .Include(i => i.Departments)
                .FirstOrDefault(x => x.Id == institusjonsId)?.Departments.ToList();
            var avdelingerForInstitusjonModeller =
                Mapper.Map<List<Domain.Place.Department>, List<Models.V1.Facility.Department>>(avdelingerForInstitusjon ?? new List<Domain.Place.Department>());
            var opprettCommand = new CreateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Name = "Test",
                    FacilityId = institusjonsId,
                    Departments = avdelingerForInstitusjonModeller
                }
            };

            var resOpprett = await opprettKlinikkHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}