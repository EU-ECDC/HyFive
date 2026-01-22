using HyFive.Domain.Exceptions;
using HyFive.Services.Unit;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            await DatabaseContext.SaveChangesAsync();

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
            if (!await DatabaseContext.Facility.AnyAsync())
            {
                var departmentType = await DatabaseContext.DepartmentType
                    .FirstOrDefaultAsync(d => d.Code == "GEN")
                    ?? new Domain.Place.DepartmentType { Name = "General", Code = "GEN" };

                var facilityType = await DatabaseContext.FacilityType
                    .FirstOrDefaultAsync(f => f.Code == "HOSP")
                    ?? new Domain.Place.FacilityType { Name = "Hospital", Code = "HOSP" };

                var city = await DatabaseContext.City
                    .FirstOrDefaultAsync(c => c.Name == "Oslo")
                    ?? new Domain.Place.City { Name = "Oslo" };

                var facility = new Domain.Place.Facility
                {
                    Name = "Main Facility",
                    Abbreviation = "MF",
                    HERId = "HER001",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
                        {
                            new Domain.Place.Department { Name = "Cardiology", DepartmentType = departmentType },
                            new Domain.Place.Department { Name = "Surgery", DepartmentType = departmentType }
                        }
                };

                DatabaseContext.Facility.Add(facility);
                await DatabaseContext.SaveChangesAsync();
            }

            var facilityFromDb = await DatabaseContext.Facility.Include(i => i.Departments).FirstAsync();
            var createdUnit = await CreateUnit(facilityFromDb.Id);
            var createdUnitFromDatabase = DatabaseContext.Unit
                .Include(k => k.Facility)
                .Include(k => k.Departments)
                .FirstOrDefaultAsync(k => k.Id == createdUnit.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createdUnit.Id, Is.GreaterThan(0));
                Assert.That(createdUnit.Name, Is.EqualTo(createdUnitFromDatabase.Result.Name));
                Assert.That(createdUnit.FacilityId, Is.EqualTo(createdUnitFromDatabase.Result.Facility.Id));
                Assert.That(createdUnit.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(createdUnitFromDatabase.Result.Departments.Select(a => a.Id).OrderBy(x => x)));
                Assert.That(createdUnit.Departments.Select(a => a.Id).OrderBy(x => x).SequenceEqual(facilityFromDb.Departments.Select(a => a.Id).OrderBy(x => x)));
            });
        }

        [Test]
        public void CreateUnitTest_NonExistentFacility()
        {
            // Arrange 
            var nonExistentFacilityId = 9999;

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("FacilityNotFound"),
                async () =>
                {
                    await CreateUnit(nonExistentFacilityId);
                }
            );
        }

        [Test]
        public async Task CreateUnitTest_CannotCreateUnitWithDepartmentForAnotherFacility()
        {
            // Arrange
            var deptType = await DatabaseContext.DepartmentType.FirstOrDefaultAsync()
                ?? new Domain.Place.DepartmentType { Name = "General", Code = "GEN" };

            var facilityType = await DatabaseContext.FacilityType.FirstOrDefaultAsync()
                ?? new Domain.Place.FacilityType { Name = "Hospital", Code = "HOSP" };

            var city = await DatabaseContext.City.FirstOrDefaultAsync()
                ?? new Domain.Place.City { Name = "Oslo" };

            // Ensure first facility
            if (!await DatabaseContext.Facility.AnyAsync())
            {
                var firstFacility = new Domain.Place.Facility
                {
                    Name = "Facility A",
                    Abbreviation = "FA",
                    HERId = "HER001",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
                    {
                        new Domain.Place.Department { Name = "Dept A1", DepartmentType = deptType },
                    }
                };
                DatabaseContext.Facility.Add(firstFacility);
                await DatabaseContext.SaveChangesAsync();
            }

            // Ensure second facility
            if (await DatabaseContext.Facility.CountAsync() < 2)
            {
                var secondFacility = new Domain.Place.Facility
                {
                    Name = "Facility B",
                    Abbreviation = "FB",
                    HERId = "HER002",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
                    {
                        new Domain.Place.Department { Name = "Dept B1", DepartmentType = deptType },
                    }
                };
                DatabaseContext.Facility.Add(secondFacility);
                await DatabaseContext.SaveChangesAsync();
            }

            // Retrieve both
            var facilities = await DatabaseContext.Facility
                .Include(i => i.Departments)
                .OrderBy(f => f.Id)
                .ToListAsync();

            var facility = facilities.First();
            var otherFacility2 = facilities.Skip(1).First();

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("DepartmentNotLinkedToFacility"),
                (AsyncTestDelegate)(async () =>
                {
                    await CreateUnit(facility.Id, otherFacility2.Departments.ToList());
                })
            );
        }

        [Test]
        public async Task UpdateUnitTest()
        {
            // Arrange
            // Ensure prerequisite entities exist
            var deptType = await DatabaseContext.DepartmentType.FirstOrDefaultAsync()
                ?? new Domain.Place.DepartmentType { Name = "General", Code = "GEN" };

            var facilityType = await DatabaseContext.FacilityType.FirstOrDefaultAsync()
                ?? new Domain.Place.FacilityType { Name = "Hospital", Code = "HOSP" };

            var city = await DatabaseContext.City.FirstOrDefaultAsync()
                ?? new Domain.Place.City { Name = "Oslo" };

            if (!await DatabaseContext.Facility.Include(f => f.Departments).AnyAsync())
            {
                var facilitySeed = new Domain.Place.Facility
                {
                    Name = "Main Facility",
                    Abbreviation = "MF",
                    HERId = "HER001",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
            {
                new Domain.Place.Department { Name = "Dept A", DepartmentType = deptType },
                new Domain.Place.Department { Name = "Dept B", DepartmentType = deptType }
            }
                };
                DatabaseContext.Facility.Add(facilitySeed);
                await DatabaseContext.SaveChangesAsync();
            }

            // Now safely retrieve
            var facility = await DatabaseContext.Facility
                .Include(i => i.Departments)
                .FirstAsync();

            var departments = facility.Departments.Take(1).ToList();

            // Create the unit to update
            var createdUnit = await CreateUnit(facility.Id);

            var updatedUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateUnit.Command
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
            var deptType = await DatabaseContext.DepartmentType.FirstOrDefaultAsync()
        ?? new Domain.Place.DepartmentType { Name = "General", Code = "GEN" };

            var facilityType = await DatabaseContext.FacilityType.FirstOrDefaultAsync()
                ?? new Domain.Place.FacilityType { Name = "Hospital", Code = "HOSP" };

            var city = await DatabaseContext.City.FirstOrDefaultAsync()
                ?? new Domain.Place.City { Name = "Oslo" };

            // Ensure first facility
            if (!await DatabaseContext.Facility.Include(f => f.Departments).AnyAsync())
            {
                var firstFacility = new Domain.Place.Facility
                {
                    Name = "Facility A",
                    Abbreviation = "FA",
                    HERId = "HER001",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
            {
                new Domain.Place.Department { Name = "Dept A", DepartmentType = deptType }
            }
                };
                DatabaseContext.Facility.Add(firstFacility);
                await DatabaseContext.SaveChangesAsync();
            }

            // Ensure second facility
            if (await DatabaseContext.Facility.CountAsync() < 2)
            {
                var secondFacility = new Domain.Place.Facility
                {
                    Name = "Facility B",
                    Abbreviation = "FB",
                    HERId = "HER002",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
            {
                new Domain.Place.Department { Name = "Dept B", DepartmentType = deptType }
            }
                };
                DatabaseContext.Facility.Add(secondFacility);
                await DatabaseContext.SaveChangesAsync();
            }

            // Retrieve both
            var facilities = await DatabaseContext.Facility
                .Include(i => i.Departments)
                .OrderBy(f => f.Id)
                .ToListAsync();

            var facility = facilities.First();
            var newFacility = facilities.Skip(1).First();

            var departments = facility.Departments.Take(1).ToList();
            var createdUnit = await CreateUnit(facility.Id);

            var updatedUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);

            var updateCommand = new UpdateUnit.Command
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = createdUnit.Id,
                    Name = "Leverpostei",
                    FacilityId = newFacility.Id, // Trying to move to another facility
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(departments)
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("UnitNotLinkedToFacility"),
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
            var deptType = await DatabaseContext.DepartmentType.FirstOrDefaultAsync()
        ?? new Domain.Place.DepartmentType { Name = "General", Code = "GEN" };

            var facilityType = await DatabaseContext.FacilityType.FirstOrDefaultAsync()
                ?? new Domain.Place.FacilityType { Name = "Hospital", Code = "HOSP" };

            var city = await DatabaseContext.City.FirstOrDefaultAsync()
                ?? new Domain.Place.City { Name = "Oslo" };

            // Ensure first facility
            if (!await DatabaseContext.Facility.Include(f => f.Departments).AnyAsync())
            {
                var firstFacility = new Domain.Place.Facility
                {
                    Name = "Facility A",
                    Abbreviation = "FA",
                    HERId = "HER001",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
            {
                new Domain.Place.Department { Name = "Dept A", DepartmentType = deptType }
            }
                };
                DatabaseContext.Facility.Add(firstFacility);
                await DatabaseContext.SaveChangesAsync();
            }

            // Ensure second facility
            if (await DatabaseContext.Facility.CountAsync() < 2)
            {
                var secondFacility = new Domain.Place.Facility
                {
                    Name = "Facility B",
                    Abbreviation = "FB",
                    HERId = "HER002",
                    FacilityType = facilityType,
                    City = city,
                    Departments = new List<Domain.Place.Department>
            {
                new Domain.Place.Department { Name = "Dept B", DepartmentType = deptType }
            }
                };
                DatabaseContext.Facility.Add(secondFacility);
                await DatabaseContext.SaveChangesAsync();
            }

            // Retrieve both
            var facilities = await DatabaseContext.Facility
                .Include(i => i.Departments)
                .OrderBy(f => f.Id)
                .ToListAsync();

            var facility = facilities.First();
            var otherFacility = facilities.Skip(1).First();

            // Create a Unit in the first facility
            var createdUnit = await CreateUnit(facility.Id);

            var updateUnitHandler = new UpdateUnit.Handler(DatabaseContext, Mapper);

            // Try to update it with departments from another facility
            var updateCommand = new UpdateUnit.Command
            {
                Unit = new Models.V1.Facility.Unit
                {
                    Id = createdUnit.Id,
                    Name = "Leverpostei",
                    FacilityId = facility.Id,
                    Departments = Mapper.Map<IEnumerable<Domain.Place.Department>, List<Models.V1.Facility.Department>>(
                        otherFacility.Departments)
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("DepartmentNotLinkedToFacility"),
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