using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Services.User;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class CreateCoordinatorForHealthcareOrganization
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
                    if (!CanCoordinatorBeUpdated(command.Coordinator, out var errorMessage))
                        return new Status { Success = false, ErrorMessage = errorMessage };

                    var institutionIds = command.Coordinator.Institutions.Select(x => x.Id);

                    foreach (var institutionId in institutionIds)
                    {
                        var coordinator = GetCoordinator(institutionId, command.Coordinator.Email, command.Coordinator.HPRNumber, command.Coordinator.IdentityPseudonym);
                        if (coordinator != null)
                        {
                            if (coordinator.IsDeactivated)
                                coordinator.IsDeactivated = false;
                        }
                        else
                        {
                            var newCoordinator = CreateCoordinatorForInstitution(command.Coordinator, institutionId);

                            // Coordinator must also be an observer for the same institution
                            var newObserver = CreateObserverForInstitution(command.Coordinator, institutionId);

                            _context.Add(newCoordinator);
                            _context.Add(newObserver);
                        }
                    }

                _context.SaveChanges();

                
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Feil under oppdatering av koordinator");
                    return new Status { Success = false, ErrorMessage = e.Message
        };
    }

                return new Status { Success = true };
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

            private Observer CreateObserverForInstitution(HealthcareOrganizationCoordinator coordinator, int institutionId)
            { 
                var institution = _context.Institution.FirstOrDefault(i => i.Id == institutionId);

                var observator = new Observer
                {
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.LastName,
                    Email = coordinator.Email,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    Institution = institution
                };

                return observator;
            }

            private Coordinator GetCoordinator(int institutionId, string email, string hprNumber, string identityPseudonym)
            {
                var coordinator = _context.Coordinator.FirstOrDefault(k => k.Institution.Id == institutionId &&
                                                                        ((!string.IsNullOrEmpty(k.Email) &&
                                                                        k.Email == email)));
                return coordinator;
            }

            private bool CanCoordinatorBeUpdated(HealthcareOrganizationCoordinator coordinator, out string errorMessage)
            {
                errorMessage = "";

                if (string.IsNullOrWhiteSpace(coordinator.FirstName))
                {
                    errorMessage = "First name must be filled in";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(coordinator.LastName))
                {
                    errorMessage = "Last name must be filled in";
                    return false;
                }

                /*if (string.IsNullOrWhiteSpace(coordinator.HPRNumber) && string.IsNullOrWhiteSpace(coordinator.IdentityPseudonym))
                {
                    errorMessage = "HPR number or identity pseudonym must be filled in";
                    return false;
                }*/

                if (!string.IsNullOrWhiteSpace(coordinator.IdentityPseudonym) && !UserValidator.IsValidIdentityPseudonym(coordinator.IdentityPseudonym))
                {
                    errorMessage = " Identity pseudonym is not valid";
                    return false;
                }

                return true;
            }
        }
    }
}
