using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using HyFive.Models.V1.Facility;
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
                var coordinatorsForFacilitiesInHealthcareOrganization = GetCoordinatorsForFacilitiesInHealthcareOrganization(request.HealthcareOrganizationId);

                List<HealthcareOrganizationCoordinator> coordinatorForHealthcareOrganizationList = CreateCoordinatorsForHealthcareOrganizationList(coordinatorsForFacilitiesInHealthcareOrganization);

                return coordinatorForHealthcareOrganizationList.ToArray();
            }

            private static List<HealthcareOrganizationCoordinator> CreateCoordinatorsForHealthcareOrganizationList(List<Coordinator> coordinatorsForFacilitiesInHealthcareOrganization)
            {
                var coordinatorsListForHealthcareOrganization = new List<HealthcareOrganizationCoordinator>();

                foreach (var coordinator in coordinatorsForFacilitiesInHealthcareOrganization)
                {
                    var coordinatorForHealthcareOrganization = coordinatorsListForHealthcareOrganization.FirstOrDefault(k => k.Email == coordinator.Email);
                    if (coordinatorForHealthcareOrganization == null)
                    {
                        coordinatorForHealthcareOrganization = CreateCoordinatorForHealthcareOrganization(coordinatorsListForHealthcareOrganization, coordinator);
                    }

                    AddFacility(coordinator, coordinatorForHealthcareOrganization);
                }

                return coordinatorsListForHealthcareOrganization;
            }

            private static void AddFacility(Coordinator coordinator, HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization)
            {
                var facilityReport = new FacilityReport
                {
                    Abbreviation = coordinator.Facility.Abbreviation,
                    HERId = coordinator.Facility.HERId,
                    Id = coordinator.Facility.Id,
                    Name = coordinator.Facility.Name,
                    FacilityType = new FacilityType
                    {
                        Id = coordinator.Facility.FacilityType.Id,
                        Code = coordinator.Facility.FacilityType.Code,
                        Name = coordinator.Facility.FacilityType.Name
                    }
                };

                coordinatorForHealthcareOrganization.Facilities.Add(facilityReport);
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
                    Facilities = new List<FacilityReport>()
                };
                coordinatorForHealthcareOrganizationList.Add(coordinatorForHealthcareOrganization);
                return coordinatorForHealthcareOrganization;
            }

            private List<Coordinator> GetCoordinatorsForFacilitiesInHealthcareOrganization(int healthcareOrganizationId)
            {
                return _context.User.OfType<Coordinator>()
                    .AsNoTracking()
                    .Include(b => b.Facility)
                        .ThenInclude(i => i.HealthcareOrganization)
                    .Include(b => b.Facility)
                        .ThenInclude(i => i.FacilityType)
                    .Where(b => b.Facility.HealthcareOrganization.Id == healthcareOrganizationId &&
                                b.IsDeactivated == false)
                    .OrderBy(b => b.LastName)
                    .ToList();
            }
        }
    }
}
