using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.User;
using HyFive.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
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
            var adminUser = await CreateAdmin("testEmail@gmail.com");
            var getAdminHandler = new GetAdmin.Handler(DatabaseContext, Mapper);
            var query = new GetAdmin.Query();

            var expectedIds = DatabaseContext.User
                .Where(u => u.UserPermissions.Any(p => p.PermissionLevel == PermissionLevelConstants.Administrator))
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .ToList();

            // Act
            var res = await getAdminHandler.Handle(query, CancellationToken.None);
            var resIds = res.OrderBy(x => x.Id).Select(x => x.Id).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resIds, Contains.Item(adminUser.Id));
                Assert.That(resIds.SequenceEqual(expectedIds));
            });
        }

        [Test]
        public async Task CreateAdmin_Test()
        {
            // Arrange and Act
            var createdAdmin = await CreateAdmin("testEmail@gmail.com");

            var createdAdminFromDatabase = DatabaseContext.User
                .Include(u => u.UserPermissions)
                .FirstOrDefault(r => r.Id == createdAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createdAdmin.Id, Is.GreaterThan(0));
                Assert.That(createdAdminFromDatabase, Is.Not.Null);
                Assert.That(createdAdmin.Email, Is.EqualTo(createdAdminFromDatabase.Email));
                Assert.That(createdAdmin.FirstName, Is.EqualTo(createdAdminFromDatabase.FirstName));
                Assert.That(createdAdmin.LastName, Is.EqualTo(createdAdminFromDatabase.LastName));

                Assert.That(createdAdminFromDatabase.UserPermissions, Is.Not.Null);
                Assert.That(createdAdminFromDatabase.UserPermissions.Any(p =>
                    p.PermissionLevel == PermissionLevelConstants.Administrator), Is.True);
            });
        }

        [Test]
        public void CreateAdmin_InvalidEmail_ThrowsException()
        {
            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ValidationException>().And.Message.Contains("EmailNotValid"),
                async () =>
                {
                    await CreateAdmin(email: "1234567890123456789012345678901234567890123@");
                }
            );
        }

        [Test]
        public async Task UpdateAdmin_Test()
        {
            // Arrange
            var createAdmin = await CreateAdmin("testEmail@gmail.com");

            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = createAdmin.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    Email = _email,
                    IsDeactivated = false
                }
            };

            // Act
            var updateAdmin = await updateAdminHandler.Handle(command, CancellationToken.None);

            var updatedAdminFromDatabase = await DatabaseContext.User
                .Include(u => u.UserPermissions)
                .FirstAsync(x => x.Id == createAdmin.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateAdmin.Id, Is.EqualTo(createAdmin.Id));
                Assert.That(updateAdmin.Email, Is.Not.EqualTo(createAdmin.Email));
                Assert.That(updateAdmin.FirstName, Is.Not.EqualTo(createAdmin.FirstName));
                Assert.That(updateAdmin.LastName, Is.Not.EqualTo(createAdmin.LastName));

                Assert.That(updatedAdminFromDatabase.Email, Is.EqualTo(command.User.Email));
                Assert.That(updatedAdminFromDatabase.FirstName, Is.EqualTo(command.User.FirstName));
                Assert.That(updatedAdminFromDatabase.LastName, Is.EqualTo(command.User.LastName));
                Assert.That(updatedAdminFromDatabase.IsDeactivated, Is.EqualTo(command.User.IsDeactivated));

                Assert.That(updatedAdminFromDatabase.UserPermissions.Any(p =>
                    p.PermissionLevel == PermissionLevelConstants.Administrator), Is.True);
            });
        }

        [Test]
        public void UpdateAdmin_NonExistentUser_ThrowsException()
        {
            // Arrange
            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = 1234567890,
                    FirstName = "Da",
                    LastName = "Vinci",
                    Email = _email,
                    IsDeactivated = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("UserNotFound"),
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

            var updateAdminHandler = new UpdateAdmin.Handler(DatabaseContext, Mapper);
            var command = new UpdateAdmin.Command()
            {
                User = new Models.V1.User.User()
                {
                    Id = user2.Id,
                    FirstName = "Da",
                    LastName = "Vinci",
                    Email = user1.Email,
                    IsDeactivated = false
                }
            };

            // Assert
            Assert.ThrowsAsync(
                Is.TypeOf<ValidationException>().And.Message.Contains("EmailAlreadyUsed"),
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