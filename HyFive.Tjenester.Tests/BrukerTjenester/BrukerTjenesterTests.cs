using HyFive.Models.V1.User;
using HyFive.Services.UserServices;
using Fhi.HelseId.Web.Services;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bruker = HyFive.Models.V1.User.User;

namespace HyFive.Services.Tests.BrukerTjenester
{
    public class BrukerTjenesterTests : ServiceTests
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

            var hentFhiAdminHandler = new GetFhiAdmin.Handler(DatabaseContext, Mapper);
            var query = new GetFhiAdmin.Query() { };
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
                Assert.That(opprettetFhiAdmin.IdentityPseudonym, Is.EqualTo(opprettetFhiAdminFraDatabase.IdentityPseudonym));
                Assert.That(opprettetFhiAdmin.FirstName, Is.EqualTo(opprettetFhiAdminFraDatabase.FirstName));
                Assert.That(opprettetFhiAdmin.LastName, Is.EqualTo(opprettetFhiAdminFraDatabase.LastName));
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
            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = opprettetFhiAdmin.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("oellooellooellooellooellooellooel")),
                    IsDisabled = false
                }
            };

            // Act
            var oppdatertFhiAdmin = await oppdaterFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());
            var oppdatertFhiAdminFraDatabase = DatabaseContext.FhiAdmin.First(x => x.Id == opprettetFhiAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oppdatertFhiAdmin.Id, Is.EqualTo(opprettetFhiAdmin.Id));
                Assert.That(oppdatertFhiAdmin.IdentityPseudonym, Is.Not.EqualTo(opprettetFhiAdmin.IdentityPseudonym));
                Assert.That(oppdatertFhiAdmin.FirstName, Is.Not.EqualTo(opprettetFhiAdmin.FirstName));
                Assert.That(oppdatertFhiAdmin.LastName, Is.Not.EqualTo(opprettetFhiAdmin.LastName));

                Assert.That(oppdatertFhiAdminFraDatabase.IdentityPseudonym, Is.EqualTo(command.User.IdentityPseudonym));
                Assert.That(oppdatertFhiAdminFraDatabase.FirstName, Is.EqualTo(command.User.FirstName));
                Assert.That(oppdatertFhiAdminFraDatabase.LastName, Is.EqualTo(command.User.LastName));
                Assert.That(oppdatertFhiAdminFraDatabase.IsDeactivated, Is.EqualTo(command.User.IsDisabled));
            });
        }

        [Test]
        public void OppdaterFhiAdmin_IkkeEksisterendeBruker_KasterException()
        {
            // Arrange
            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = 1234567890,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = _pseudonym,
                    IsDisabled = false
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
            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = 1234567890,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = null,
                    IsDisabled = false
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
            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = 1234567890,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = "1234567890123456789012345678901234567890123@",
                    IsDisabled = false
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
            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = 1234567890,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = "test",
                    IsDisabled = false
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

            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = bruker2.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = bruker1.IdentityPseudonym,
                    IsDisabled = false
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
            _currentUserSubstitute.PidPseudonym.Returns(bruker1.IdentityPseudonym);

            var oppdaterFhiAdminHandler = new UpdateFhiAdmin.Handler(DatabaseContext, Mapper, _currentUserSubstitute);
            var command = new UpdateFhiAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id =  bruker1.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = bruker1.IdentityPseudonym,
                    IsDisabled = false
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

        private async Task<Models.V1.User.User> OpprettFhiAdmin(string pseudonym = null)
        {
            var opprettFhiAdminHandler = new CreateFhiAdmin.Handler(DatabaseContext, Mapper);
            var command = new CreateFhiAdmin.Command()
            {
                Request = new CreateFhiAdminRequest()
                {
                    FirstName = "Test",
                    LastName = "Testesen",
                    IdentityPseudonym = pseudonym ?? _pseudonym,
                }
            };

            var opprettetFhiAdmin = await opprettFhiAdminHandler.Handle(command, new System.Threading.CancellationToken());

            return opprettetFhiAdmin;
        }

        #endregion
    }
}