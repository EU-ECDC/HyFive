using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Services.Unit;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.Unit
{
    public class UnitTests : ServiceTests
    {
        [Test]
        public async Task GetUnitTest()
        {
            // Arrange
            var getUnitHandler = new GetUnit.Handler(DatabaseContext);

            var unitLevel = DatabaseContext.OrganisationUnitLevel
                .FirstOrDefault(x => x.Level == OrganisationUnitLevels.Unit);

            if (unitLevel == null)
            {
                unitLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Unit
                };
                DatabaseContext.OrganisationUnitLevel.Add(unitLevel);
                DatabaseContext.SaveChanges();
            }

            var facilityLevel = DatabaseContext.OrganisationUnitLevel
                .FirstOrDefault(x => x.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                DatabaseContext.SaveChanges();
            }

            var facility = new Domain.Place.OrganisationUnit
            {
                Id = 9998,
                Name = "Test Facility",
                LevelId = facilityLevel.Id
            };

            var unit = new Domain.Place.OrganisationUnit
            {
                Id = 9999,
                Name = "Test Unit",
                ParentId = facility.Id,
                LevelId = unitLevel.Id
            };

            DatabaseContext.OrganisationUnit.Add(facility);
            DatabaseContext.OrganisationUnit.Add(unit);
            DatabaseContext.SaveChanges();

            // Act
            var query = new GetUnit.Query
            {
                Id = unit.Id,
                FacilityId = facility.Id
            };

            var res = await getUnitHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(unit.Id));
        }

        [Test]
        public async Task GetUnitsForFacilityTest()
        {
            // Arrange
            var facilityLevel = DatabaseContext.OrganisationUnitLevel
                .FirstOrDefault(x => x.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var unitLevel = DatabaseContext.OrganisationUnitLevel
                .FirstOrDefault(x => x.Level == OrganisationUnitLevels.Unit);

            if (unitLevel == null)
            {
                unitLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Unit
                };
                DatabaseContext.OrganisationUnitLevel.Add(unitLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var facility = new Domain.Place.OrganisationUnit
            {
                Id = 9999,
                Name = "Facility A",
                LevelId = facilityLevel.Id
            };

            var unit = new Domain.Place.OrganisationUnit
            {
                Id = 9998,
                Name = "Unit A1",
                ParentId = facility.Id,
                LevelId = unitLevel.Id
            };

            var unit2 = new Domain.Place.OrganisationUnit
            {
                Id = 9997,
                Name = "Unit A2",
                ParentId = facility.Id,
                LevelId = unitLevel.Id
            };

            var otherFacility = new Domain.Place.OrganisationUnit
            {
                Id = 1111,
                Name = "Facility B",
                LevelId = facilityLevel.Id
            };

            var otherUnit = new Domain.Place.OrganisationUnit
            {
                Id = 1112,
                Name = "Unit B1",
                ParentId = otherFacility.Id,
                LevelId = unitLevel.Id
            };

            var otherUnit2 = new Domain.Place.OrganisationUnit
            {
                Id = 1113,
                Name = "Unit B2",
                ParentId = otherFacility.Id,
                LevelId = unitLevel.Id
            };

            DatabaseContext.OrganisationUnit.AddRange(
                facility, unit, unit2,
                otherFacility, otherUnit, otherUnit2);

            await DatabaseContext.SaveChangesAsync();

            var getUnitsForFacility = new GetUnitsForFacility.Handler(DatabaseContext, Mapper);
            var query = new GetUnitsForFacility.Query { FacilityId = facility.Id };

            // Act
            var res = (await getUnitsForFacility.Handle(query, CancellationToken.None)).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.EqualTo(2));
                Assert.That(res.Any(x => x.Id == unit.Id), Is.True);
                Assert.That(res.Any(x => x.Id == unit2.Id), Is.True);
                Assert.That(res.All(x => x.FacilityId == facility.Id), Is.True);
            });
        }

        [Test]
        public async Task CreateUnitTest()
        {
            // Arrange
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var departmentLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Department);

            if (departmentLevel == null)
            {
                departmentLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Department
                };
                DatabaseContext.OrganisationUnitLevel.Add(departmentLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var unitLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Unit);

            if (unitLevel == null)
            {
                unitLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Unit
                };
                DatabaseContext.OrganisationUnitLevel.Add(unitLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var organisationUnitType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (organisationUnitType == null)
            {
                organisationUnitType = new Domain.Place.OrganisationUnitType
                {
                    Name = "General",
                    Code = "GEN"
                };
                DatabaseContext.OrganisationUnitType.Add(organisationUnitType);
                await DatabaseContext.SaveChangesAsync();
            }

            var facility = new Domain.Place.OrganisationUnit
            {
                Name = "Main Facility",
                LevelId = facilityLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(facility);
            await DatabaseContext.SaveChangesAsync();

            var department = new Domain.Place.OrganisationUnit
            {
                Name = "Cardiology",
                ParentId = facility.Id,
                LevelId = departmentLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(department);
            await DatabaseContext.SaveChangesAsync();

            // Act
            var createdUnit = await CreateUnit(facility.Id, department.Id);

            var createdUnitFromDatabase = await DatabaseContext.OrganisationUnit
                .Include(x => x.Parent)
                .Include(x => x.LevelRef)
                .FirstOrDefaultAsync(x => x.Id == createdUnit.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createdUnit.Id, Is.GreaterThan(0));
                Assert.That(createdUnitFromDatabase, Is.Not.Null);
                Assert.That(createdUnit.Name, Is.EqualTo(createdUnitFromDatabase.Name));
                Assert.That(createdUnitFromDatabase.ParentId, Is.EqualTo(department.Id));
                Assert.That(createdUnitFromDatabase.LevelRef.Level, Is.EqualTo(OrganisationUnitLevels.Unit));
            });
        }

        [Test]
        public void CreateUnitTest_NonExistentDepartment()
        {
            // Arrange
            var nonExistentDepartmentId = 9999;

            // Act & Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("DepartmentNotFound"),
                async () =>
                {
                    await CreateUnit(
                        facilityId: 1,
                        departmentId: nonExistentDepartmentId);
                });
        }

        [Test]
        public async Task CreateUnitTest_CannotCreateUnitWithDepartmentForAnotherFacility()
        {
            // Arrange
            var facilityLevel = DatabaseContext.OrganisationUnitLevel
                .FirstOrDefault(x => x.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                DatabaseContext.SaveChanges();
            }

            var departmentLevel = DatabaseContext.OrganisationUnitLevel
                .FirstOrDefault(x => x.Level == OrganisationUnitLevels.Department);

            if (departmentLevel == null)
            {
                departmentLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Department
                };
                DatabaseContext.OrganisationUnitLevel.Add(departmentLevel);
                DatabaseContext.SaveChanges();
            }

            var organisationUnitType = DatabaseContext.OrganisationUnitType.FirstOrDefault();
            if (organisationUnitType == null)
            {
                organisationUnitType = new Domain.Place.OrganisationUnitType
                {
                    Name = "General",
                    Code = "GEN"
                };
                DatabaseContext.OrganisationUnitType.Add(organisationUnitType);
                DatabaseContext.SaveChanges();
            }

            var facilityA = new Domain.Place.OrganisationUnit
            {
                Name = "Facility A",
                LevelId = facilityLevel.Id,
                TypeId = organisationUnitType.Id
            };

            var facilityB = new Domain.Place.OrganisationUnit
            {
                Name = "Facility B",
                LevelId = facilityLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.AddRange(facilityA, facilityB);
            DatabaseContext.SaveChanges();

            var departmentOfFacilityB = new Domain.Place.OrganisationUnit
            {
                Name = "Dept B1",
                ParentId = facilityB.Id,
                LevelId = departmentLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(departmentOfFacilityB);
            DatabaseContext.SaveChanges();

            // Act + Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("DepartmentNotLinkedToFacility"),
                async () =>
                {
                    await CreateUnit(
                        facilityId: facilityA.Id,
                        departmentId: departmentOfFacilityB.Id);
                });
        }

        [Test]
        public async Task UpdateUnitTest()
        {
            // Arrange
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var departmentLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Department);

            if (departmentLevel == null)
            {
                departmentLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Department
                };
                DatabaseContext.OrganisationUnitLevel.Add(departmentLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var unitLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Unit);

            if (unitLevel == null)
            {
                unitLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Unit
                };
                DatabaseContext.OrganisationUnitLevel.Add(unitLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var organisationUnitType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (organisationUnitType == null)
            {
                organisationUnitType = new Domain.Place.OrganisationUnitType
                {
                    Name = "General",
                    Code = "GEN"
                };
                DatabaseContext.OrganisationUnitType.Add(organisationUnitType);
                await DatabaseContext.SaveChangesAsync();
            }

            var facility = new Domain.Place.OrganisationUnit
            {
                Name = "Main Facility",
                LevelId = facilityLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(facility);
            await DatabaseContext.SaveChangesAsync();

            var department = new Domain.Place.OrganisationUnit
            {
                Name = "Dept A",
                ParentId = facility.Id,
                LevelId = departmentLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(department);
            await DatabaseContext.SaveChangesAsync();

            var unit = new Domain.Place.OrganisationUnit
            {
                Name = "Original Unit",
                ParentId = department.Id,
                LevelId = unitLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(unit);
            await DatabaseContext.SaveChangesAsync();

            var updatedUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);

            var updateCommand = new UpdateUnit.Command
            {
                Request = new UpdateUnitRequest
                {
                    Id = unit.Id,
                    Name = "Leverpostei",
                    FacilityId = facility.Id,
                    DepartmentIds = new List<int> { department.Id },
                    Abbreviation = "LEV",
                    Description = "Updated description"
                }
            };

            // Act
            var updateResults = await updatedUnitHandler.Handle(updateCommand, CancellationToken.None);

            var updatedFromDb = await DatabaseContext.OrganisationUnit
                .FirstOrDefaultAsync(x => x.Id == unit.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(unit.Id));
                Assert.That(updateResults.Name, Is.EqualTo("Leverpostei"));
                Assert.That(updatedFromDb, Is.Not.Null);
                Assert.That(updatedFromDb.Name, Is.EqualTo("Leverpostei"));
                Assert.That(updatedFromDb.ParentId, Is.EqualTo(department.Id));
                Assert.That(updatedFromDb.Abbreviation, Is.EqualTo("LEV"));
                Assert.That(updatedFromDb.Description, Is.EqualTo("Updated description"));
            });
        }

        [Test]
        public async Task UpdateUnitTest_CannotUpdateUnitWithDepartmentForAnotherFacility()
        {
            // Arrange
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var departmentLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Department);

            if (departmentLevel == null)
            {
                departmentLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Department
                };
                DatabaseContext.OrganisationUnitLevel.Add(departmentLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var unitLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Unit);

            if (unitLevel == null)
            {
                unitLevel = new Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Unit
                };
                DatabaseContext.OrganisationUnitLevel.Add(unitLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var organisationUnitType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (organisationUnitType == null)
            {
                organisationUnitType = new Domain.Place.OrganisationUnitType
                {
                    Name = "General",
                    Code = "GEN"
                };
                DatabaseContext.OrganisationUnitType.Add(organisationUnitType);
                await DatabaseContext.SaveChangesAsync();
            }

            var facilityA = new Domain.Place.OrganisationUnit
            {
                Name = "Facility A",
                LevelId = facilityLevel.Id,
                TypeId = organisationUnitType.Id
            };

            var facilityB = new Domain.Place.OrganisationUnit
            {
                Name = "Facility B",
                LevelId = facilityLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.AddRange(facilityA, facilityB);
            await DatabaseContext.SaveChangesAsync();

            var departmentA = new Domain.Place.OrganisationUnit
            {
                Name = "Dept A",
                ParentId = facilityA.Id,
                LevelId = departmentLevel.Id,
                TypeId = organisationUnitType.Id
            };

            var departmentB = new Domain.Place.OrganisationUnit
            {
                Name = "Dept B",
                ParentId = facilityB.Id,
                LevelId = departmentLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.AddRange(departmentA, departmentB);
            await DatabaseContext.SaveChangesAsync();

            var unit = new Domain.Place.OrganisationUnit
            {
                Name = "Unit A1",
                ParentId = departmentA.Id,
                LevelId = unitLevel.Id,
                TypeId = organisationUnitType.Id
            };

            DatabaseContext.OrganisationUnit.Add(unit);
            await DatabaseContext.SaveChangesAsync();

            var updateUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);

            var updateCommand = new UpdateUnit.Command
            {
                Request = new UpdateUnitRequest
                {
                    Id = unit.Id,
                    Name = "Leverpostei",
                    FacilityId = facilityA.Id,
                    DepartmentIds = new List<int> { departmentB.Id },
                }
            };

            // Act + Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("DepartmentNotLinkedToFacility"),
                async () =>
                {
                    await updateUnitHandler.Handle(updateCommand, CancellationToken.None);
                });
        }

        #region Helper-methods

        private async Task<Models.V1.OrganisationUnit.UnitResponse> CreateUnit(
            int facilityId,
            int departmentId)
        {
            var createUnitHandler = new CreateUnit.Handler(DatabaseContext, Mapper);

            var createCommand = new CreateUnit.Command
            {
                Request = new CreateUnitRequest
                {
                    Name = "Test",
                    FacilityId = facilityId,
                    DepartmentIds = new List<int> { departmentId },
                }
            };

            var createResult = await createUnitHandler.Handle(createCommand, CancellationToken.None);

            return createResult;
        }

        #endregion
    }
}