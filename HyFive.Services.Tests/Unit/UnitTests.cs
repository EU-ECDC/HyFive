using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Services.Unit;
using System;

namespace HyFive.Services.Tests.Unit
{
    public class UnitTests : ServiceTests
    {
        [Test]
        public async Task GetUnitTest()
        {
            // Arrange
            var getUnitHandler = new GetUnit.Handler(DatabaseContext, Mapper);

            var facility = new Domain.Place.Facility { Id = 9999 };
            var unit = new Domain.Place.Unit { Id = 9999, Facility = facility };
            DatabaseContext.Unit.Add(unit);
            DatabaseContext.SaveChanges();

            // Act
            var query = new GetUnit.Query() { Id = 9999, FacilityId = facility.Id };
            var res = await getUnitHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(unit.Id, Is.EqualTo(res.Id));
        }

        [Test]
        public async Task GetUnitsForFacilityTest()
        {
            // Arrange
            var facility = new Domain.Place.Facility { Id = 9999 };
            var unit = new Domain.Place.Unit { Id = 9999, Facility = facility };
            var unit2 = new Domain.Place.Unit { Id = 99999, Facility = facility };
            DatabaseContext.Facility.Add(facility);
            DatabaseContext.Unit.Add(unit);
            DatabaseContext.Unit.Add(unit2);

            var otherFacility = new Domain.Place.Facility { Id = 1111 };
            var otherUnit = new Domain.Place.Unit { Id = 1111, Facility = otherFacility };
            var otherUnit2 = new Domain.Place.Unit { Id = 11111, Facility = otherFacility };
            DatabaseContext.Facility.Add(otherFacility);
            DatabaseContext.Unit.Add(otherUnit);
            DatabaseContext.Unit.Add(otherUnit2);

            DatabaseContext.SaveChanges();

            var getUnitsForFacility = new GetUnitsForFacility.Handler(DatabaseContext, Mapper);
            var query = new GetUnitsForFacility.Query() { FacilityId = facility.Id };

            // Act
            var res = await getUnitsForFacility.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res, Has.All.Property(nameof(Models.V1.Facility.Unit.FacilityId)).EqualTo(facility.Id));
                Assert.That(res.Any(x => x.Id == unit.Id));
                Assert.That(res.Any(x => x.Id == unit2.Id));
            });
        }

        [Test]
        public async Task CreateUnitTest()
        {
            // Arrange and Act
            var facility = DatabaseContext.Facility.Include(i => i.Departments).First();
            var createdUnit = await CreateUnit(facility.Id);
            var createdUnitFromDatabase = DatabaseContext.Unit
                .Include(k => k.Facility)
                .Include(k => k.Departments)
                .FirstOrDefault(k => k.Id == createdUnit.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createdUnit.Id, Is.GreaterThan(0));
                Assert.That(createdUnit.Name, Is.EqualTo(createdUnitFromDatabase.Name));
                Assert.That(createdUnit.FacilityId, Is.EqualTo(createdUnitFromDatabase.Facility.Id));
                Assert.That(createdUnit.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(createdUnitFromDatabase.Departments.Select(a => a.Id).OrderBy(x => x)));
                Assert.That(createdUnit.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(facility.Departments.Select(a => a.Id).OrderBy(x => x)));
            });
        }

        [Test]
        public void CreateUnitTest_NonExistentFacility()
        {
            // Arrange 
            var nonExistentFacilityId = 9999;

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("Could not find"),
                async () =>
                {
                    await CreateUnit(nonExistentFacilityId);
                }
            );
        }

        [Test]
        public void CreateUnitTest_CannotCreateUnitWithDepartmentForAnotherFacility()
        {
            // Arrange
            var facility = DatabaseContext.Facility.Include(i => i.Departments).First();
            var otherFacility = DatabaseContext.Facility.Include(i => i.Departments).First(x => x.Id != facility.Id);

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<InvalidOperationException>().And.Message.Contains("not associated with an facility"),
                (AsyncTestDelegate)(async () =>
                {
                    await CreateUnit(facility.Id, otherFacility.Departments.ToList());
                })
            );
        }

        [Test]
        public async Task UpdateUnitTest()
        {
            // Arrange
            var facility = DatabaseContext.Facility.Include(i => i.Departments).First();
            var departments = facility.Departments.Take(1);
            var createdUnit = await CreateUnit(facility.Id);
            var updatedUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = createdUnit.Id,
                    Name = "Leverpostei",
                    FacilityId = facility.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(departments)
                }
            };

            // Act
            var updateResults = await updatedUnitHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdUnit.Id));
                Assert.That(updateResults.Name, Is.Not.EqualTo(createdUnit.Name));
                Assert.That(updateResults.FacilityId, Is.EqualTo(createdUnit.FacilityId));
                Assert.That(updateResults.Departments.Count, Is.EqualTo(updateCommand.Unit.Departments.Count));
            });
        }

        [Test]
        public async Task UpdateUnitTest_CannotUpdateFacilityIdOfUnit()
        {
            // Arrange
            var facility = DatabaseContext.Facility.Include(i => i.Departments).First();
            var departments = facility.Departments.Take(1);
            var createdUnit = await CreateUnit(facility.Id);
            var updatedUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);
            var newFacility = DatabaseContext.Facility.Include(i => i.Departments).First(x => x.Id != facility.Id);
            var updateCommand = new UpdateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = createdUnit.Id,
                    Name = "Leverpostei",
                    FacilityId = newFacility.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(departments)
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<Exception>().And.Message.Contains("not associated with a facility"),
                async () =>
                {
                    await updatedUnitHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task UpdateUnitTest_CannotUpdateUnitWithDepartmentForAnotherFacility()
        {
            // Arrange
            var facility = DatabaseContext.Facility.Include(i => i.Departments).First();
            var createdUnit = await CreateUnit(facility.Id);
            var updateUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);

            var otherFacility = DatabaseContext.Facility.Include(i => i.Departments).First(x => x.Id != facility.Id);
            var updateCommand = new UpdateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = createdUnit.Id,
                    Name = "Leverpostei",
                    FacilityId = facility.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(otherFacility.Departments)
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<InvalidOperationException>().And.Message.Contains("not associated facility"),
                async () =>
                {
                    await updateUnitHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #region Helper-methods

        private async Task<Models.V1.Facility.Unit> CreateUnit(int facilitiesId, List<Domain.Place.Department> departments = null)
        {
            var createUnitHandler = new CreateUnit.Handler(DatabaseContext, Mapper);
            var departmentForFacility = departments ?? DatabaseContext.Facility
                .Include(i => i.Departments)
                .FirstOrDefault(x => x.Id == facilitiesId)?.Departments.ToList();
            var departmentForFacilityModel =
                Mapper.Map<List<Domain.Place.Department>, List<Models.V1.Facility.Department>>(departmentForFacility ?? new List<Domain.Place.Department>());
            var createCommand = new CreateUnit.Command()
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Name = "Test",
                    FacilityId = facilitiesId,
                    Departments = departmentForFacilityModel
                }
            };

            var createResult = await createUnitHandler.Handle(createCommand, new System.Threading.CancellationToken());

            return createResult;
        }

        #endregion
    }
}