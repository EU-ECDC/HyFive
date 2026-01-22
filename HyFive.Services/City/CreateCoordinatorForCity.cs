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



            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<Status> Handle(Command command, CancellationToken cancellationToken)
            {
                
                if (!CoordinatorForCityValidator.CanBeUpdated(command.Coordinator, out var errorCode, out var args))
                    throw new ValidationException(errorCode, args);

                var facilityIds = command.Coordinator.Facilities.Select(x => x.Id);

                foreach (var facilityId in facilityIds)
                {
                    var coordinator = CoordinatorForCityHelper.GetCoordinator(_context, facilityId, command.Coordinator.Email);
                    if (coordinator != null)
                    {
                        if (coordinator.IsDeactivated)
                            coordinator.IsDeactivated = false;
                    }
                    else
                    {
                        var newCoordinator = CoordinatorForCityHelper.CreateCoordinatorForFacility(_context, command.Coordinator, facilityId);

                        // Coordinator must also be an observer for the same facility
                        var newObserver = CreateObserverForFacility(command.Coordinator, facilityId);

                        _context.Add(newCoordinator);
                        _context.Add(newObserver);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new Status { Success = true };
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
        }
    }
}
