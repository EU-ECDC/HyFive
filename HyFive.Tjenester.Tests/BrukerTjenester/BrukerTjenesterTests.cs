using HyFive.Modeller.V1.Bruker;
using HyFive.Tjenester.BrukerTjenester;
using Fhi.HelseId.Web.Services;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bruker = HyFive.Modeller.V1.Bruker.Bruker;

namespace HyFive.Tjenester.Tests.BrukerTjenester
{
    public class BrukerTjenesterTests : TjenesteTests
    {
        private readonly string _pseudonym = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("hellohellohellohellohellohellohel"));
        private ICurrentUser _currentUserSubstitute;

        [SetUp]
        public void SetUpSubstitutes()
        {
            _currentUserSubstitute = Substitute.For<ICurrentUser>();
        }

        [Test]
        public async Task HentFhiAdmin_Test()
        {
            // Arrange
            var fhiAdmin = await OpprettFhiAdmin();

            var hentFhiAdminHandler = new HentFhiAdmin.Handler(DatabaseContext, Mapper);
            var query = new HentFhiAdmin.Query() { };
            var fhiAdminIdsFraDatabase = DatabaseContext.FhiAdmin.OrderBy(x => x.Id).Select(x => x.Id).ToList();

            // Act
            var res = await hentFhiAdminHandler.Handle(query, new System.Threading.CancellationToken());
            var resIds = res.OrderBy(x => x.Id).Select(x => x.Id).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resIds, Contains.Item(fhiAdmin.Id));
                Assert.That(resIds.SequenceEqual(fhiAdminIdsFraDatabase));
            });
        }

        [Test]
        public async Task OpprettFhiAdmin_Test()
        {
            // Arrange and Act
            var opprettetFhiAdmin = await OpprettFhiAdmin();
            var opprettetFhiAdminFraDatabase = DatabaseContext.FhiAdmin.FirstOrDefault(r => r.Id == opprettetFhiAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetFhiAdmin.Id, Is.GreaterThan(0));
                Assert.That(opprettetFhiAdmin.IdentPseudonym, Is.EqualTo(opprettetFhiAdminFraDatabase.IdentPseudonym));
                Assert.That(opprettetFhiAdmin.Fornavn, Is.EqualTo(opprettetFhiAdminFraDatabase.Fornavn));
                Assert.That(opprettetFhiAdmin.Etternavn, Is.EqualTo(opprettetFhiAdminFraDatabase.Etternavn));
            });
        }

        [Test]
        public void OpprettFhiAdmin_ManglerPseudonym_KasterException()
        {
            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Mangler"),
                async () =>
                {
                    await OpprettFhiAdmin(pseudonym: "");
                }
            );
        }

        [Test]
        public void OpprettFhiAdmin_IkkeGyldigPseudonym_KasterException()
        {
            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke gyldig."),
                async () =>
                {
                    await OpprettFhiAdmin(pseudonym: "1234567890123456789012345678901234567890123@");
                }
            );
        }

        [Test]
        public void OpprettFhiAdmin_ForKortPseudonym_KasterException()
        {
            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke gyldig."),
                async () =>
                {
                    await OpprettFhiAdmin(pseudonym: "test");
                }
            );
        }

        [Test]
        public async Task OpprettFhiAdmin_EksisterendePseudonym_KasterException()
        {
            await OpprettFhiAdmin(pseudonym: _pseudonym);

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("allerede i bruk"),
                async () =>
                {
                    await OpprettFhiAdmin(pseudonym: _pseudonym);
                }
            );
        }

        [Test]
        public async Task OppdaterFhiAdmin_Test()
        {
            // Arrange
            var opprettetFhiAdmin = await OpprettFhiAdmin();
            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id = opprettetFhiAdmin.Id,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("oellooellooellooellooellooellooel")),
                    ErDeaktivert = false
                }
            };

            // Act
            var oppdatertFhiAdmin = await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
            var oppdatertFhiAdminFraDatabase = DatabaseContext.FhiAdmin.First(x => x.Id == opprettetFhiAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oppdatertFhiAdmin.Id, Is.EqualTo(opprettetFhiAdmin.Id));
                Assert.That(oppdatertFhiAdmin.IdentPseudonym, Is.Not.EqualTo(opprettetFhiAdmin.IdentPseudonym));
                Assert.That(oppdatertFhiAdmin.Fornavn, Is.Not.EqualTo(opprettetFhiAdmin.Fornavn));
                Assert.That(oppdatertFhiAdmin.Etternavn, Is.Not.EqualTo(opprettetFhiAdmin.Etternavn));

                Assert.That(oppdatertFhiAdminFraDatabase.IdentPseudonym, Is.EqualTo(command.Bruker.IdentPseudonym));
                Assert.That(oppdatertFhiAdminFraDatabase.Fornavn, Is.EqualTo(command.Bruker.Fornavn));
                Assert.That(oppdatertFhiAdminFraDatabase.Etternavn, Is.EqualTo(command.Bruker.Etternavn));
                Assert.That(oppdatertFhiAdminFraDatabase.ErDeaktivert, Is.EqualTo(command.Bruker.ErDeaktivert));
            });
        }

        [Test]
        public void OppdaterFhiAdmin_IkkeEksisterendeBruker_KasterException()
        {
            // Arrange
            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id = 1234567890,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = _pseudonym,
                    ErDeaktivert = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Fant ikke bruker med Id"),
                async () =>
                {
                    await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public void OppdaterFhiAdmin_ManglerPseudonym_KasterException()
        {
            // Arrange
            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id = 1234567890,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = null,
                    ErDeaktivert = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Mangler"),
                async () =>
                {
                    await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public void OppdaterFhiAdmin_IkkeGyldigPseudonym_KasterException()
        {
            // Arrange
            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id = 1234567890,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = "1234567890123456789012345678901234567890123@",
                    ErDeaktivert = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke gyldig."),
                async () =>
                {
                    await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public void OppdaterFhiAdmin_ForKortPseudonym_KasterException()
        {
            // Arrange
            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id = 1234567890,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = "test",
                    ErDeaktivert = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("ikke gyldig."),
                async () =>
                {
                    await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task OppdaterFhiAdmin_ProverAOppdatereTilPseudonymTilEnAnnenFhiAdmin_KasterException()
        {
            // Arrange
            var bruker1 = await OpprettFhiAdmin(_pseudonym);
            var bruker2 = await OpprettFhiAdmin(Convert.ToBase64String(Encoding.UTF8.GetBytes("oellooellooellooellooellooellooel")));

            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id = bruker2.Id,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = bruker1.IdentPseudonym,
                    ErDeaktivert = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Pseudonymet er allerede i bruk"),
                async () =>
                {
                    await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task OppdaterFhiAdmin_InnloggetBrukerProverAEndreSegSelv_KasterException()
        {
            // Arrange
            var bruker1 = await OpprettFhiAdmin(_pseudonym);
            _currentUserSubstitute.PidPseudonym.Returns(bruker1.IdentPseudonym);

            var oppdaterFhiAdminHandler = new OppdaterFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new OppdaterFhiAdmin.Command()
            {
                Bruker = new Modeller.V1.Bruker.Bruker()
                {
                    Id =  bruker1.Id,
                    Fornavn = "Da",
                    Etternavn = "Vinci",
                    IdentPseudonym = bruker1.IdentPseudonym,
                    ErDeaktivert = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Bruker kan ikke endre på seg selv"),
                async () =>
                {
                    await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        #region Helper-methods

        private async Task<Modeller.V1.Bruker.Bruker> OpprettFhiAdmin(string pseudonym = null)
        {
            var opprettFhiAdminHandler = new OpprettFhiAdmin.Handler(DatabaseContext, Mapper);
            var command = new OpprettFhiAdmin.Command()
            {
                Request = new OpprettFhiAdminRequest()
                {
                    Fornavn = "Test",
                    Etternavn = "Testesen",
                    IdentPseudonym = pseudonym ?? _pseudonym,
                }
            };

            var opprettetFhiAdmin = await opprettFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());

            return opprettetFhiAdmin;
        }

        #endregion
    }
}