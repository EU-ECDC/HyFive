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

namespace HyFive.Services.City
{
    public class UpdateCoordinatorForCity
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
                    if(!CanCoordinatorBeUpdated(command.Coordinator, out var errorMessage))
                        return new Status { Success = false, ErrorMessage = errorMessage };

                    var facilityIdList = command.Coordinator.Facilities.Select(x => x.Id);
                    List<Coordinator> coordinators = FindCoordinatorForFacilityInCity(command);
                    UpdateCoordinatorsForFacilityInCity(command.Coordinator, coordinators);

                    UpdateFacilityForCoordinator(command.Coordinator, facilityIdList, coordinators);

                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Error while updating coordinator");
                    return new Status { Success = false, ErrorMessage = e.Message };
                }

                return new Status { Success = true };
            }

            private static bool CanCoordinatorBeUpdated(CityCoordinator coordinator, out string errorMessage)
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
                

                return true;
            }

            private void UpdateFacilityForCoordinator(CityCoordinator coordinatorForCity, IEnumerable<int> facilityIds, List<Coordinator> coordinators)
            {
                if (coordinatorForCity.IsDisabled)
                    return;

                DeactivateCoordinatorForFacilityNotInList(coordinators, facilityIds);

                foreach (var facilityId in facilityIds)
                {
                    var coordinator = GetCoordinator(facilityId, coordinatorForCity.Email);
                    if (coordinator != null)
                    {
                        if (coordinator.IsDeactivated)
                            coordinator.IsDeactivated = false;
                    }
                    else
                    {
                        var newCoordinator = CreateCoordinatorForFacility(coordinatorForCity, facilityId);
                        _context.Add(newCoordinator);
                    }
                }
            }

            private static void UpdateCoordinatorsForFacilityInCity(CityCoordinator coordinatorForCity, List<Coordinator> coordinators)
            {
                foreach (var coordinator in coordinators)
                {
                    coordinator.FirstName = coordinatorForCity.FirstName;
                    coordinator.LastName = coordinatorForCity.LastName;
                    coordinator.Email = coordinatorForCity.Email;
                    coordinator.HPRNumber = coordinatorForCity.ModifiedHPRNumber;
                    coordinator.IdentityPseudonym = coordinatorForCity.ModifiedPseudonym;
                    coordinator.IsDeactivated = coordinatorForCity.IsDisabled;
                }
            }

            private List<Coordinator> FindCoordinatorForFacilityInCity(Command request)
            {
                return _context.Coordinator.Where(k => k.Facility.City.Id == request.CityId &&
                                                    (!string.IsNullOrEmpty(k.Email) &&
                                                    k.Email == request.Coordinator.Email)).ToList();
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

            private static void DeactivateCoordinatorForFacilityNotInList(List<Coordinator> coordinators, IEnumerable<int> facilityIds)
            {
                var coordinatorsNotInList = coordinators.Where(k => !facilityIds.Contains(k.Id));

                foreach (var coordinator in coordinatorsNotInList)
                {
                    coordinator.IsDeactivated = true;
                }
            }

            private Coordinator GetCoordinator(int facilityId, string email)
            {
                var coordinator = _context.Coordinator.FirstOrDefault(k => k.Facility.Id == facilityId &&
                                                                        (!string.IsNullOrEmpty(k.Email) && k.Email == email));
                return coordinator;
            }
        }
    }
}
