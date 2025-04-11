using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Tjenester.Klinikk;
using System;

namespace HyFive.Tjenester.Tests.Klinikk
{
    public class KlinikkTests : TjenesteTests
    {
        [Test]
        public async Task HentKlinikkTest()
        {
            // Arrange
            var hentKlinikkHandler = new HentKlinikk.Handler(DatabaseContext, Mapper);

            var institusjon = new Domain.Place.Institution { Id = 9999 };
            var klinikk = new Domain.Place.Clinic { Id = 9999, Institution = institusjon };
            DatabaseContext.Clinic.Add(klinikk);
            DatabaseContext.SaveChanges();

            // Act
            var query = new HentKlinikk.Query() { Id = 9999, InstitusjonId = institusjon.Id };
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

            var hentKlinikkerForInstitusjon = new HentKlinikkerForInstitusjon.Handler(DatabaseContext, Mapper);
            var query = new HentKlinikkerForInstitusjon.Query() { InstitusjonId = institusjon.Id };

            // Act
            var res = await hentKlinikkerForInstitusjon.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res, Has.All.Property(nameof(Modeller.V1.Institution.Clinic.InstitutionId)).EqualTo(institusjon.Id));
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
                Assert.That(opprettetKlinikk.Name, Is.EqualTo(opprettetKlinikkFraDatabase.Navn));
                Assert.That(opprettetKlinikk.InstitutionId, Is.EqualTo(opprettetKlinikkFraDatabase.Institusjon.Id));
                Assert.That(opprettetKlinikk.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(opprettetKlinikkFraDatabase.Avdelinger.Select(a => a.Id).OrderBy(x => x)));
                Assert.That(opprettetKlinikk.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(institusjon.Avdelinger.Select(a => a.Id).OrderBy(x => x)));
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
                async () =>
                {
                    await OpprettKlinikk(institusjon.Id, annenInstitusjon.Avdelinger.ToList());
                }
            );
        }

        [Test]
        public async Task OppdaterKlinikkTest()
        {
            // Arrange
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var avdelinger = institusjon.Avdelinger.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new OppdaterKlinikk.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institution.Clinic
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    InstitutionId = institusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Avdeling>, List<Modeller.V1.Institution.Department>>(avdelinger)
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
                Assert.That(resultatOppdater.Departments.Count, Is.EqualTo(oppdaterCommand.Klinikk.Departments.Count));
            });
        }

        [Test]
        public async Task OppdaterKlinikkTest_KanIkkeOppdatereInstitusjonIdTilKlinikk()
        {
            // Arrange
            var institusjon = DatabaseContext.Institution.Include(i => i.Departments).First();
            var avdelinger = institusjon.Avdelinger.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new OppdaterKlinikk.Handler(DatabaseContext, Mapper);
            var nyinstitusjon = DatabaseContext.Institution.Include(i => i.Departments).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new OppdaterKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institution.Clinic
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    InstitutionId = nyinstitusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Avdeling>, List<Modeller.V1.Institution.Department>>(avdelinger)
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
            var oppdaterKlinikkHandler = new OppdaterKlinikk.Handler(DatabaseContext, Mapper);

            var annenInstitusjon = DatabaseContext.Institution.Include(i => i.Departments).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new OppdaterKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institution.Clinic
                {
                    Id = opprettetKlinikk.Id,
                    Name = "Leverpostei",
                    InstitutionId = institusjon.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Avdeling>, List<Modeller.V1.Institution.Department>>(annenInstitusjon.Avdelinger)
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

        private async Task<Modeller.V1.Institution.Clinic> OpprettKlinikk(int institusjonsId, List<Domain.Place.Avdeling> avdelinger = null)
        {
            var opprettKlinikkHandler = new OpprettKlinikk.Handler(DatabaseContext, Mapper);
            var avdelingerForInstitusjon = avdelinger ?? DatabaseContext.Institution
                .Include(i => i.Departments)
                .FirstOrDefault(x => x.Id == institusjonsId)?.Avdelinger.ToList();
            var avdelingerForInstitusjonModeller =
                Mapper.Map<List<Domain.Place.Avdeling>, List<Modeller.V1.Institution.Department>>(avdelingerForInstitusjon ?? new List<Domain.Place.Avdeling>());
            var opprettCommand = new OpprettKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institution.Clinic
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