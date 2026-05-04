using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Session;
using HyFive.Services.ProtectiveEquipment;
using HyFive.Services.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Tests.ProtectiveEquipment
{
    public class ProtectiveEquipmentTests : ServiceTests
    {
        private Guid sessionId = Guid.NewGuid();
        private Guid observationId = Guid.NewGuid();
        private readonly string hprnumber = "9383840";

        #region ProtectiveEquipmentSession


        #endregion

        #region ProtectiveEquipmentType

        [Test]
        public async Task GetProtectiveEquipmentTypes_Test()
        {
            // Arrange
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                DatabaseContext.ProtectiveEquipmentType.AddRange(
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                    {
                        Name = "Gloves",
                        Code = "PE-G"
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                    {
                        Name = "Mask",
                        Code = "PE-M"
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                    {
                        Name = "Gown",
                        Code = "PE-GW"
                    }
                );

                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.ProtectiveEquipmentType
         .Select(x => x.Id)
         .ToListAsync();

            var handler = new GetProtectiveEquipmentTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentTypes.Query();

            // Act
            var res = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateProtectiveEquipmentType_Test()
        {
            // Arrange
            var createdProtectiveEquipmentType = await CreatedProtectiveEquipmentType();
            var updateProtectiveEquipmentTypeHandler = new UpdateProtectiveEquipmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentType.Command()
            {
                EquipmentType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType()
                {
                    Id = createdProtectiveEquipmentType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateProtectiveEquipmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdProtectiveEquipmentType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.EquipmentType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.EquipmentType.Code));
                Assert.That(updateResults.Code, Is.EqualTo(createdProtectiveEquipmentType.Code));
            });
        }

        [Test]
        public void UpdateProtectiveEquipmentType_NonExistentId()
        {
            // Arrange
            var updateProtectiveEquipmentTypeHandler = new UpdateProtectiveEquipmentType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentType.Command()
            {
                EquipmentType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("ProtectiveEquipmentTypeNotFound"),
                async () =>
                {
                    await updateProtectiveEquipmentTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region ProtectiveEquipmentSettingType

        [Test]
        public async Task GetProtectiveEquipmentSettingTypes_Test()
        {
            if (!await DatabaseContext.ProtectiveEquipmentSettingType.AnyAsync())
            {
                DatabaseContext.ProtectiveEquipmentSettingType.AddRange(
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Name = "In patient room",
                        Code = "SET-1"
                    },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType
                    {
                        Name = "Operating theatre",
                        Code = "SET-2"
                    }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var existingTypes = await DatabaseContext.ProtectiveEquipmentSettingType
                .Select(x => x.Id)
                .ToListAsync();

            var handler = new GetProtectiveEquipmentSettingTypes.Handler(DatabaseContext, Mapper);
            var query = new GetProtectiveEquipmentSettingTypes.Query();

            // Act
            var res = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingTypes.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingTypes.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task UpdateProtectiveEquipmentSettingType_Test()
        {
            // Arrange
            var createdProtectiveEquipmentSettingType = await CreateProtectiveEquipmentSettingType();
            var updateProtectiveEquipmentSettingTypeHandler = new UpdateProtectiveEquipmentSettingType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentSettingType.Command()
            {
                SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                {
                    Id = createdProtectiveEquipmentSettingType.Id,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act
            var updateResults = await updateProtectiveEquipmentSettingTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(createdProtectiveEquipmentSettingType.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.SettingType.Name));
                Assert.That(updateResults.Code, Is.Not.EqualTo(updateCommand.SettingType.Code));
                Assert.That(updateResults.Code, Is.EqualTo(createdProtectiveEquipmentSettingType.Code));
            });
        }

        [Test]
        public void UpdateProtectiveEquipmentSettingType_NonExistentId()
        {
            // Arrange
            var updateProtectiveEquipmentSettingTypeHandler = new UpdateProtectiveEquipmentSettingType.Handler(DatabaseContext, Mapper);
            var updateCommand = new UpdateProtectiveEquipmentSettingType.Command()
            {
                SettingType = new Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType()
                {
                    Id = 99999999,
                    Code = "DV",
                    Name = "Da Vinci",
                }
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("ProtectiveEquipmentSettingTypeNotFound"),
                async () =>
                {
                    await updateProtectiveEquipmentSettingTypeHandler.Handle(updateCommand, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region MisusedType

        [Test]
        public async Task GetMisuseTypes_Test()
        {
            // Arrange
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                var gloves = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };

                gloves.MisuseTypes = new List<Domain.Observation.ProtectiveEquipment.MisuseType>
                    {
                        new Domain.Observation.ProtectiveEquipment.MisuseType { Name = "Worn incorrectly" },
                        new Domain.Observation.ProtectiveEquipment.MisuseType { Name = "Not replaced when damaged" }
                    };

                DatabaseContext.ProtectiveEquipmentType.Add(gloves);
                await DatabaseContext.SaveChangesAsync();
            }

            var equipment = await DatabaseContext.ProtectiveEquipmentType
                .Include(x => x.MisuseTypes)
                .FirstAsync();

            var existingMisuseTypesForEquipment = equipment.MisuseTypes
                .Select(x => x.Id)
                .ToList();

            var handler = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query { EquipmentTypeId = equipment.Id };

            // Act
            var res = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Not.Zero);
                Assert.That(res, Has.Count.EqualTo(existingMisuseTypesForEquipment.Count));
                Assert.That(res.OrderBy(it => it.Id).Select(it => it.Id), Is.EqualTo(existingMisuseTypesForEquipment.OrderBy(x => x)));
            });
        }

        [Test]
        public async Task GetMisuseTypes_NonExistentEquipment()
        {
            // Arrange
            var getMisuseTypes = new GetMisuseTypes.Handler(DatabaseContext, Mapper);
            var query = new GetMisuseTypes.Query() { EquipmentTypeId = 123456789 };

            // Act
            var res = await getMisuseTypes.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res, Has.Count.Zero);
            });
        }

        [Test]
        public async Task CreateMisuseType_Test()
        {
            // Arrange and Act
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                DatabaseContext.ProtectiveEquipmentType.AddRange(
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType { Name = "Gloves", Code = "PE-1" },
                    new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType { Name = "Mask", Code = "PE-2" }
                );
                await DatabaseContext.SaveChangesAsync();
            }

            var name = "test";
            var equipmentType = await DatabaseContext.ProtectiveEquipmentType.FirstAsync();
            var createdMisuseType = await CreatedMisuseType(name: name, equipmentTypeId: equipmentType.Id);
            var createdMisuseTypeFromDatabase = await DatabaseContext.MisuseType
                .FirstOrDefaultAsync(a => a.Id == createdMisuseType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(createdMisuseType.Id, Is.GreaterThan(0));
                Assert.That(createdMisuseType.Name, Is.EqualTo(name));
                Assert.That(createdMisuseType.Name, Is.EqualTo(createdMisuseTypeFromDatabase.Name));
                Assert.That(createdMisuseTypeFromDatabase.ProtectiveEquipmentType.Id, Is.EqualTo(equipmentType.Id));
            });
        }

        [Test]
        public void CreateMisuseType_NonExistentEquipmentType_ShouldFail()
        {
            // Act
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("EquipmentTypeNotFound"),
                async () =>
                {
                    await CreatedMisuseType(equipmentTypeId: 123456789);
                }
            );
        }

        [Test]
        public async Task UpdateMisuseType_Test()
        {
            // Arrange
            // Ensure at least one ProtectiveEquipmentType exists
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                var gloves = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };
                DatabaseContext.ProtectiveEquipmentType.Add(gloves);
                await DatabaseContext.SaveChangesAsync();
            }

            var equipmentType = await DatabaseContext.ProtectiveEquipmentType.FirstAsync();

            // Create a misuse type linked via navigation property
            var misuse = new Domain.Observation.ProtectiveEquipment.MisuseType
            {
                Name = "Incorrect use",
                ProtectiveEquipmentType = equipmentType
            };
            DatabaseContext.MisuseType.Add(misuse);
            await DatabaseContext.SaveChangesAsync();

            var updateHandler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);

            var updateCommand = new UpdateMisuseType.Command
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType
                {
                    Id = misuse.Id,
                    Name = "Da Vinci"
                },
                EquipmentTypeId = equipmentType.Id
            };

            // Act
            var updateResults = await updateHandler.Handle(updateCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResults.Id, Is.EqualTo(misuse.Id));
                Assert.That(updateResults.Name, Is.EqualTo(updateCommand.MisuseType.Name));
            });
        }

        [Test]
        public async  Task UpdateMisuseType_NonExistentEquipmentTypeId()
        {
            // Arrange
            // Ensure there’s at least one MisuseType in the DB
            if (!await DatabaseContext.MisuseType.AnyAsync())
            {
                var fakeEquipmentType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };

                var misuse = new Domain.Observation.ProtectiveEquipment.MisuseType
                {
                    Name = "Incorrect use",
                    ProtectiveEquipmentType = fakeEquipmentType
                };

                DatabaseContext.MisuseType.Add(misuse);
                await DatabaseContext.SaveChangesAsync();
            }

            var misuseType = await DatabaseContext.MisuseType.FirstAsync();

            var handler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);

            var command = new UpdateMisuseType.Command
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType
                {
                    Id = misuseType.Id,
                    Name = "Da Vinci"
                },
                // Invalid equipment type ID
                EquipmentTypeId = 123456789
            };


            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("EquipmentTypeNotFound"),
                async () =>
                {
                    await handler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        [Test]
        public async Task UpdateMisuseType_NonExistentMisuseTypeId()
        {
            // Arrange
            if (!await DatabaseContext.ProtectiveEquipmentType.AnyAsync())
            {
                var gloves = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType
                {
                    Name = "Gloves",
                    Code = "PE-G"
                };
                DatabaseContext.ProtectiveEquipmentType.Add(gloves);
                await DatabaseContext.SaveChangesAsync();
            }

            var equipmentType = await DatabaseContext.ProtectiveEquipmentType.FirstAsync();

            var handler = new UpdateMisuseType.Handler(DatabaseContext, Mapper);

            var command = new UpdateMisuseType.Command
            {
                MisuseType = new Models.V1.Observation.ProtectiveEquipment.MisuseType
                {
                    Id = 123456789,   // non-existent MisuseType
                    Name = "Da Vinci"
                },
                EquipmentTypeId = equipmentType.Id
            };

            // Act and Assert
            Assert.ThrowsAsync(
                Is.TypeOf<DomainException>().And.Message.Contains("MisuseTypeNotFound"),
                async () =>
                {
                    await handler.Handle(command, new System.Threading.CancellationToken());
                }
            );
        }

        #endregion

        #region Helper-methods

        private async Task<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType> CreatedProtectiveEquipmentType(string code = null)
        {
            var protectiveEquipmentType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentType.Add(protectiveEquipmentType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(protectiveEquipmentType);
        }

        private async Task<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType> CreateProtectiveEquipmentSettingType(string code = null)
        {
            var protectiveEquipmentSettingType = new Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType() { Code = code ?? "TEST", Name = "test" };
            DatabaseContext.ProtectiveEquipmentSettingType.Add(protectiveEquipmentSettingType);
            await DatabaseContext.SaveChangesAsync();

            return Mapper.Map<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>(protectiveEquipmentSettingType);
        }

        private async Task<Models.V1.Observation.ProtectiveEquipment.MisuseType> CreatedMisuseType(string name = null, int equipmentTypeId = 0)
        {
            var createMisuseTypeHandler = new CreateMisuseType.Handler(DatabaseContext, Mapper);
            var createCommand = new CreateMisuseType.Command()
            {
                MisuseType = new CreateIncorrectUseTypeRequest() { Name = name ?? "Test" },
                EquipmentTypeId = equipmentTypeId != 0 ? equipmentTypeId : DatabaseContext.ProtectiveEquipmentType.First().Id
            };

            var createResults = await createMisuseTypeHandler.Handle(createCommand, new System.Threading.CancellationToken());

            return createResults;
        }

        #endregion
    }
}
