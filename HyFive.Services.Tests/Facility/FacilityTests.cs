using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Models.V1.Session;
using HyFive.Services.Common;
using HyFive.Services.Department;
using HyFive.Services.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
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
            // Arrange: make sure Facility level exists
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(l => l.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new HyFive.Domain.Place.OrganisationUnitLevel
                {
                    Level = OrganisationUnitLevels.Facility
                };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            // Arrange: make sure some OU type exists
            var ouType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (ouType == null)
            {
                ouType = new HyFive.Domain.Place.OrganisationUnitType
                {
                    Name = "Test Type",
                    Code = "TEST"
                };
                DatabaseContext.OrganisationUnitType.Add(ouType);
                await DatabaseContext.SaveChangesAsync();
            }

            // Arrange: create a facility Unit
            var facilityOu = new HyFive.Domain.Place.OrganisationUnit
            {
                ParentId = null,
                Name = "Test Facility",
                Abbreviation = "TF",
                Description = null,
                AddressId = null,
                TypeId = ouType.Id,
                LevelId = facilityLevel.Id
            };

            DatabaseContext.OrganisationUnit.Add(facilityOu);
            await DatabaseContext.SaveChangesAsync();

            // Arrange: mediator mock that routes GetOrganisationUnit.Query to the real handler
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(m => m.Send(It.IsAny<GetOrganisationUnit.Query>(), It.IsAny<CancellationToken>()))
                .Returns<GetOrganisationUnit.Query, CancellationToken>(async (q, ct) =>
                {
                    var handler = new GetOrganisationUnit.Handler(DatabaseContext, Mapper);
                    return await handler.Handle(q, ct);
                });

            var getFacilityHandler = new GetFacility.Handler(DatabaseContext, mediator.Object, Mapper);
            var query = new GetFacility.Query { FacilityId = facilityOu.Id };

            // Act
            var res = await getFacilityHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.Id, Is.EqualTo(facilityOu.Id));
                Assert.That(res.Abbreviation, Is.EqualTo(facilityOu.Abbreviation));
                Assert.That(res.Name, Is.EqualTo(facilityOu.Name));

                // optional extra assertions that make sense now:
                Assert.That(res.LevelId, Is.EqualTo(facilityOu.LevelId));
                Assert.That(res.TypeId, Is.EqualTo(facilityOu.TypeId));
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
                Assert.That(res.Count, Is.EqualTo(DatabaseContext.OrganisationUnit.ToList().Count));
            });
        }

        [Test]
        public async Task GetFacilitiesForCoordinatorTest()
        {
            // Arrange
            var email = "test@gmail.com";

            // Ensure required lookup rows exist (Level + Type)
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(l => l.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel { Level = OrganisationUnitLevels.Facility };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            var facilityType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (facilityType == null)
            {
                facilityType = new Domain.Place.OrganisationUnitType { Name = "Hospital", Code = "HOSP" };
                DatabaseContext.OrganisationUnitType.Add(facilityType);
                await DatabaseContext.SaveChangesAsync();
            }

            var facilityOu = new Domain.Place.OrganisationUnit
            {
                Name = "Coordinator Facility",
                Abbreviation = "CF",
                LevelId = facilityLevel.Id,
                TypeId = facilityType.Id,
                ParentId = null
            };
            DatabaseContext.OrganisationUnit.Add(facilityOu);

            var user = new Domain.User.User
            {
                Email = email,
                FirstName = "Test",
                LastName = "Coordinator",
                IsDeactivated = false,
                CreatedTime = DateTime.UtcNow,
                IdentityPseudonym = null
            };
            DatabaseContext.User.Add(user);

            await DatabaseContext.SaveChangesAsync();

            // Grant coordinator permission on the facility OU
            DatabaseContext.UserPermission.Add(new Domain.User.UserPermission
            {
                UserId = user.Id,
                OrganisationUnitId = facilityOu.Id,
                PermissionLevel = PermissionLevelConstants.Coordinator
            });

            await DatabaseContext.SaveChangesAsync();

            var handler = new GetFacilitiesForCoordinator.Handler(DatabaseContext, Mapper);

            // Act
            var facilities = await handler.Handle(
                new GetFacilitiesForCoordinator.Query { CoordinatorEmail = email },
                CancellationToken.None);

            // Assert
            Assert.That(facilities, Is.Not.Null);
            Assert.That(facilities.Length, Is.GreaterThan(0));
            Assert.That(facilities.Any(f => f.Id == facilityOu.Id), Is.True);

        }

        [Test]
        public async Task GetFacilityTypesTest()
        {
            // Arrange
            var handler = new GetFacilityTypes.Handler(DatabaseContext, Mapper);

            // Expected = exactly what the handler is supposed to return (root OU types)
            var expectedTypeCodes = await DatabaseContext.OrganisationUnit
                .AsNoTracking()
                .Where(ou => ou.ParentId == null)
                .Select(ou => ou.Type.Code)
                .Where(code => code != null)
                .Distinct()
                .OrderBy(code => code)
                .ToListAsync();

            // Act
            var types = await handler.Handle(new GetFacilityTypes.Query(), CancellationToken.None);

            var actualTypeCodes = types
                .Select(t => t.Code)
                .Where(code => code != null)
                .Distinct()
                .OrderBy(code => code)
                .ToList();

            // Assert
            Assert.That(actualTypeCodes, Is.EqualTo(expectedTypeCodes));
        }

        [Test]
        public async Task GetCoordinatorsForFacilityTest()
        {
            // Arrange
            // Create facility OU (root)
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Facility)
                ?? (await CreateOrganisationUnitLevel(OrganisationUnitLevels.Facility));

            var deptLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(x => x.Level == OrganisationUnitLevels.Department)
                ?? (await CreateOrganisationUnitLevel(OrganisationUnitLevels.Department));

            var facilityType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (facilityType == null)
            {
                facilityType = new Domain.Place.OrganisationUnitType
                {
                    Name = "Hospital",
                    Code = "HOSP"
                };

                DatabaseContext.OrganisationUnitType.Add(facilityType);
                await DatabaseContext.SaveChangesAsync();
            }

            var deptType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (deptType == null)
            {
                deptType = new Domain.Place.OrganisationUnitType
                {
                    Name = "Department",
                    Code = "DEPT"
                };

                DatabaseContext.OrganisationUnitType.Add(deptType);
                await DatabaseContext.SaveChangesAsync();
            }

            var facility = new Domain.Place.OrganisationUnit
            {
                Name = "Facility X",
                ParentId = null,
                LevelId = facilityLevel.Id,
                TypeId = facilityType.Id
            };
            DatabaseContext.OrganisationUnit.Add(facility);
            await DatabaseContext.SaveChangesAsync();

            // Department under facility
            var department = new Domain.Place.OrganisationUnit
            {
                Name = "Dept A",
                ParentId = facility.Id,
                LevelId = deptLevel.Id,
                TypeId = deptType.Id
            };
            DatabaseContext.OrganisationUnit.Add(department);

            // User
            var user = new Domain.User.User
            {
                Email = "coordinator@test.com",
                FirstName = "Test",
                LastName = "Coordinator",
                IsDeactivated = false,
                CreatedTime = DateTime.UtcNow
            };
            DatabaseContext.User.Add(user);

            await DatabaseContext.SaveChangesAsync();

            // Permission on the DEPARTMENT (descendant of facility)
            DatabaseContext.UserPermission.Add(new Domain.User.UserPermission
            {
                UserId = user.Id,
                OrganisationUnitId = department.Id,
                PermissionLevel = PermissionLevelConstants.Coordinator
            });

            await DatabaseContext.SaveChangesAsync();

            var handler = new GetCoordinatorsForFacility.Handler(DatabaseContext, Mapper);

            // Act
            var coordinators = await handler.Handle(
                new GetCoordinatorsForFacility.Query { FacilityId = facility.Id },
                CancellationToken.None);

            // Assert
            Assert.That(coordinators, Has.Length.EqualTo(1));
            Assert.That(coordinators[0].Email, Is.EqualTo("coordinator@test.com"));
        }


        [Test]
        public async Task GetObserversForFacilityTest()
        {
            // Arrange
            var facilityLevel = await EnsureOrganisationUnitLevel(OrganisationUnitLevels.Facility);
            var facilityType = await EnsureOrganisationUnitType("HOSP", "Hospital");

            var facility = new Domain.Place.OrganisationUnit
            {
                Name = "Facility A",
                LevelId = facilityLevel.Id,
                TypeId = facilityType.Id,
                ParentId = null
            };
            DatabaseContext.OrganisationUnit.Add(facility);

            var observerUser = new HyFive.Domain.User.User
            {
                Email = "observer@test.com",
                FirstName = "Obs",
                LastName = "User",
                IsDeactivated = false,
                CreatedTime = DateTime.UtcNow,
                IdentityPseudonym = null
            };
            DatabaseContext.User.Add(observerUser);

            await DatabaseContext.SaveChangesAsync();

            DatabaseContext.UserPermission.Add(new HyFive.Domain.User.UserPermission
            {
                UserId = observerUser.Id,
                OrganisationUnitId = facility.Id,         
                PermissionLevel = "Observer"
            });

            await DatabaseContext.SaveChangesAsync();

            var handler = new GetObserversForFacility.Handler(DatabaseContext, Mapper);

            // Act
            var observers = await handler.Handle(
                new GetObserversForFacility.Query { FacilityId = facility.Id },
                CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(observers.Length, Is.EqualTo(1));
                Assert.That(observers.Select(o => o.Email), Does.Contain("observer@test.com"));
            });
        }

        [Test]
        public async Task CreateAndGetPredefinedCommentsTest()
        {
            // Arrange
            var facilityOu = await EnsureFacilityOrganisationUnitAsync();

            var createHandler = new CreatePredefinedComment.Handler(DatabaseContext);

            var createCmd = new CreatePredefinedComment.Command
            {
                FacilityId = facilityOu.Id, // <-- THIS is now OrganisationUnitId
                SessionType = SessionType.ProtectiveEquipment,
                NewPredefinedComment = new CreatePredefinedCommentRequest
                {
                    Comment = "Hello predefined"
                }
            };

            // Act (create)
            var created = await createHandler.Handle(createCmd, CancellationToken.None);

            // Act (get)
            var getHandler = new GetPredefinedComments.Handler(DatabaseContext);

            var comments = await getHandler.Handle(new GetPredefinedComments.Query
            {
                FacilityId = facilityOu.Id,
                SessionType = SessionType.ProtectiveEquipment
            }, CancellationToken.None);

            // Assert
            Assert.That(created, Is.True);
            Assert.That(comments, Is.Not.Null);
            Assert.That(comments, Contains.Item("Hello predefined"));

        }

        [Test]
        public async Task UpdateFacilityTest()
        {
            // Arrange
            (var facility, _) = await CreateFacility();

            var newName = $"A new name and a random value:{Guid.NewGuid()}";

            // You MUST provide a valid OrganisationUnitTypeId (handler validates it exists)
            // If your CreateFacility() already sets facility.TypeId, reuse it.
            var typeId = facility.Type?.Id > 0 ? facility.Type.Id : facility.TypeId;

            if (typeId == 0)
            {
                // fallback: ensure at least one type exists
                var type = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
                if (type == null)
                {
                    type = new Domain.Place.OrganisationUnitType { Name = "Hospital", Code = "HOSP" };
                    DatabaseContext.OrganisationUnitType.Add(type);
                    await DatabaseContext.SaveChangesAsync();
                }
                typeId = type.Id;
            }

            var cmd = new UpdateFacility.Command
            {
                Request = new UpdateOrganizationUnitRequest
                {
                    Id = facility.Id,
                    Name = newName,
                    Abbreviation = facility.Abbreviation,   // keep same (or set new)
                    Description = facility.Description,     // keep same (or set new)
                    OrganisationUnitTypeId = typeId,
                    Address = facility.Address == null
                        ? null
                        : new HyFive.Models.V1.OrganisationUnit.Address
                        {
                            City = facility.Address.City,
                            Street = facility.Address.Street,
                            PostalCode = facility.Address.PostalCode
                        }
                }
            };

            DatabaseContext.ChangeTracker.Clear();
            var updateFacilityHandler = new UpdateFacility.Handler(DatabaseContext, Mapper);
            // Act
            await updateFacilityHandler.Handle(cmd, CancellationToken.None);

            // Assert (DB check)
            var updated = await DatabaseContext.OrganisationUnit
                .AsNoTracking()
                .FirstAsync(i => i.Id == facility.Id);

            Assert.That(updated.Name, Is.EqualTo(newName));

        }

        [Test]
        public async Task UpdateFacilityTypeTest()
        {
            // Arrange: ensure at least one OrganisationUnitType exists
            var existing = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (existing == null)
            {
                existing = new Domain.Place.OrganisationUnitType
                {
                    Code = "HOSP",
                    Name = "Hospital",
                    Description = null
                };
                DatabaseContext.OrganisationUnitType.Add(existing);
                await DatabaseContext.SaveChangesAsync();
            }

            var handler = new UpdateFacilityType.Handler(DatabaseContext, Mapper);
            var newName = $"NAME{Guid.NewGuid()}";

            // Act
            await handler.Handle(new UpdateFacilityType.Command
            {
                FacilityType = new HyFive.Models.V1.OrganisationUnit.OrganisationUnitType
                {
                    Id = existing.Id,
                    Code = "CODE", // handler ignores this (won'type update DB)
                    Name = newName,
                    Description = existing.Description
                }
            }, CancellationToken.None);

            // Assert
            var after = await DatabaseContext.OrganisationUnitType
                .AsNoTracking()
                .FirstAsync(x => x.Id == existing.Id);

            Assert.That(after.Name, Is.EqualTo(newName));
            Assert.That(after.Code, Is.EqualTo(existing.Code)); // unchanged (by design)
        }

        [Test]
        public async Task CreateFacilityTest()
        {
            // Arrange
            await EnsureOrganisationUnitLevel(OrganisationUnitLevels.Facility);
            var facilityType = await EnsureOrganisationUnitType(code: "HOSP", name: "Hospital");
            var city = await EnsureCity("Oslo");

            var handler = new CreateFacility.Handler(DatabaseContext, Mapper);

            // Act
            var created = await handler.Handle(new CreateFacility.Command
            {
                Request = new CreateOrganisationUnitRequest
                {
                    Name = "FacilityTest",
                    Abbreviation = "test1",
                    Description = null,
                    OrganisationUnitTypeId = facilityType.Id,
                    CityId = city.Id,

                    FirstName = "User",
                    LastName = "Test",
                    Email = "test@gmail.com",
                    Pseudonym = null
                }
            }, CancellationToken.None);

            // Assert: Unit stored
            var ou = await DatabaseContext.OrganisationUnit
                .Include(x => x.LevelRef)
                .Include(x => x.Type)
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == created.Id);

            Assert.That(ou, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(ou.ParentId, Is.Null);
                Assert.That(ou.LevelRef.Level, Is.EqualTo(OrganisationUnitLevels.Facility));
                Assert.That(ou.Name, Is.EqualTo("FacilityTest"));
                Assert.That(ou.Abbreviation, Is.EqualTo("test1"));
                Assert.That(ou.TypeId, Is.EqualTo(facilityType.Id));
                Assert.That(ou.Address, Is.Not.Null);
                Assert.That(ou.Address.City.Name, Is.EqualTo("Oslo"));
            });

            // Assert: coordinator permission exists
            var permission = await DatabaseContext.UserPermission
                .Include(p => p.User)
                .FirstOrDefaultAsync(p =>
                    p.OrganisationUnitId == ou.Id &&
                    p.PermissionLevel == PermissionLevelConstants.Coordinator &&
                    p.User.Email == "test@gmail.com");

            Assert.That(permission, Is.Not.Null);

        }

        [Test]
        public async Task CreateFacilityTypeTest()
        {
            // Arrange and Act
            var createType = await CreateOrganisationUnitType();
            var createTypeFromDatabase = await DatabaseContext.OrganisationUnitType.FirstAsync(i => i.Id == createType.Id);

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
            _ = await CreateOrganisationUnitType(code: "CODE");

            // Act and Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
            {
                await CreateOrganisationUnitType(code: "CODE");
            });
        }

        #region Helper-methods

        private async Task<HyFive.Models.V1.OrganisationUnit.OrganisationUnitType> CreateOrganisationUnitType(string code = null)
        {
            // In the new schema: FacilityType == OrganisationUnitType (for Level = Facility)
            var handler = new CreateOrganisationUnitType.Handler(DatabaseContext, Mapper);

            var cmd = new CreateOrganisationUnitType.Command
            {
                Type = new HyFive.Models.V1.OrganisationUnit.OrganisationUnitType
                {
                    Code = code ?? "TEST",
                    Name = "Test"
                }
            };

            return await handler.Handle(cmd, CancellationToken.None);
        }

        private async Task<Domain.Place.OrganisationUnit> EnsureFacilityOrganisationUnitAsync()
        {
            // 1) Ensure OrganisationUnitLevel = Facility exists
            var facilityLevel = await DatabaseContext.OrganisationUnitLevel
                .FirstOrDefaultAsync(l => l.Level == OrganisationUnitLevels.Facility);

            if (facilityLevel == null)
            {
                facilityLevel = new Domain.Place.OrganisationUnitLevel { Level = OrganisationUnitLevels.Facility };
                DatabaseContext.OrganisationUnitLevel.Add(facilityLevel);
                await DatabaseContext.SaveChangesAsync();
            }

            // 2) Ensure some OrganisationUnitType exists (or create a dedicated one for test)
            var facilityType = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync();
            if (facilityType == null)
            {
                facilityType = new Domain.Place.OrganisationUnitType { Name = "HospitalTest", Code = "TESTHOSP" };
                DatabaseContext.OrganisationUnitType.Add(facilityType);
                await DatabaseContext.SaveChangesAsync();
            }

            // 3) Create facility OU
            var facilityOu = new Domain.Place.OrganisationUnit
            {
                ParentId = null,
                Name = "Test Facility",
                Abbreviation = "TF",
                Description = null,
                AddressId = null,
                TypeId = facilityType.Id,
                LevelId = facilityLevel.Id
            };

            DatabaseContext.OrganisationUnit.Add(facilityOu);
            await DatabaseContext.SaveChangesAsync();

            return facilityOu;
        }

        private async Task<Domain.Place.OrganisationUnitLevel> CreateOrganisationUnitLevel(string level)
        {
            var entity = new Domain.Place.OrganisationUnitLevel { Level = level };
            DatabaseContext.OrganisationUnitLevel.Add(entity);
            await DatabaseContext.SaveChangesAsync();
            return entity;
        }

        private async Task<Domain.Place.OrganisationUnitLevel> EnsureOrganisationUnitLevel(string levelName)
        {
            var lvl = await DatabaseContext.OrganisationUnitLevel.FirstOrDefaultAsync(x => x.Level == levelName);
            if (lvl != null) return lvl;

            lvl = new Domain.Place.OrganisationUnitLevel { Level = levelName };
            DatabaseContext.OrganisationUnitLevel.Add(lvl);
            await DatabaseContext.SaveChangesAsync();
            return lvl;
        }

        private async Task<Domain.Place.OrganisationUnitType> EnsureOrganisationUnitType(string code, string name)
        {
            var type = await DatabaseContext.OrganisationUnitType.FirstOrDefaultAsync(x => x.Code == code);
            if (type != null) return type;

            type = new Domain.Place.OrganisationUnitType { Code = code, Name = name };
            DatabaseContext.OrganisationUnitType.Add(type);
            await DatabaseContext.SaveChangesAsync();
            return type;
        }

        private async Task<Domain.Place.City> EnsureCity(string name)
        {
            var city = await DatabaseContext.City.FirstOrDefaultAsync(c => c.Name == name);

            if (city != null)
                return city;

            city = new Domain.Place.City { Name = name };
            DatabaseContext.City.Add(city);
            await DatabaseContext.SaveChangesAsync();

            return city;
        }

        #endregion

    }
}