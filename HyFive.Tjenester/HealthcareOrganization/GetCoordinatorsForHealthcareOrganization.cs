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
                var coordinationsListForHealthcareOrganization = new List<HealthcareOrganizationCoordinator>();

                foreach (var coordinator in coordinatorsForInstitutionsInHealthcareOrganization)
                {
                    var coordinatorForHealthcareOrganization = coordinationsListForHealthcareOrganization.FirstOrDefault(k => (!string.IsNullOrEmpty(k.HPRNumber) && k.HPRNumber == coordinator.HPRNumber) ||
                                                                                                         (!string.IsNullOrEmpty(k.IdentityPseudonym) && k.IdentityPseudonym == coordinator.IdentityPseudonym));
                    if (coordinatorForHealthcareOrganization == null)
                    {
                        coordinatorForHealthcareOrganization = CreateCoordinatorForHealthcareOrganization(coordinationsListForHealthcareOrganization, coordinator);
                    }

                    AddInstitution(coordinator, coordinatorForHealthcareOrganization);
                }

                return coordinationsListForHealthcareOrganization;
            }

            private static void AddInstitution(Coordinator coordinator, HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization)
            {
                var institusjonRapport = new InstitutionReport
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

                coordinatorForHealthcareOrganization.Institutions.Add(institusjonRapport);
            }

            private static HealthcareOrganizationCoordinator CreateCoordinatorForHealthcareOrganization(List<HealthcareOrganizationCoordinator> coordinatorForHealthcareOrganizationList, Coordinator coordinator)
            {
                var koordinatorForHelseforetak = new HealthcareOrganizationCoordinator
                {
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.LastName,
                    Email = coordinator.Email,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    CreatedTime = coordinator.CreatedTime,
                    Institutions = new List<InstitutionReport>()
                };
                coordinatorForHealthcareOrganizationList.Add(koordinatorForHelseforetak);
                return koordinatorForHelseforetak;
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
