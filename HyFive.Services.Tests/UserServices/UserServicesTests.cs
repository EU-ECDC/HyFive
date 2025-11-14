using HyFive.Models.V1.User;
using HyFive.Services.UserServices;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bruker = HyFive.Models.V1.User.User;

namespace HyFive.Services.Tests.UserServices
{
    public class UserServicesTests : ServiceTests
    {
        private readonly string _email = "testEmail@test.com";
        private IHttpContextAccessor _httpContextAccessorSubstitute;


        [SetUp]
        public void SetUpSubstitutes()
        {
            _httpContextAccessorSubstitute = Substitute.For<IHttpContextAccessor>();
        }

        [Test]
        public async Task GetAdmin_Test()
        {
            // Arrange
            var Admin = await CreateAdmin("testEmail@gmail.com");

            var getAdminHandler = new GetAdmin.Handler(DatabaseContext, Mapper);
            var query = new GetAdmin.Query() { };
            var adminIdsFromDatabase = DatabaseContext.Admin.OrderBy(x => x.Id).Select(x => x.Id).ToList();

            // Act
            var res = await getAdminHandler.Handle(query, new System.Threading.CancellationToken());
            var resIds = res.OrderBy(x => x.Id).Select(x => x.Id).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resIds, Contains.Item(Admin.Id));
                Assert.That(resIds.SequenceEqual(adminIdsFromDatabase));
            });
        }

        [Test]
        public async Task CreateAdmin_Test()
        {
            // Arrange and Act
            var createAdmin = await CreateAdmin("testEmail@gmail.com");
            var createAdminFromDatabase = DatabaseContext.Admin.FirstOrDefault(r => r.Id == createAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createAdmin.Id, Is.GreaterThan(0));
                Assert.That(createAdmin.IdentityPseudonym, Is.EqualTo(createAdminFromDatabase.IdentityPseudonym));
                Assert.That(createAdmin.FirstName, Is.EqualTo(createAdminFromDatabase.FirstName));
                Assert.That(createAdmin.LastName, Is.EqualTo(createAdminFromDatabase.LastName));
            });
        }

        [Test]
        public void CreateAdmin_InvalidEmail_ThrowsException()
        {
            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("Email '1234567890123456789012345678901234567890123@' is not valid."),
                async () =>
                {
                    await CreateAdmin(email: "1234567890123456789012345678901234567890123@");
                }
            );
        }


        [Test]
        public async Task CreateAdmin_ExistingEmail_ThrowsException()
        {
            await CreateAdmin(email: _email);

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("Email 'testEmail@test.com' is already in use"),
                async () =>
                {
                    await CreateAdmin(email: _email);
                }
            );
        }

        [Test]
        public async Task UpdateAdmin_Test()
        {
            // Arrange
            var createAdmin = await CreateAdmin("testEmail@gmail.com");
            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper, _httpContextAccessorSubstitute);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = createAdmin.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    IdentityPseudonym = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("oellooellooellooellooellooellooel")),
                    IsDisabled = false
                }
            };

            // Act
            var updateAdmin = await updateAdminHandler.Handle(command, new System.Threading.CancellationToken());
            var updatedAdminFromDatabase = DatabaseContext.Admin.First(x => x.Id == createAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateAdmin.Id, Is.EqualTo(createAdmin.Id));
                Assert.That(updateAdmin.IdentityPseudonym, Is.Not.EqualTo(createAdmin.IdentityPseudonym));
                Assert.That(updateAdmin.FirstName, Is.Not.EqualTo(createAdmin.FirstName));
                Assert.That(updateAdmin.LastName, Is.Not.EqualTo(createAdmin.LastName));

                Assert.That(updatedAdminFromDatabase.IdentityPseudonym, Is.EqualTo(command.User.IdentityPseudonym));
                Assert.That(updatedAdminFromDatabase.FirstName, Is.EqualTo(command.User.FirstName));
                Assert.That(updatedAdminFromDatabase.LastName, Is.EqualTo(command.User.LastName));
                Assert.That(updatedAdminFromDatabase.IsDeactivated, Is.EqualTo(command.User.IsDisabled));
            });
        }

        [Test]
        public void UpdateAdmin_NonExistentUser_ThrowsException()
        {
            // Arrange
            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper, _httpContextAccessorSubstitute);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = 1234567890,
                    FirstName = "Da",
                    LastName = "Vinci",
                    Email = _email,
                    IsDisabled = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("User not found with Id: 1234567890"),
                async () =>
                {
                    await updateAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task UpdateAdmin_TryingToUpdateToEmailOfAnotherAdmin_ThrowsException()
        {
            // Arrange
            var user1 = await CreateAdmin(_email);
            var user2 = await CreateAdmin("user2@test.com");

            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper, _httpContextAccessorSubstitute);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = user2.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    Email = user1.Email,
                    IsDisabled = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("The email is already in use"),
                async () =>
                {
                    await updateAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task UpdateAdmin_LoggedInUserTriesToChangeSelf_ThrowsException()
        {
            // Arrange
            var user1 = await CreateAdmin(_email);

            _httpContextAccessorSubstitute.HttpContext = new DefaultHttpContext();
            _httpContextAccessorSubstitute.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                new Claim(ClaimTypes.Email, _email)
                    },
                    "TestAuth"
                )
            );

            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper, _httpContextAccessorSubstitute);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id =  user1.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    Email = user1.Email,
                    IsDisabled = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentException>().And.Message.Contains("User cannot change themselves"),
                async () =>
                {
                    await updateAdminHandler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        #region Helper-methods

        private async Task<Models.V1.User.User> CreateAdmin(string email)
        {
            var createAdminHandler = new CreateAdmin.Handler(DatabaseContext, Mapper);
            var command = new CreateAdmin.Command()
            {
                Request = new CreateAdminRequest()
                {
                    FirstName = "Test",
                    LastName = "Testesen",
                    Email = email ?? _email,
                }
            };

            var createAdmin = await createAdminHandler.Handle(command, new System.Threading.CancellationToken());

            return createAdmin;
        }

        #endregion
    }
}