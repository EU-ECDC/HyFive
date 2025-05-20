using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Services.Clinic;
using System;

namespace HyFive.Services.Tests.Klinikk
{
    public class KlinikkTests : ServiceTests
    {
        [Test]
        public async Task HentKlinikkTest()
        {
            // Arrange
            var hentKlinikkHandler = new GetClinic.Handler(DatabaseContext, Mapper);

            var institusjon = new Domain.Place.Institution { Id = 9999 };
            var klinikk = new Domain.Place.Clinic { Id = 9999, Institution = institusjon };
            DatabaseContext.Clinic.Add(klinikk);
            DatabaseContext.SaveChanges();

            // Act
            var query = new GetClinic.Query() { Id = 9999, InstitutionId = institusjon.Id };
            var res = await hentKlinikkHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(klinikk.Id, Is.EqualTo(res.Id));
        }

        [Test]
        public async Task HentKlinikkerForInstitusjonTest()
        {
            // Arrange
            var institusjon = new Domain.Place.Institution { Id = 9999 };
            var klinikk = new Domain.Place.Clinic { Id = 9999, Institution = institusjon };
            var klinikk2 = new Domain.Place.Clinic { Id = 99999, Institution = institusjon };
            DatabaseContext.Institution.Add(institusjon);
            DatabaseContext.Clinic.Add(klinikk);
            DatabaseContext.Clinic.Add(klinikk2);

            var annenInstitusjon = new Domain.Place.Institution { Id = 1111 };
            var annenKlinikk = new Domain.Place.Clinic { Id = 1111, Institution = annenInstitusjon };
            var annenKlinikk2 = new Domain.Place.Clinic { Id = 11111, Institution = annenInstitusjon };
            DatabaseContext.Institution.Add(annenInstitusjon);
            DatabaseContext.Clinic.Add(annenKlinikk);
            DatabaseContext.Clinic.Add(annenKlinikk2);

            DatabaseContext.SaveChanges();

            var hentKlinikkerForInstitusjon = new GetClinicsForInstitution.Handler(DatabaseContext, Mapper);
            var query = new GetClinicsForInstitution.Query() { InstitutionId = institusjon.Id };

            // Act
            var res = await hentKlinikkerForInstitusjon.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res, Has.All.Property(nameof(Models.V1.Institution.Clinic.InstitutionId)).EqualTo(institusjon.Id));
                Assert.That(res.Any(x => x.Id == klinikk.Id));
                Assert.That(res.Any(x => x.Id == klinikk2.Id));
            });
        }

        [Test]
        public async Task OpprettKlinikkTest()
        {
            // Arrange and Act
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var opprettetKlinikkFraDatabase = DatabaseContext.Clinic
                .Include(k => k.Institution)
                .Include(k => k.Departments)
                .FirstOrDefault(k => k.Id == opprettetKlinikk.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetKlinikk.Id, Is.GreaterThan(0));
                Assert.That(opprettetKlinikk.Name, Is.EqualTo(opprettetKlinikkFraDatabase.Name));
                Assert.That(opprettetKlinikk.InstitutionId, Is.EqualTo(opprettetKlinikkFraDatabase.Institution.Id));
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
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var annenInstitusjon = DatabaseContext.Institution.Include(i => i.Departments).First(x => x.Id != institusjon.Id);

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
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var avdelinger = institusjon.Departments.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new UpdateClinic.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateClinic.Command()
            {
                Clinic = new Models.V1.Institution.Clinic
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    InstitutionId = institusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Institution.Department>>(avdelinger)
                }
            };

            // Act
            var resultatOppdater = await oppdaterKlinikkHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetKlinikk.Id));
                Assert.That(resultatOppdater.Name, Is.Not.EqualTo(opprettetKlinikk.Name));
                Assert.That(resultatOppdater.InstitutionId, Is.EqualTo(opprettetKlinikk.InstitutionId));
                Assert.That(resultatOppdater.Departments.Count, Is.EqualTo(oppdaterCommand.Clinic.Departments.Count));
            });
        }

        [Test]
        public async Task OppdaterKlinikkTest_KanIkkeOppdatereInstitusjonIdTilKlinikk()
        {
            // Arrange
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var avdelinger = institusjon.Departments.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new UpdateClinic.Handler(DatabaseContext, Mapper);
            var nyinstitusjon = DatabaseContext.Institution.Include(i => i.Departments).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new UpdateClinic.Command()
            {
                Clinic = new Models.V1.Institution.Clinic
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    InstitutionId = nyinstitusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Institution.Department>>(avdelinger)
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
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new UpdateClinic.Handler(DatabaseContext, Mapper);

            var annenInstitusjon = DatabaseContext.Institution.Include(i => i.Departments).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new UpdateClinic.Command()
            {
                Clinic = new Models.V1.Institution.Clinic
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    InstitutionId = institusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Institution.Department>>(annenInstitusjon.Departments)
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

        private async Task<Models.V1.Institution.Clinic> OpprettKlinikk(int institusjonsId, List<Domain.Place.Department> avdelinger = null)
        {
            var opprettKlinikkHandler = new CreateClinic.Handler(DatabaseContext, Mapper);
            var avdelingerForInstitusjon = avdelinger ?? DatabaseContext.Institution
                .Include(i => i.Departments)
                .FirstOrDefault(x => x.Id == institusjonsId)?.Departments.ToList();
            var avdelingerForInstitusjonModeller =
                Mapper.Map<List<Domain.Place.Department>, List<Models.V1.Institution.Department>>(avdelingerForInstitusjon ?? new List<Domain.Place.Department>());
            var opprettCommand = new CreateClinic.Command()
            {
                Clinic = new Models.V1.Institution.Clinic
                {
                    Name = "Test",
                    InstitutionId = institusjonsId,
                    Departments = avdelingerForInstitusjonModeller
                }
            };

            var resOpprett = await opprettKlinikkHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}