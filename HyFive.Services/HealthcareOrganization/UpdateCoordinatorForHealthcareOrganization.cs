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

                    var institutionIdList = command.Coordinator.Institutions.Select(x => x.Id);
                    List<Coordinator> coordinators = FindCoordinatorForInstitutionInHealthcareOrganization(command);
                    UpdateCoordinatorsForInstitutionInHealthcareOrganization(command.Coordinator, coordinators);

                    UpdateInstitutionForCoordinator(command.Coordinator, institutionIdList, coordinators);

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

            private void UpdateInstitutionForCoordinator(HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization, IEnumerable<int> institutionIds, List<Coordinator> coordinators)
            {
                if (coordinatorForHealthcareOrganization.IsDisabled)
                    return;

                DeactivateCoordinatorForInstitutionNotInList(coordinators, institutionIds);

                foreach (var institutionId in institutionIds)
                {
                    var coordinator = GetCoordinator(institutionId, coordinatorForHealthcareOrganization.Email);
                    if (coordinator != null)
                    {
                        if (coordinator.IsDeactivated)
                            coordinator.IsDeactivated = false;
                    }
                    else
                    {
                        var newCoordinator = CreateCoordinatorForInstitution(coordinatorForHealthcareOrganization, institutionId);
                        _context.Add(newCoordinator);
                    }
                }
            }

            private static void UpdateCoordinatorsForInstitutionInHealthcareOrganization(HealthcareOrganizationCoordinator coordinatorForHealthcareOrganization, List<Coordinator> coordinators)
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

            private List<Coordinator> FindCoordinatorForInstitutionInHealthcareOrganization(Command request)
            {
                return _context.Coordinator.Where(k => k.Institution.HealthcareOrganization.Id == request.HealthcareOrganizationId &&
                                                    ((!string.IsNullOrEmpty(k.Email) &&
                                                    k.Email == request.Coordinator.Email))).ToList();
            }

            private Coordinator CreateCoordinatorForInstitution(HealthcareOrganizationCoordinator coordinator, int institutionId)
            {
                var institution = _context.Institution.FirstOrDefault(i => i.Id == institutionId);
                var newCoordinator = new Coordinator
                {
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.LastName,
                    Email = coordinator.Email,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    Institution = institution
                };
                return newCoordinator;
            }

            private void DeactivateCoordinatorForInstitutionNotInList(List<Coordinator> coordinators, IEnumerable<int> institutionIds)
            {
                var coordinatorsNotInList = coordinators.Where(k => !institutionIds.Contains(k.Id));

                coordinatorsNotInList.All(k => k.IsDeactivated = true);
            }

            private Coordinator GetCoordinator(int institutionId, string email)
            {
                var coordinator = _context.Coordinator.FirstOrDefault(k => k.Institution.Id == institutionId &&
                                                                        ((!string.IsNullOrEmpty(k.Email) && k.Email == email)));
                return coordinator;
            }
        }
    }
}
