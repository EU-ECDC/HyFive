using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Services.Localization;
using HyFive.Services.User;
using MediatR;
using Microsoft.Extensions.Localization;
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
            

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<Status> Handle(Command command, CancellationToken cancellationToken)
            {
                
                if(!CoordinatorForCityValidator.CanBeUpdated(command.Coordinator, out var errorCode, out var args))
                    throw new ValidationException(errorCode, args);

                var facilityIdList = command.Coordinator.Facilities.Select(x => x.Id);
                List<Coordinator> coordinators = FindCoordinatorForFacilityInCity(command);
                UpdateCoordinatorsForFacilityInCity(command.Coordinator, coordinators);

                UpdateFacilityForCoordinator(command.Coordinator, facilityIdList, coordinators);

                await _context.SaveChangesAsync(cancellationToken);

                return new Status { Success = true };
            }
            
            private void UpdateFacilityForCoordinator(CityCoordinator coordinatorForCity, IEnumerable<int> facilityIds, List<Coordinator> coordinators)
            {
                if (coordinatorForCity.IsDisabled)
                    return;

                DeactivateCoordinatorForFacilityNotInList(coordinators, facilityIds);

                foreach (var facilityId in facilityIds)
                {
                    var coordinator = CoordinatorForCityHelper.GetCoordinator(_context, facilityId, coordinatorForCity.Email);
                    if (coordinator != null)
                    {
                        if (coordinator.IsDeactivated)
                            coordinator.IsDeactivated = false;
                    }
                    else
                    {
                        var newCoordinator = CoordinatorForCityHelper.CreateCoordinatorForFacility(_context, coordinatorForCity, facilityId);
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
            private static void DeactivateCoordinatorForFacilityNotInList(List<Coordinator> coordinators, IEnumerable<int> facilityIds)
            {
                var coordinatorsNotInList = coordinators.Where(k => !facilityIds.Contains(k.Id));

                foreach (var coordinator in coordinatorsNotInList)
                {
                    coordinator.IsDeactivated = true;
                }
            }
        }
    }
}
