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

            var institusjon = new Domene.Sted.Institusjon { Id = 9999 };
            var klinikk = new Domene.Sted.Klinikk { Id = 9999, Institusjon = institusjon };
            DatabaseContext.Klinikk.Add(klinikk);
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
            var institusjon = new Domene.Sted.Institusjon { Id = 9999 };
            var klinikk = new Domene.Sted.Klinikk { Id = 9999, Institusjon = institusjon };
            var klinikk2 = new Domene.Sted.Klinikk { Id = 99999, Institusjon = institusjon };
            DatabaseContext.Institusjon.Add(institusjon);
            DatabaseContext.Klinikk.Add(klinikk);
            DatabaseContext.Klinikk.Add(klinikk2);

            var annenInstitusjon = new Domene.Sted.Institusjon { Id = 1111 };
            var annenKlinikk = new Domene.Sted.Klinikk { Id = 1111, Institusjon = annenInstitusjon };
            var annenKlinikk2 = new Domene.Sted.Klinikk { Id = 11111, Institusjon = annenInstitusjon };
            DatabaseContext.Institusjon.Add(annenInstitusjon);
            DatabaseContext.Klinikk.Add(annenKlinikk);
            DatabaseContext.Klinikk.Add(annenKlinikk2);

            DatabaseContext.SaveChanges();

            var hentKlinikkerForInstitusjon = new HentKlinikkerForInstitusjon.Handler(DatabaseContext, Mapper);
            var query = new HentKlinikkerForInstitusjon.Query() { InstitusjonId = institusjon.Id };

            // Act
            var res = await hentKlinikkerForInstitusjon.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res, Has.All.Property(nameof(Modeller.V1.Institusjon.Klinikk.InstitusjonId)).EqualTo(institusjon.Id));
                Assert.That(res.Any(x => x.Id == klinikk.Id));
                Assert.That(res.Any(x => x.Id == klinikk2.Id));
            });
        }

        [Test]
        public async Task OpprettKlinikkTest()
        {
            // Arrange and Act
            var institusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First();
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var opprettetKlinikkFraDatabase = DatabaseContext.Klinikk
                .Include(k => k.Institusjon)
                .Include(k => k.Avdelinger)
                .FirstOrDefault(k => k.Id == opprettetKlinikk.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetKlinikk.Id, Is.GreaterThan(0));
                Assert.That(opprettetKlinikk.Navn, Is.EqualTo(opprettetKlinikkFraDatabase.Navn));
                Assert.That(opprettetKlinikk.InstitusjonId, Is.EqualTo(opprettetKlinikkFraDatabase.Institusjon.Id));
                Assert.That(opprettetKlinikk.Avdelinger.Select(a => a.Id).OrderBy(x => x).SequenceEqual(opprettetKlinikkFraDatabase.Avdelinger.Select(a => a.Id).OrderBy(x => x)));
                Assert.That(opprettetKlinikk.Avdelinger.Select(a => a.Id).OrderBy(x => x).SequenceEqual(institusjon.Avdelinger.Select(a => a.Id).OrderBy(x => x)));
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
            var institusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First();
            var annenInstitusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First(x => x.Id != institusjon.Id);

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
            var institusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First();
            var avdelinger = institusjon.Avdelinger.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new OppdaterKlinikk.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new OppdaterKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institusjon.Klinikk
                {
                    Id = opprettetKlinikk.Id,
                    Navn = "Leverpostei",
                    InstitusjonId = institusjon.Id,
                    Avdelinger = Mapper.Map<IEnumerable<Domene.Sted.Avdeling>, List<Modeller.V1.Institusjon.Avdeling>>(avdelinger)
                }
            };

            // Act
            var resultatOppdater = await oppdaterKlinikkHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetKlinikk.Id));
                Assert.That(resultatOppdater.Navn, Is.Not.EqualTo(opprettetKlinikk.Navn));
                Assert.That(resultatOppdater.InstitusjonId, Is.EqualTo(opprettetKlinikk.InstitusjonId));
                Assert.That(resultatOppdater.Avdelinger.Count, Is.EqualTo(oppdaterCommand.Klinikk.Avdelinger.Count));
            });
        }

        [Test]
        public async Task OppdaterKlinikkTest_KanIkkeOppdatereInstitusjonIdTilKlinikk()
        {
            // Arrange
            var institusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First();
            var avdelinger = institusjon.Avdelinger.Take(1);
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new OppdaterKlinikk.Handler(DatabaseContext, Mapper);
            var nyinstitusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new OppdaterKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institusjon.Klinikk
                {
                    Id = opprettetKlinikk.Id,
                    Navn = "Leverpostei",
                    InstitusjonId = nyinstitusjon.Id,
                    Avdelinger = Mapper.Map<IEnumerable<Domene.Sted.Avdeling>, List<Modeller.V1.Institusjon.Avdeling>>(avdelinger)
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
            var institusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First();
            var opprettetKlinikk = await OpprettKlinikk(institusjon.Id);
            var oppdaterKlinikkHandler = new OppdaterKlinikk.Handler(DatabaseContext, Mapper);

            var annenInstitusjon = DatabaseContext.Institusjon.Include(i => i.Avdelinger).First(x => x.Id != institusjon.Id);
            var oppdaterCommand = new OppdaterKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institusjon.Klinikk
                {
                    Id = opprettetKlinikk.Id,
                    Navn = "Leverpostei",
                    InstitusjonId = institusjon.Id,
                    Avdelinger = Mapper.Map<IEnumerable<Domene.Sted.Avdeling>, List<Modeller.V1.Institusjon.Avdeling>>(annenInstitusjon.Avdelinger)
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

        private async Task<Modeller.V1.Institusjon.Klinikk> OpprettKlinikk(int institusjonsId, List<Domene.Sted.Avdeling> avdelinger = null)
        {
            var opprettKlinikkHandler = new OpprettKlinikk.Handler(DatabaseContext, Mapper);
            var avdelingerForInstitusjon = avdelinger ?? DatabaseContext.Institusjon
                .Include(i => i.Avdelinger)
                .FirstOrDefault(x => x.Id == institusjonsId)?.Avdelinger.ToList();
            var avdelingerForInstitusjonModeller =
                Mapper.Map<List<Domene.Sted.Avdeling>, List<Modeller.V1.Institusjon.Avdeling>>(avdelingerForInstitusjon ?? new List<Domene.Sted.Avdeling>());
            var opprettCommand = new OpprettKlinikk.Command()
            {
                Klinikk = new Modeller.V1.Institusjon.Klinikk
                {
                    Navn = "Test",
                    InstitusjonId = institusjonsId,
                    Avdelinger = avdelingerForInstitusjonModeller
                }
            };

            var resOpprett = await opprettKlinikkHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}