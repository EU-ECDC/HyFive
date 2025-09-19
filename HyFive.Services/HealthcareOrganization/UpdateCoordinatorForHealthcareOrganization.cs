using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Services.User;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class UpdateCoordinatorForHealthcareOrganization
    {
        public class Command : IRequest<Status>
        {
            public HealthcareOrganizationCoordinator Coordinator { get; set; }
            public int HealthcareOrganizationId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Status>
        {
            private readonly HandHygieneContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(HandHygieneContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Status> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    if(!CanCoordinatorBeUpdated(command.Coordinator, out var errorMessage))
                        return new Status { Success = false, ErrorMessage = errorMessage };

                    var facilityIdList = command.Coordinator.Facilities.Select(x => x.Id);
                    List<Coordinator> coordinators = FindCoordinatorForFacilityInHealthcareOrganization(command);
                    UpdateCoordinatorsForFacilityInHealthcareOrganization(command.Coordinator, coordinators);

                    UpdateFacilityForCoordinator(command.Coordinator, facilityIdList, coordinators);

                    _context.SaveChanges();
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Error while updating coordinator");
                    return new Status { Success = false, ErrorMessage = e.Message };
                }

                return new Status { Success = true };
            }

            private bool CanCoordinatorBeUpdated(HealthcareOrganizationCoordinator coordinator, out string errorMessage)
            {
                errorMessage = "";

                if(string.IsNullOrWhiteSpace(coordinator.FirstName))
                {
                    errorMessage = "First name must be filled in";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(coordinator.LastName))
                {
                    errorMessage = "Last name must be filled in";
                    return false;
                }

                /*if(string.IsNullOrWhiteSpace(coordinator.ModifiedHPRNumber) && string.IsNullOrWhiteSpace(coordinator.ModifiedPseudonym))
                {
                    errorMessage = "HPR number or identity pseudonym must be filled in";
                    return false;
                }*/

                if(!string.IsNullOrWhiteSpace(coordinator.ModifiedPseudonym) && !UserValidator.IsValidIdentityPseudonym(coordinator.ModifiedPseudonym))
                {
                    errorMessage = "Identity pseudonym is not valid";
                    return false;
                }

                return true;
            }

            private void UpdateFacilityForCoordinator(HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization, IEnumerable<int> facilityIds, List<Coordinator> coordinators)
            {
                if (coordinatorForHealthcareOrganization.IsDisabled)
                    return;

                DeactivateCoordinatorForFacilityNotInList(coordinators, facilityIds);

                foreach (var facilityId in facilityIds)
                {
                    var coordinator = GetCoordinator(facilityId, coordinatorForHealthcareOrganization.Email);
                    if (coordinator != null)
                    {
                        if (coordinator.IsDeactivated)
                            coordinator.IsDeactivated = false;
                    }
                    else
                    {
                        var newCoordinator = CreateCoordinatorForFacility(coordinatorForHealthcareOrganization, facilityId);
                        _context.Add(newCoordinator);
                    }
                }
            }

            private static void UpdateCoordinatorsForFacilityInHealthcareOrganization(HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization, List<Coordinator> coordinators)
            {
                foreach (var coordinator in coordinators)
                {
                    coordinator.FirstName = coordinatorForHealthcareOrganization.FirstName;
                    coordinator.LastName = coordinatorForHealthcareOrganization.LastName;
                    coordinator.Email = coordinatorForHealthcareOrganization.Email;
                    coordinator.HPRNumber = coordinatorForHealthcareOrganization.ModifiedHPRNumber;
                    coordinator.IdentityPseudonym = coordinatorForHealthcareOrganization.ModifiedPseudonym;
                    coordinator.IsDeactivated = coordinatorForHealthcareOrganization.IsDisabled;
                }
            }

            private List<Coordinator> FindCoordinatorForFacilityInHealthcareOrganization(Command request)
            {
                return _context.Coordinator.Where(k => k.Facility.HealthcareOrganization.Id == request.HealthcareOrganizationId &&
                                                    ((!string.IsNullOrEmpty(k.Email) &&
                                                    k.Email == request.Coordinator.Email))).ToList();
            }

            private Coordinator CreateCoordinatorForFacility(HealthcareOrganizationCoordinator coordinator, int facilityId)
            {
                var facility = _context.Facility.FirstOrDefault(i => i.Id == facilityId);
                var newCoordinator = new Coordinator
                {
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.LastName,
                    Email = coordinator.Email,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    Facility = facility
                };
                return newCoordinator;
            }

            private void DeactivateCoordinatorForFacilityNotInList(List<Coordinator> coordinators, IEnumerable<int> facilityIds)
            {
                var coordinatorsNotInList = coordinators.Where(k => !facilityIds.Contains(k.Id));

                coordinatorsNotInList.All(k => k.IsDeactivated = true);
            }

            private Coordinator GetCoordinator(int facilityId, string email)
            {
                var coordinator = _context.Coordinator.FirstOrDefault(k => k.Facility.Id == facilityId &&
                                                                        ((!string.IsNullOrEmpty(k.Email) && k.Email == email)));
                return coordinator;
            }
        }
    }
}
