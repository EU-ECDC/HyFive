using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1.Facility;
using HyFive.Models.V1.Session;
using HyFive.Services.Facility;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.Facility
{
    public class FacilityTests : ServiceTests
    {
        [Test]
        public async Task CreateAndGetFacilityTest()
        {
            // Arrange
            var facility = (await CreateFacility()).Item1;
            var getFacilityHandler = new GetFacility.Handler(DatabaseContext, Mapper);
            var query = new GetFacility.Query() { FacilityId = facility.Id };

            // Act
            var res = await getFacilityHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.Abbreviation, Is.EqualTo(facility.Abbreviation));
                Assert.That(res.Name, Is.EqualTo(facility.Name));
                Assert.That(res.HERId, Is.EqualTo(facility.HERId));
            });
        }

        [Test]
        public async Task GetFacilityTest()
        {
            // Arrange
            var facility = (await CreateFacility()).Item1;

            var getFacilitiesHandler = new GetFacilities.Handler(DatabaseContext, Mapper);
            var query = new GetFacilities.Query() { };

            // Act
            var res = await getFacilitiesHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.Select(r => r.Id).ToList(), Contains.Item(facility.Id));
                Assert.That(res.Count, Is.EqualTo(DatabaseContext.Facility.ToList().Count));
            });
        }

        [Test]
        public async Task GetFacilitiesForCoordinatorTest()
        {
            // Arrange
            
                var facilityType = await DatabaseContext.FacilityType.FirstOrDefaultAsync() ??
                    new Domain.Place.FacilityType { Name = "Hospital", Code = "HOSP" };

                if (facilityType.Id == 0)
                {
                    DatabaseContext.FacilityType.Add(facilityType);
                    await DatabaseContext.SaveChangesAsync();
                }

                var facility = new Domain.Place.Facility
                {
                    Name = "Coordinator Facility",
                    Abbreviation = "CF",
                    HERId = "HER999",
                    FacilityType = facilityType,
                    City = new Domain.Place.City { Name = "Oslo" }
                };

            var coordinator = new Domain.User.User
            {
                Email = "test@gmail.com",
                FirstName = "Test",
                LastName = "Coordinator",
                Discriminator = nameof(Coordinator),
                IsDeactivated = false
            };
            coordinator.Facility = facility;
            facility.Users.Add(coordinator);
            DatabaseContext.Facility.Add(facility);
                await DatabaseContext.SaveChangesAsync();
            

            var handler = new GetFacilitiesForCoordinator.Handler(DatabaseContext, Mapper);


            // Act
            var facilities = await handler.Handle(
             new GetFacilitiesForCoordinator.Query
             {
                 CoordinatorEmail = "test@gmail.com"
             }, CancellationToken.None);

            // Assert
            Assert.That(facilities.Length, Is.GreaterThan(0));

        }

        [Test]
        public async Task GetFacilityTypesTest()
        {
            // Arrange
            var handler = new GetFacilityTypes.Handler(DatabaseContext, Mapper);
            var existingTypeCodes = await DatabaseContext.FacilityType.Select(i => i.Code).ToListAsync();

            // Act
            var types = await handler.Handle(new GetFacilityTypes.Query(), CancellationToken.None);

            // Assert
            Assert.That(existingTypeCodes.OrderBy(x => x).SequenceEqual(types.OrderBy(x => x.Code).Select(t => t.Code)));
        }

        [Test]
        public async Task GetCoordinatorsForFacilityTest()
        {
            // Arrange
            (var facility, _) = await CreateFacility();
            var handler = new GetCoordinatorsForFacility.Handler(DatabaseContext, Mapper);
            var numberOfCoordinatorsAssignedToFacility =
                await DatabaseContext.Coordinator
                    .Include(k => k.Facility)
                    .CountAsync(k => k.Facility.Id == facility.Id);

            // Act
            var coordinators =
                await handler.Handle(new GetCoordinatorsForFacility.Query() { FacilityId = facility.Id },
                    CancellationToken.None);

            // Assert
            Assert.That(coordinators.ToList(), Has.Count.EqualTo(numberOfCoordinatorsAssignedToFacility));
        }


        [Test]
        public async Task GetObserversForFacilityTest()
        {
            // Arrange
            (var facility, var observer) = await CreateFacility();
            var handler = new GetObserversForFacility.Handler(DatabaseContext, Mapper);
            var numberOfCoordinatorsAssignedToFacility =
                await DatabaseContext.Observer
                    .Include(k => k.Facility)
                    .CountAsync(k => k.Facility.Id == facility.Id);

            // Act
            var observers =
                await handler.Handle(new GetObserversForFacility.Query() { FacilityId = facility.Id },
                    CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(observers.ToList(), Has.Count.EqualTo(numberOfCoordinatorsAssignedToFacility));
                Assert.That(observers.Select(o => o.Id).ToList(), Contains.Item(observer.Id));
            });
        }

        [Test]
        public async Task CreateAndGetPredefinedCommentsTest()
        {
            // Arrange
            var getPredefinedCommentsHandler = new GetPredefinedComments.Handler(DatabaseContext);
            var facilityEntity = await DatabaseContext.Facility.FirstOrDefaultAsync();
            if (facilityEntity == null)
            {
                facilityEntity = new Domain.Place.Facility
                {
                    Name = "Test Facility",
                    Abbreviation = "TF",
                    HERId = "HER123",
                    FacilityType = new Domain.Place.FacilityType { Name = "HospitalTest", Code = "TESTHOSP" },
                    City = new Domain.Place.City { Name = "Oslo" }
                };
                DatabaseContext.Facility.Add(facilityEntity);
                await DatabaseContext.SaveChangesAsync();
            }

            var facilityId = facilityEntity.Id;

            var createPredefinedCommentHandler = new CreatePredefinedComment.Handler(DatabaseContext);

            var commentsRequest = new CreatePredefinedCommentRequest()
            {
                Comment = $"{Guid.NewGuid()}"
            };

            // Act

            var couldCreateComment = await createPredefinedCommentHandler.Handle(
                new CreatePredefinedComment.Command() { NewPredefinedComment = commentsRequest, FacilityId = facilityId }, CancellationToken.None);

            var comments = await getPredefinedCommentsHandler.Handle(new GetPredefinedComments.Query()
            {
                FacilityId = facilityId,
                SessionType = SessionType.ProtectiveEquipment
            }, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(couldCreateComment);
                Assert.That(comments.Any);
                Assert.That(comments, Contains.Item(commentsRequest.Comment));
            });

        }

        [Test]
        public async Task UpdateFacilityTest()
        {
            // Arrange
            (var facility, _) = await CreateFacility();
            var updateFacilityHandler = new UpdateFacility.Handler(DatabaseContext, Mapper);
            var newName = $"A new name and a random value:{Guid.NewGuid()}";
            facility.Name = newName;

            // Act
            await updateFacilityHandler.Handle(new UpdateFacility.Command()
            { Facility = facility }, CancellationToken.None);

            // Assert
            Assert.That((await DatabaseContext.Facility.FirstAsync(i => i.Id == facility.Id)).Name, Is.EqualTo(newName));

        }

        [Test]
        public async Task UpdateFacilityTypeTest()
        {
            // Arrange
            var updateFacilityTypeHandler = new UpdateFacilityType.Handler(DatabaseContext, Mapper);
            var originalType = await DatabaseContext.FacilityType.FirstAsync();
            var newName = $"NAME{Guid.NewGuid()}";

            // Act
            await updateFacilityTypeHandler.Handle(new UpdateFacilityType.Command()
            {
                FacilityType = new FacilityType()
                {
                    Id = originalType.Id,
                    Code = "CODE",
                    Name = newName
                }
            }, CancellationToken.None);

            // Assert
            var typeAfterUpdate = await DatabaseContext.FacilityType.FirstAsync(k => k.Id == originalType.Id);
            Assert.That(typeAfterUpdate.Name, Is.EqualTo(newName));
            Assert.That(typeAfterUpdate.Code, Is.EqualTo(originalType.Code));
        }

        [Test]
        public async Task CreateFacilityTest()
        {
            // Arrange and Act
            (var createFacility, _) = await CreateFacility();
            var createFacilityFromDatabase = await DatabaseContext.Facility.Include(i => i.Departments).FirstOrDefaultAsync(i => i.Id == createFacility.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createFacilityFromDatabase, Is.Not.Null);
                Assert.That(createFacilityFromDatabase.Name, Is.EqualTo(createFacility.Name));
                Assert.That(createFacilityFromDatabase.Abbreviation, Is.EqualTo(createFacility.Abbreviation));
            });

        }

        [Test]
        public async Task CreateFacilityTypeTest()
        {
            // Arrange and Act
            var createType = await CreateFacilityType();
            var createTypeFromDatabase = await DatabaseContext.FacilityType.FirstAsync(i => i.Id == createType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createTypeFromDatabase.Name == createType.Name);
                Assert.That(createTypeFromDatabase.Code == createType.Code);
            });
        }

        [Test]
        public async Task CreateFacilityType_ExistingCode_ThrowsException()
        {
            // Arrange
            _ = await CreateFacilityType(code: "CODE");

            // Act and Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
            {
                await CreateFacilityType(code: "CODE");
            });
        }

        #region Helper-methods

        private async Task<Models.V1.Facility.FacilityType> CreateFacilityType(string code = null)
        {
            var createFacilityTypeHandler = new CreateFacilityType.Handler(DatabaseContext, Mapper);
            var createCommand = new CreateFacilityType.Command()
            {
                FacilityType = new CreateFacilityTypeRequest()
                {
                    Code = code ?? "TEST",
                    Name = "Test"
                }
            };

            var createRes = await createFacilityTypeHandler.Handle(createCommand, new System.Threading.CancellationToken());

            return createRes;
        }

        #endregion

    }
}