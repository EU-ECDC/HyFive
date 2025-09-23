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

namespace HyFive.Services.City
{
    public class CreateCoordinatorForCity
    {
        public class Command : IRequest<Status>
        {
            public CityCoordinator Coordinator { get; set; }
            public int CityId { get; set; }
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

                    var facilityIds = command.Coordinator.Facilities.Select(x => x.Id);

                    foreach (var facilityId in facilityIds)
                    {
                        var coordinator = GetCoordinator(facilityId, command.Coordinator.Email);
                        if (coordinator != null)
                        {
                            if (coordinator.IsDeactivated)
                                coordinator.IsDeactivated = false;
                        }
                        else
                        {
                            var newCoordinator = CreateCoordinatorForFacility(command.Coordinator, facilityId);

                            // Coordinator must also be an observer for the same facility
                            var newObserver = CreateObserverForFacility(command.Coordinator, facilityId);

                            _context.Add(newCoordinator);
                            _context.Add(newObserver);
                        }
                    }

                _context.SaveChanges();

                
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Error while updating coordinator");
                    return new Status { Success = false, ErrorMessage = e.Message
        };
    }

                return new Status { Success = true };
            }

            private Coordinator CreateCoordinatorForFacility(CityCoordinator coordinator, int facilityId)
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

            private Observer CreateObserverForFacility(CityCoordinator coordinator, int facilityId)
            { 
                var facility = _context.Facility.FirstOrDefault(i => i.Id == facilityId);

                var observator = new Observer
                {
                    FirstName = coordinator.FirstName,
                    LastName = coordinator.LastName,
                    Email = coordinator.Email,
                    HPRNumber = coordinator.HPRNumber,
                    IdentityPseudonym = coordinator.IdentityPseudonym,
                    Facility = facility
                };

                return observator;
            }

            private Coordinator GetCoordinator(int facilityId, string email)
            {
                var coordinator = _context.Coordinator.FirstOrDefault(k => k.Facility.Id == facilityId &&
                                                                        ((!string.IsNullOrEmpty(k.Email) &&
                                                                        k.Email == email)));
                return coordinator;
            }

            private bool CanCoordinatorBeUpdated(CityCoordinator coordinator, out string errorMessage)
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

                if (string.IsNullOrWhiteSpace(coordinator.Email))
                {
                    errorMessage = "Email must be filled in";
                    return false;
                }

                //if (!string.IsNullOrWhiteSpace(coordinator.IdentityPseudonym) && !UserValidator.IsValidIdentityPseudonym(coordinator.IdentityPseudonym))
                //{
                //    errorMessage = " Identity pseudonym is not valid";
                //    return false;
                //}

                return true;
            }
        }
    }
}
