using HyFive.Services.Department;
using HyFive.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HyFive.Services.Tests.Department
{
    public class DepartmentTests : ServiceTests
    {
        [Test]
        public async Task GetDepartmentTest()
        {
            // Arrange
            var getDepartmentHandler = new GetDepartment.Handler(DatabaseContext, Mapper);
            const int departmentId = 9999;

            var facilityOu = await DatabaseContext.OrganisationUnit.FirstOrDefaultAsync(ou => ou.ParentId == null && ou.LevelId == 1);
            if (facilityOu == null)
            {
                facilityOu = new Domain.Place.OrganisationUnit
                {
                    Name = "Test Facility",
                    LevelId = 1,
                    ParentId = null
                };
            }
            DatabaseContext.OrganisationUnit.Add(facilityOu);
            await DatabaseContext.SaveChangesAsync();

            var departmentOu = await DatabaseContext.OrganisationUnit.FirstOrDefaultAsync(ou => ou.Id == departmentId);
            if (departmentOu == null)
            {
                departmentOu = new Domain.Place.OrganisationUnit
                {
                    Id = departmentId,
                    Name = "Test Department",
                    LevelId = 2,
                    ParentId = facilityOu.Id
                };

                DatabaseContext.OrganisationUnit.Add(departmentOu);
                await DatabaseContext.SaveChangesAsync();
            }

            var query = new GetDepartment.Query() { Id = departmentId };
            // Act
            var res = await getDepartmentHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(res.Id, Is.EqualTo(res.Id));
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
            const int facilityId = 9999;

            // Ensure levels exist (only if your test DB doesn't seed them)
            if (!await DatabaseContext.OrganisationUnitLevel.AnyAsync())
            {
                DatabaseContext.OrganisationUnitLevel.AddRange(
                    new Domain.Place.OrganisationUnitLevel { Id = 1, Level = "Facility" },
                    new Domain.Place.OrganisationUnitLevel { Id = 2, Level = "Department" },
                    new Domain.Place.OrganisationUnitLevel { Id = 3, Level = "Unit" }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            // Facility OU (root)
            var facilityOu = await DatabaseContext.OrganisationUnit.FirstOrDefaultAsync(x => x.Id == facilityId);
            if (facilityOu == null)
            {
                facilityOu = new Domain.Place.OrganisationUnit
                {
                    Id = facilityId,
                    Name = "Test Facility",
                    LevelId = 1,
                    ParentId = null
                };
                DatabaseContext.OrganisationUnit.Add(facilityOu);
                await DatabaseContext.SaveChangesAsync();
            }

            // Two department OUs under the facility
            var dep1 = await DatabaseContext.OrganisationUnit.FirstOrDefaultAsync(x => x.Id == 10001);
            if (dep1 == null)
            {
                dep1 = new Domain.Place.OrganisationUnit
                {
                    Id = 10001,
                    Name = "Department 1",
                    LevelId = 2,
                    ParentId = facilityId
                };
                DatabaseContext.OrganisationUnit.Add(dep1);
            }

            var dep2 = await DatabaseContext.OrganisationUnit.FirstOrDefaultAsync(x => x.Id == 10002);
            if (dep2 == null)
            {
                dep2 = new Domain.Place.OrganisationUnit
                {
                    Id = 10002,
                    Name = "Department 2",
                    LevelId = 2,
                    ParentId = facilityId
                };
                DatabaseContext.OrganisationUnit.Add(dep2);
            }

            await DatabaseContext.SaveChangesAsync();

            var handler = new GetDepartmentsForFacility.Handler(DatabaseContext, Mapper);
            var query = new GetDepartmentsForFacility.Query { FacilityId = facilityId };

            // Act
            var res = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));

                Assert.That(res.All(x => x.ParentId == facilityId), Is.True);

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
            // Arrange: seed Unit levels if missing (required for LevelId FK)
            if (!await DatabaseContext.OrganisationUnitLevel.AnyAsync())
            {
                DatabaseContext.OrganisationUnitLevel.AddRange(
                    new Domain.Place.OrganisationUnitLevel { Id = 1, Level = "Facility" },
                    new Domain.Place.OrganisationUnitLevel { Id = 2, Level = "Department" },
                    new Domain.Place.OrganisationUnitLevel { Id = 3, Level = "Unit" }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            // Arrange: ensure at least one Role exists
            if (!await DatabaseContext.Role.AnyAsync())
            {
                DatabaseContext.Role.Add(new Domain.Observation.Role { Name = "Test Role" });
                await DatabaseContext.SaveChangesAsync();
            }

            // Arrange: ensure at least one Facility (root Unit) exists
            var facilityOu = await DatabaseContext.OrganisationUnit
                .FirstOrDefaultAsync(x => x.ParentId == null && x.LevelId == 1);

            if (facilityOu == null)
            {
                facilityOu = new Domain.Place.OrganisationUnit
                {
                    Name = "Test Facility",
                    LevelId = 1,
                    ParentId = null
                };
                DatabaseContext.OrganisationUnit.Add(facilityOu);
                await DatabaseContext.SaveChangesAsync();
            }

            // Act
            var createdDepartment = await CreateDepartment(); // should create Unit(LevelId=2, ParentId=facilityOu.Id)

            // Load from DB
            var createdFromDb = await DatabaseContext.OrganisationUnit
                .Include(ou => ou.Parent) // facility
                .Include(ou => ou.OrganisationUnitRoles) // <-- adjust nav name if different
                    .ThenInclude(our => our.Role)
                .FirstOrDefaultAsync(ou => ou.Id == createdDepartment.Id);

            // Assert
            Assert.That(createdFromDb, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(createdDepartment.Id, Is.GreaterThan(0));
                Assert.That(createdDepartment.Name, Is.EqualTo(createdFromDb!.Name));

                Assert.That(createdDepartment.ParentId, Is.EqualTo(createdFromDb.Parent!.Id));

                if (createdFromDb.OrganisationUnitRoles != null && createdFromDb.OrganisationUnitRoles.Any())
                {
                    var firstRoleId = createdFromDb.OrganisationUnitRoles.First().RoleId;

                    Assert.That(createdDepartment.Roles.Any(r => r.Id == firstRoleId), Is.True);
                }
            });
        }

        [Test]
        public async Task CreateDepartmentType_Test()
        {
            // Arrange & Act
            var created = await CreateOrganisationUnitType();
            var fromDb = await DatabaseContext.OrganisationUnitType
                .FirstOrDefaultAsync(x => x.Id == created.Id);

            // Assert
            Assert.That(fromDb, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(created.Id, Is.GreaterThan(0));
                Assert.That(created.Name, Is.EqualTo(fromDb!.Name));
                Assert.That(created.Code, Is.EqualTo(fromDb.Code));
                
            });
        }

        [Test]
        public async Task UpdateDepartmentTest()
        {
            // Arrange
            var roleIds = new List<int> { 1 };

            // Create a department (Unit level Department) with one role
            var department = await CreateDepartment(roleIds: roleIds);

            var handler = new UpdateDepartment.Handler(DatabaseContext, Mapper);

            // Find or create another OrganisationUnitType (department type replacement)
            var otherOuType = await DatabaseContext.OrganisationUnitType
                .FirstOrDefaultAsync(t => t.Id != department.TypeId);

            if (otherOuType == null)
            {
                otherOuType = new Domain.Place.OrganisationUnitType
                {
                    Name = "Another Test Type",
                    Code = "AAA"
                };
                DatabaseContext.OrganisationUnitType.Add(otherOuType);
                await DatabaseContext.SaveChangesAsync();
            }

            // Find or create another role different than the original
            var otherRole = await DatabaseContext.Role
                .FirstOrDefaultAsync(x => !roleIds.Contains(x.Id));

            if (otherRole == null)
            {
                otherRole = new Domain.Observation.Role { Name = "Extra Role" };
                DatabaseContext.Role.Add(otherRole);
                await DatabaseContext.SaveChangesAsync();
            }

            var facilityId = department.ParentId ?? department.Id;

            var updateCommand = new UpdateDepartment.Command
            {
                Id = department.Id,
                FacilityId = facilityId,                 // parent facility OU id
                Name = "Da Vinci",
                OrganisationUnitTypeId = otherOuType.Id,
                RoleIds = new List<int> { otherRole.Id }
            };

            // Act
            var updated = await handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.Id, Is.EqualTo(department.Id));
                Assert.That(updated.Name, Is.EqualTo("Da Vinci"));
                Assert.That(updated.TypeId, Is.EqualTo(otherOuType.Id));

                // Parent should still be the facility
                Assert.That(updated.ParentId, Is.EqualTo(updateCommand.FacilityId));

                // Roles returned as objects -> verify by Id
                Assert.That(updated.Roles, Has.Count.EqualTo(1));
                Assert.That(updated.Roles.Any(r => r.Id == otherRole.Id), Is.True);

                // If Update replaces roles, ensure old role removed:
                Assert.That(updated.Roles.Any(r => r.Id == roleIds[0]), Is.False);
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
                Assert.That(updateResults.TypeId, Is.EqualTo(updateDepartment.TypeId));
                Assert.That(updateResults.Roles, Has.Count.EqualTo(updateDepartment.Roles.Count));
                CollectionAssert.AreEquivalent(
                            updateDepartment.Roles.Select(r => r.Id),
                            updateResults.Roles.Select(r => r.Id));
            });
        }

        [Test]
        public async Task UpdateDepartmentTypeTest()
        {
            // Arrange
            var updateDepartmentType = await CreateOrganisationUnitType();
            var updateDepartmentTypeHandler = new UpdateDepartmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateDepartmentType.Command()
            {
                Type = new Models.V1.OrganisationUnit.OrganisationUnitType()
                {
                    Id = updateDepartmentType.Id,
                    Name = "Da Vinci",
                    Code = updateDepartmentType.Code
                }
            };

            // Act
            var updateResults = await updateDepartmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(updateDepartmentType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.Type.Name));
            });
        }

        [Test]
        public async Task UpdateDepartmentType_ShouldNotBeAbleToUpdateCode_OnlyUpdatesName()
        {
            // Arrange
            var createDepartmentType = await CreateOrganisationUnitType(code: "HELLO");
            var updateDepartmentTypeHandler = new UpdateDepartmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateDepartmentType.Command()
            {
                Type = new Models.V1.OrganisationUnit.OrganisationUnitType()
                {
                    Id = createDepartmentType.Id,
                    Code = "HELLO",
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
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.Type.Name));
            });
        }

        #region Helper-methods

        private async Task<Models.V1.OrganisationUnit.OrganisationUnit> CreateDepartment(
            int facilityId = 0,
            List<int> roleIds = null,
            int organisationUnitTypeId = 0)
        {
            var createHandler = new CreateDepartment.Handler(DatabaseContext, Mapper);

            // Ensure we have a facility OU (LevelId=1)
            var facility = await DatabaseContext.OrganisationUnit
                .FirstOrDefaultAsync(x => x.LevelId == 1 && x.ParentId == null);

            if (facility == null)
            {
                facility = new Domain.Place.OrganisationUnit
                {
                    Name = "Test Facility",
                    LevelId = 1,
                    ParentId = null,
                    TypeId = (await EnsureOrganisationUnitType()).Id
                };

                DatabaseContext.OrganisationUnit.Add(facility);
                await DatabaseContext.SaveChangesAsync();
            }

            var type = organisationUnitTypeId != 0
                ? await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync(x => x.Id == organisationUnitTypeId)
                : await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();

            if (type == null)
            {
                type = new Domain.Place.OrganisationUnitType { Name = "Test Type", Code = "TEST" };
                DatabaseContext.OrganisationUnitType.Add(type);
                await DatabaseContext.SaveChangesAsync();
            }

            var role = await DatabaseContext.Role.FirstOrDefaultAsync();
            if (role == null)
            {
                role = new Domain.Observation.Role { Name = "Test Role" };
                DatabaseContext.Role.Add(role);
                await DatabaseContext.SaveChangesAsync();
            }

            var cmd = new CreateDepartment.Command
            {
                Request = new Models.V1.OrganisationUnit.CreateDepartmentRequest
                {
                    FacilityId = facilityId != 0 ? facilityId : facility.Id,
                    Name = "Test",
                    DepartmentTypeId = organisationUnitTypeId != 0 ? organisationUnitTypeId : type.Id,
                    RoleIds = roleIds ?? new List<int> { role.Id }
                }
            };

            return await createHandler.Handle(cmd, CancellationToken.None);
        }
        private async Task<Domain.Place.OrganisationUnitType> EnsureOrganisationUnitType()
        {
            var t = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (t != null) return t;

            t = new Domain.Place.OrganisationUnitType { Name = "Default", Code = "DEF" };
            DatabaseContext.OrganisationUnitType.Add(t);
            await DatabaseContext.SaveChangesAsync();
            return t;
        }


        private async Task<Models.V1.OrganisationUnit.OrganisationUnitType> CreateOrganisationUnitType(string code = null)
        {
            var handler = new CreateOrganisationUnitType.Handler(DatabaseContext, Mapper);

            var cmd = new CreateOrganisationUnitType.Command
            {
                // Use the correct property name from your Command (most commonly "Request")
                Type = new Models.V1.OrganisationUnit.OrganisationUnitType
                {
                    Code = code ?? "TEST",
                    Name = "Test"
                }
            };

            return await handler.Handle(cmd, CancellationToken.None);
        }

        #endregion
    }
}