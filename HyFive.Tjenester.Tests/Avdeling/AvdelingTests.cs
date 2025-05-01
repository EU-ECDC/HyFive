using HyFive.Services.Department;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HyFive.Services.Tests.Department
{
    public class AvdelingTests : ServiceTests
    {
        [Test]
        public async Task HentAvdelingTest()
        {
            // Arrange
            var hentAvdelingHandler = new GetDepartment.Handler(DatabaseContext, Mapper);
            var query = new GetDepartment.Query() { Id = 9999 };

            var avdeling = new Domain.Place.Department { Id = 9999, InstitutionId = DatabaseContext.Institution.First().Id };
            DatabaseContext.Department.Add(avdeling);
            DatabaseContext.SaveChanges();

            // Act
            var res = await hentAvdelingHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(avdeling.Id, Is.EqualTo(res.Id));
        }
        [Test]
        public async Task HentAvdeling_IdEksistererIkke_ReturnererNull()
        {
            // Arrange
            var hentAvdelingHandler = new GetDepartment.Handler(DatabaseContext, Mapper);
            var query = new GetDepartment.Query() { Id = 123456789 };

            // Act
            var hentAvdelingResultat = await hentAvdelingHandler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.That(hentAvdelingResultat, Is.Null);
        }

        [Test]
        public async Task HentAvdelingerForInstitusjonTest()
        {
            // Arrange
            var institusjon = new Domain.Place.Institution { Id = 9999 };
            var avdeling = new Domain.Place.Department { Id = 9999, InstitutionId = institusjon.Id };
            var avdeling2 = new Domain.Place.Department { Id = 99999, InstitutionId = institusjon.Id };
            DatabaseContext.Institution.Add(institusjon);
            DatabaseContext.Department.Add(avdeling);
            DatabaseContext.Department.Add(avdeling2);
            DatabaseContext.SaveChanges();

            var hentAvdelingerForInstitusjon = new GetDepartmentsForInstitution.Handler(DatabaseContext, Mapper);
            var query = new GetDepartmentsForInstitution.Query() { InstitutionId = 9999 };

            // Act
            var res = await hentAvdelingerForInstitusjon.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(res.ToList(), Has.Count.EqualTo(2));
                Assert.That(res.All(x => x.InstitutionId == institusjon.Id));
            });
        }

        [Test]
        public async Task HentAvdelingerForInstitusjon_InstitusjonEksistererIkke_ReturnererTomListe()
        {
            // Arrange
            var hentAvdelingerForInstitusjon = new GetDepartmentsForInstitution.Handler(DatabaseContext, Mapper);
            var query = new GetDepartmentsForInstitution.Query() { InstitutionId = 123456789 };

            // Act
            var hentAvdelingerForInstitusjonResultat = await hentAvdelingerForInstitusjon.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(hentAvdelingerForInstitusjonResultat.ToList(), Has.Count.EqualTo(0));
            });
        }

        [Test]
        public async Task OpprettAvdelingTest()
        {
            // Arrange and Act
            var opprettetAvdeling = await OpprettAvdeling();
            var opprettetAvdelingFraDatabase = DatabaseContext.Department
                .Include(a => a.Institution)
                .Include(a => a.Roles)
                .FirstOrDefault(a => a.Id == opprettetAvdeling.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetAvdeling.Id, Is.GreaterThan(0));
                Assert.That(opprettetAvdeling.Name, Is.EqualTo(opprettetAvdelingFraDatabase.Name));
                Assert.That(opprettetAvdeling.InstitutionId, Is.EqualTo(opprettetAvdelingFraDatabase.Institution.Id));
                Assert.That(opprettetAvdeling.Roles.Any(r => r.Id == opprettetAvdelingFraDatabase.Roles.First().Id));
            });
        }

        [Test]
        public async Task OpprettAvdelingType_Test()
        {
            // Arrange and Act
            var opprettetAvdelingType = await OpprettAvdelingType();
            var opprettetAvdelingTypeFraDatabase = DatabaseContext.DepartmentType
                .FirstOrDefault(a => a.Id == opprettetAvdelingType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opprettetAvdelingType.Id, Is.GreaterThan(0));
                Assert.That(opprettetAvdelingType.Name, Is.EqualTo(opprettetAvdelingTypeFraDatabase.Name));
                Assert.That(opprettetAvdelingType.Code, Is.EqualTo(opprettetAvdelingTypeFraDatabase.Code));
            });
        }

        [Test]
        public async Task OppdaterAvdelingTest()
        {
            // Arrange
            var rolleIder = new List<int>() { 1 };
            var opprettetAvdeling = await OpprettAvdeling(rolleIder: rolleIder);
            var oppdaterAvdelingHandler = new UpdateDepartment.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateDepartment.Command()
            {
                Id = opprettetAvdeling.Id,
                Name = "Da Vinci",
                DepartmentTypeId = DatabaseContext.DepartmentType.FirstOrDefault(at => at.Id != opprettetAvdeling.DepartmentTypeId).Id,
                Role = new List<Models.V1.Observation.Role>()
                {
                    Mapper.Map<Domain.Observation.Role, Models.V1.Observation.Role>(DatabaseContext.Role.First(x => !rolleIder.Contains(x.Id)))
                }
            };

            // Act
            var resultatOppdater = await oppdaterAvdelingHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetAvdeling.Id));
                Assert.That(resultatOppdater.Name, Is.Not.EqualTo(opprettetAvdeling.Name));
                Assert.That(resultatOppdater.DepartmentTypeId, Is.Not.EqualTo(opprettetAvdeling.DepartmentTypeId));
                Assert.That(resultatOppdater.Roles.Count, Is.EqualTo(oppdaterCommand.Role.Count));
                Assert.That(resultatOppdater.Roles, Does.Not.Contain(opprettetAvdeling.Roles.First().Id));
            });
        }

        [Test]
        public async Task OppdaterAvdelingTest_KanOppdatereKunNavn()
        {
            // Arrange
            var opprettetAvdeling = await OpprettAvdeling();
            var oppdaterAvdelingHandler = new UpdateDepartment.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateDepartment.Command
            {
                Id = opprettetAvdeling.Id,
                Name = "Da Vinci"
            };

            // Act
            var resultatOppdater = await oppdaterAvdelingHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            var a = DatabaseContext.Department.FirstOrDefault(x => x.Id == opprettetAvdeling.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetAvdeling.Id));
                Assert.That(resultatOppdater.Name, Is.Not.EqualTo(opprettetAvdeling.Name));
                Assert.That(resultatOppdater.DepartmentTypeId, Is.EqualTo(opprettetAvdeling.DepartmentTypeId));
                Assert.That(resultatOppdater.Roles, Has.Count.EqualTo(opprettetAvdeling.Roles.Count));
                Assert.That(resultatOppdater.Roles.Select(r => r.Id), Is.EqualTo(opprettetAvdeling.Roles.Select(r => r.Id)));
            });
        }

        [Test]
        public async Task OppdaterAvdelingTypeTest()
        {
            // Arrange
            var opprettetAvdelingType = await OpprettAvdelingType();
            var oppdaterAvdelingTypeHandler = new UpdateDepartmentType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateDepartmentType.Command()
            {
                DepartmentType = new Models.V1.Institution.DepartmentType()
                {
                    Id = opprettetAvdelingType.Id,
                    Name = "Da Vinci",
                }
            };

            // Act
            var resultatOppdater = await oppdaterAvdelingTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetAvdelingType.Id));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.DepartmentType.Name));
            });
        }

        [Test]
        public async Task OppdaterAvdelingType_SkalIkkeKunneOppdatereKode_OppdatererKunNavn()
        {
            // Arrange
            var opprettetAvdelingType = await OpprettAvdelingType(kode: "HELLO");
            var oppdaterAvdelingTypeHandler = new UpdateDepartmentType.Handler(DatabaseContext, Mapper);
            var oppdaterCommand = new UpdateDepartmentType.Command()
            {
                DepartmentType = new Models.V1.Institution.DepartmentType()
                {
                    Id = opprettetAvdelingType.Id,
                    Code = "PROVER",
                    Name = "ProverAEndreKode"
                }
            };

            // Act
            var resultatOppdater = await oppdaterAvdelingTypeHandler.Handle(oppdaterCommand, new System.Threading.CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultatOppdater.Id, Is.EqualTo(opprettetAvdelingType.Id));
                Assert.That(resultatOppdater.Code, Is.EqualTo(opprettetAvdelingType.Code));
                Assert.That(resultatOppdater.Name, Is.EqualTo(oppdaterCommand.DepartmentType.Name));
            });
        }

        #region Helper-methods

        private async Task<Models.V1.Institution.Department> OpprettAvdeling(int institusjonsId = 0, List<int> rolleIder = null, int avdelingTypeId = 0)
        {
            var opprettAvdelingHandler = new CreateDepartment.Handler(DatabaseContext, Mapper);
            var opprettCommand = new CreateDepartment.Command()
            {
                Request = new Models.V1.Institution.CreateDepartmentRequest()
                {
                    Name = "Test",
                    InstitutionId = institusjonsId == 0 ? DatabaseContext.Institution.First().Id : institusjonsId,
                    DepartmentTypeId = avdelingTypeId == 0 ? DatabaseContext.DepartmentType.First().Id : avdelingTypeId,
                    RoleIds = rolleIder ?? new List<int>() { DatabaseContext.Role.First().Id }
                }
            };

            var resOpprett = await opprettAvdelingHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        private async Task<Models.V1.Institution.DepartmentType> OpprettAvdelingType(string kode = null)
        {
            var opprettAvdelingTypeHandler = new CreateDepartmentType.Handler(DatabaseContext, Mapper);
            var opprettCommand = new CreateDepartmentType.Command()
            {
                DepartmentType = new Models.V1.Institution.DepartmentType()
                {
                    Code = kode ?? "TEST",
                    Name = "Test"
                }
            };

            var resOpprett = await opprettAvdelingTypeHandler.Handle(opprettCommand, new System.Threading.CancellationToken());

            return resOpprett;
        }

        #endregion
    }
}