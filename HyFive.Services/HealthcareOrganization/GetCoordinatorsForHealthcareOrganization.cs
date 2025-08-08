using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class GetCoordinatorsForHealthcareOrganization
    {
        public class Query : IRequest<HealthcareOrganizationCoordinator[]>
        {
            public int HealthcareOrganizationId { get; set; }
        }

        public class Handler : IRequestHandler<Query, HealthcareOrganizationCoordinator[]>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }


            public async Task<HealthcareOrganizationCoordinator[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var coordinatorsForInstitutionsInHealthcareOrganization = GetCoordinatorsForInstitutionsInHealthcareOrganization(request.HealthcareOrganizationId);

                List<HealthcareOrganizationCoordinator> coordinatorForHealthcareOrganizationList = CreateCoordinatorsForHealthcareOrganizationList(coordinatorsForInstitutionsInHealthcareOrganization);

                return coordinatorForHealthcareOrganizationList.ToArray();
            }

            private static List<HealthcareOrganizationCoordinator> CreateCoordinatorsForHealthcareOrganizationList(List<Coordinator> coordinatorsForInstitutionsInHealthcareOrganization)
            {
                var coordinatorsListForHealthcareOrganization = new List<HealthcareOrganizationCoordinator>();

                foreach (var coordinator in coordinatorsForInstitutionsInHealthcareOrganization)
                {
                    var coordinatorForHealthcareOrganization = coordinatorsListForHealthcareOrganization.FirstOrDefault(k => k.Email == coordinator.Email);
                    if (coordinatorForHealthcareOrganization == null)
                    {
                        coordinatorForHealthcareOrganization = CreateCoordinatorForHealthcareOrganization(coordinatorsListForHealthcareOrganization, coordinator);
                    }

                    AddInstitution(coordinator, coordinatorForHealthcareOrganization);
                }

                return coordinatorsListForHealthcareOrganization;
            }

            private static void AddInstitution(Coordinator coordinator, HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization)
            {
                var institutionReport = new InstitutionReport
                {
                    Abbreviation = coordinator.Institution.Abbreviation,
                    HERId = coordinator.Institution.HERId,
                    Id = coordinator.Institution.Id,
                    Name = coordinator.Institution.Name,
                    InstitutionType = new InstitutionType
                    {
                        Id = coordinator.Institution.InstitutionType.Id,
                        Code = coordinator.Institution.InstitutionType.Code,
                        Name = coordinator.Institution.InstitutionType.Name
                    }
                };

                coordinatorForHealthcareOrganization.Institutions.Add(institutionReport);
            }

            private static HealthcareOrganizationCoordinator CreateCoordinatorForHealthcareOrganization(List<HealthcareOrganizationCoordinator> coordinatorForHealthcareOrganizationList, Coordinator coordinator)
            {
                var coordinatorForHealthcareOrganization = new HealthcareOrganizationCoordinator
                {
                    Id = coordinator.Id,
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.LastName,
                    Email = coordinator.Email,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    CreatedTime = coordinator.CreatedTime,
                    Institutions = new List<InstitutionReport>()
                };
                coordinatorForHealthcareOrganizationList.Add(coordinatorForHealthcareOrganization);
                return coordinatorForHealthcareOrganization;
            }

            private List<Coordinator> GetCoordinatorsForInstitutionsInHealthcareOrganization(int healthcareOrganizationId)
            {
                return _context.User.OfType<Coordinator>()
                    .AsNoTracking()
                    .Include(b => b.Institution)
                        .ThenInclude(i => i.HealthcareOrganization)
                    .Include(b => b.Institution)
                        .ThenInclude(i => i.InstitutionType)
                    .Where(b => b.Institution.HealthcareOrganization.Id == healthcareOrganizationId &&
                                b.IsDeactivated == false)
                    .OrderBy(b => b.LastName)
                    .ToList();
            }
        }
    }
}
