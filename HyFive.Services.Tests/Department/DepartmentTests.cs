using HyFive.Services.Department;
using HyFive.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.Department
{
    public class DepartmentTests : ServiceTests
    {
        [Test]
        public async Task GetDepartmentTest()
        {
            // Arrange
            var getDepartmentHandler = new GetDepartment.Handler(DatabaseContext, Mapper);
            var query = new GetDepartment.Query() { Id = 9999 };

            var facility = await DatabaseContext.Facility.FirstOrDefaultAsync();
            if (facility == null)
            {
                facility = new Domain.Place.Facility { Name = "Test Facility" };
                DatabaseContext.Facility.Add(facility);
                await DatabaseContext.SaveChangesAsync();
            }

            var department = new Domain.Place.Department { Id = 9999, FacilityId = facility.Id };
            DatabaseContext.Department.Add(department);
            await DatabaseContext.SaveChangesAsync();

            // Act
            var res = await getDepartmentHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(department.Id, Is.EqualTo(res.Id));
        }
        [Test]
        public async Task GetDepartment_IdDoesNotExist_ReturnsNull()
        {
            // Arrange
            var getDepartmentHandler = new GetDepartment.Handler(DatabaseContext, Mapper);
            var query = new GetDepartment.Query() { Id = 123456789 };

            // Act
            var getDepartmentResults = await getDepartmentHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(getDepartmentResults, Is.Null);
        }

        [Test]
        public async Task GetDepartmentsForFacilityTest()
        {
            // Arrange
            var facility = new Domain.Place.Facility { Id = 9999 };
            var department = new Domain.Place.Department { Id = 9999, FacilityId = facility.Id };
            var department2 = new Domain.Place.Department { Id = 99999, FacilityId = facility.Id };
            DatabaseContext.Facility.Add(facility);
            DatabaseContext.Department.Add(department);
            DatabaseContext.Department.Add(department2);
            await DatabaseContext.SaveChangesAsync();

            var getDepartmentsForFacility = new GetDepartmentsForFacility.Handler(DatabaseContext, Mapper);
            var query = new GetDepartmentsForFacility.Query() { FacilityId = 9999 };

            // Act
            var res = await getDepartmentsForFacility.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res.All(x => x.FacilityId == facility.Id));
            });
        }

        [Test]
        public async Task GetDepartmentsForFacility_FacilityDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            var getDepartmentsForFacility = new GetDepartmentsForFacility.Handler(DatabaseContext, Mapper);
            var query = new GetDepartmentsForFacility.Query() { FacilityId = 123456789 };

            // Act
            var getDepartmentsForFacilityResults = await getDepartmentsForFacility.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(getDepartmentsForFacilityResults.ToList(), Has.Count.EqualTo(0));
            });
        }

        [Test]
        public async Task CreateDepartmentTest()
        {
            // Arrange: seed required data if missing
            if (!await DatabaseContext.Facility.AnyAsync())
            {
                DatabaseContext.Facility.Add(new Domain.Place.Facility { Name = "Test Facility" });
            }

            if (!await DatabaseContext.DepartmentType.AnyAsync())
            {
                DatabaseContext.DepartmentType.Add(new Domain.Place.DepartmentType { Name = "Test Type" });
            }

            if (!await DatabaseContext.Role.AnyAsync())
            {
                DatabaseContext.Role.Add(new Domain.Observation.Role { Name = "Test Role" });
            }

            await DatabaseContext.SaveChangesAsync();

            // Arrange and Act
            var createDepartment = await CreateDepartment();
            var createdDepartmentFromDatabase = await DatabaseContext.Department
                .Include(a => a.Facility)
                .Include(a => a.Roles)
                .FirstOrDefaultAsync(a => a.Id == createDepartment.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createDepartment.Id, Is.GreaterThan(0));
                Assert.That(createDepartment.Name, Is.EqualTo(createdDepartmentFromDatabase.Name));
                Assert.That(createDepartment.FacilityId, Is.EqualTo(createdDepartmentFromDatabase.Facility.Id));
                Assert.That(createDepartment.Roles.Any(r => r.Id == createdDepartmentFromDatabase.Roles.First().Id));
            });
        }

        [Test]
        public async Task CreateDepartmentType_Test()
        {
            // Arrange and Act
            var createDepartmentType = await CreateDepartmentType();
            var createdDepartmentTypeFromDatabase = await DatabaseContext.DepartmentType
                .FirstOrDefaultAsync(a => a.Id == createDepartmentType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createDepartmentType.Id, Is.GreaterThan(0));
                Assert.That(createDepartmentType.Name, Is.EqualTo(createdDepartmentTypeFromDatabase.Name));
                Assert.That(createDepartmentType.Code, Is.EqualTo(createdDepartmentTypeFromDatabase.Code));
            });
        }

        [Test]
        public async Task UpdateDepartmentTest()
        {
            // Arrange
            var roleIds = new List<int>() { 1 };
            var updateDepartment = await CreateDepartment(roleIds: roleIds);
            var updateDepartmentHandler = new UpdateDepartment.Handler(DatabaseContext, Mapper);

            var otherDepartmentType = await DatabaseContext.DepartmentType
                .FirstOrDefaultAsync(at => at.Id != updateDepartment.DepartmentTypeId);

            if (otherDepartmentType == null)
            {
                // Create another department type if none exists
                otherDepartmentType = new Domain.Place.DepartmentType { Name = "Another Test Type", Code = "AAA"};
                DatabaseContext.DepartmentType.Add(otherDepartmentType);
                await DatabaseContext.SaveChangesAsync();
            }

            var otherRole = await DatabaseContext.Role
                .FirstOrDefaultAsync(x => !roleIds.Contains(x.Id));

            if (otherRole == null)
            {
                // Create another role if none exists
                otherRole = new Domain.Observation.Role { Name = "Extra Role" };
                DatabaseContext.Role.Add(otherRole);
                await DatabaseContext.SaveChangesAsync();
            }

            var updateCommand = new UpdateDepartment.Command()
            {
                Id = updateDepartment.Id,
                Name = "Da Vinci",
                DepartmentTypeId = otherDepartmentType.Id,
                Role = new List<Models.V1.Observation.Role>()
                {
                    Mapper.Map<Domain.Observation.Role, Models.V1.Observation.Role>(otherRole)
                }
            };

            // Act
            var updateResults = await updateDepartmentHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(updateDepartment.Id));
                Assert.That(updateResults.Name, Is.Not.EqualTo(updateDepartment.Name));
                Assert.That(updateResults.DepartmentTypeId, Is.Not.EqualTo(updateDepartment.DepartmentTypeId));
                Assert.That(updateResults.Roles.Count, Is.EqualTo(updateCommand.Role.Count));
                Assert.That(updateResults.Roles, Does.Not.Contain(updateDepartment.Roles[0].Id));
            });
        }

        [Test]
        public async Task UpdateDepartmentTest_CanUpdateNameOnly()
        {
            // Arrange
            var updateDepartment = await CreateDepartment();
            var updateDepartmentHandler = new UpdateDepartment.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateDepartment.Command
            {
                Id = updateDepartment.Id,
                Name = "Da Vinci"
            };

            // Act
            var updateResults = await updateDepartmentHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(updateDepartment.Id));
                Assert.That(updateResults.Name, Is.Not.EqualTo(updateDepartment.Name));
                Assert.That(updateResults.DepartmentTypeId, Is.EqualTo(updateDepartment.DepartmentTypeId));
                Assert.That(updateResults.Roles, Has.Count.EqualTo(updateDepartment.Roles.Count));
                Assert.That(updateResults.Roles.Select(r => r.Id), Is.EqualTo(updateDepartment.Roles.Select(r => r.Id)));
            });
        }

        [Test]
        public async Task UpdateDepartmentTypeTest()
        {
            // Arrange
            var updateDepartmentType = await CreateDepartmentType();
            var updateDepartmentTypeHandler = new UpdateDepartmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateDepartmentType.Command()
            {
                DepartmentType = new Models.V1.Facility.DepartmentType()
                {
                    Id = updateDepartmentType.Id,
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateDepartmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(updateDepartmentType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.DepartmentType.Name));
            });
        }

        [Test]
        public async Task UpdateDepartmentType_ShouldNotBeAbleToUpdateCode_OnlyUpdatesName()
        {
            // Arrange
            var createDepartmentType = await CreateDepartmentType(code: "HELLO");
            var updateDepartmentTypeHandler = new UpdateDepartmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateDepartmentType.Command()
            {
                DepartmentType = new Models.V1.Facility.DepartmentType()
                {
                    Id = createDepartmentType.Id,
                    Code = "PROVER",
                    Name = "ProverAEndreKode"
                }
            };

            // Act
            var updateResults = await updateDepartmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createDepartmentType.Id));
                Assert.That(updateResults.Code, Is.EqualTo(createDepartmentType.Code));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.DepartmentType.Name));
            });
        }

        #region Helper-methods

        private async Task<Models.V1.Facility.Department> CreateDepartment(int institutionsId = 0, List<int> roleIds = null, int departmentTypeId = 0)
        {
            var createDepartmentHandler = new CreateDepartment.Handler(DatabaseContext, Mapper);

            var facility = await DatabaseContext.Facility.FirstOrDefaultAsync();
            if (facility == null)
            {
                facility = new Domain.Place.Facility { Name = "Test Facility" };
                DatabaseContext.Facility.Add(facility);
                await DatabaseContext.SaveChangesAsync();
            }

            var deptType = await DatabaseContext.DepartmentType.FirstOrDefaultAsync();
            if (deptType == null)
            {
                deptType = new Domain.Place.DepartmentType { Name = "Test Type" };
                DatabaseContext.DepartmentType.Add(deptType);
                await DatabaseContext.SaveChangesAsync();
            }

            var role = await DatabaseContext.Role.FirstOrDefaultAsync();
            if (role == null)
            {
                role = new Domain.Observation.Role { Name = "Test Role" };
                DatabaseContext.Role.Add(role);
                await DatabaseContext.SaveChangesAsync();
            }

            var createCommand = new CreateDepartment.Command()
            {
                Request = new Models.V1.Facility.CreateDepartmentRequest()
                {
                    Name = "Test",
            FacilityId = institutionsId != 0 ? institutionsId : facility.Id,
            DepartmentTypeId = departmentTypeId != 0 ? departmentTypeId : deptType.Id,
            RoleIds = roleIds ?? new List<int> { role.Id }
                }
            };

            var createResults = await createDepartmentHandler.Handle(createCommand, new System.Threading.CancellationToken());
            
            return createResults;
        }

        private async Task<Models.V1.Facility.DepartmentType> CreateDepartmentType(string code = null)
        {
            var createDepartmentTypeHandler = new CreateDepartmentType.Handler(DatabaseContext, Mapper);
            var createCommand = new CreateDepartmentType.Command()
            {
                DepartmentType = new Models.V1.Facility.DepartmentType()
                {
                    Code = code ?? "TEST",
                    Name = "Test"
                }
            };

            var createResults = await createDepartmentTypeHandler.Handle(createCommand, new System.Threading.CancellationToken());

            return createResults;
        }

        #endregion
    }
}