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
                    if(!CanCoordinatorBeUpdated(command.Coordinator, out var feilmelding))
                        return new Status { Success = false, ErrorMessage = feilmelding };

                    var institusjonIdListe = command.Coordinator.Institutions.Select(x => x.Id);
                    List<Coordinator> coordinators = FindCoordinatorForInstitutionInHealthcareOrganization(command);
                    UpdateCoordinatorsForInstitutionInHealthcareOrganization(command.Coordinator, coordinators);

                    UpdateInstitutionForCoordinator(command.Coordinator, institusjonIdListe, coordinators);

                    _context.SaveChanges();
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Feil under oppdatering av koordinator");
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

                if (string.IsNullOrWhiteSpace(coordinator.Surname))
                {
                    errorMessage = "Last name must be filled in";
                    return false;
                }

                if(string.IsNullOrWhiteSpace(coordinator.ModifiedHPRNumber) && string.IsNullOrWhiteSpace(coordinator.ModifiedPseudonym))
                {
                    errorMessage = "HPR number or identity pseudonym must be filled in";
                    return false;
                }

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
                    var coordinator = GetCoordinator(institutionId, coordinatorForHealthcareOrganization.HPRNumber, coordinatorForHealthcareOrganization.IdentityPseudonym);
                    if (coordinator != null)
                    {
                        if (coordinator.IsDisabled)
                            coordinator.IsDisabled = false;
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
                    coordinator.LastName = coordinatorForHealthcareOrganization.Surname;
                    coordinator.Email = coordinatorForHealthcareOrganization.Email;
                    coordinator.HPRNumber = coordinatorForHealthcareOrganization.ModifiedHPRNumber;
                    coordinator.IdentityPseudonym = coordinatorForHealthcareOrganization.ModifiedPseudonym;
                    coordinator.IsDisabled = coordinatorForHealthcareOrganization.IsDisabled;
                }
            }

            private List<Coordinator> FindCoordinatorForInstitutionInHealthcareOrganization(Command request)
            {
                return _context.Coordinator.Where(k => k.Institution.HealthcareOrganization.Id == request.HealthcareOrganizationId &&
                                                    ((!string.IsNullOrEmpty(k.HPRNumber) &&
                                                    k.HPRNumber == request.Coordinator.HPRNumber) ||
                                                    (!string.IsNullOrEmpty(k.IdentityPseudonym) &&
                                                    k.IdentityPseudonym == request.Coordinator.IdentityPseudonym))).ToList();
            }

            private Coordinator CreateCoordinatorForInstitution(HealthcareOrganizationCoordinator coordinator, int institutionId)
            {
                var institution = _context.Institution.FirstOrDefault(i => i.Id == institutionId);
                var newCoordinator = new Coordinator
                {
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.Surname,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    Institution = institution
                };
                return newCoordinator;
            }

            private void DeactivateCoordinatorForInstitutionNotInList(List<Coordinator> coordinators, IEnumerable<int> institutionIds)
            {
                var coordinatorsNotInList = coordinators.Where(k => !institutionIds.Contains(k.Id));

                coordinatorsNotInList.All(k => k.IsDisabled = true);
            }

            private Coordinator GetCoordinator(int institutionId, string hprNumber, string identityPseudonym)
            {
                var coordinator = _context.Coordinator.FirstOrDefault(k => k.Institution.Id == institutionId &&
                                                                        ((!string.IsNullOrEmpty(k.HPRNumber) &&
                                                                        k.HPRNumber == hprNumber) ||
                                                                        (!string.IsNullOrEmpty(k.IdentityPseudonym) &&
                                                                        k.IdentityPseudonym == identityPseudonym)));
                return coordinator;
            }
        }
    }
}
