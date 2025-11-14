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

namespace HyFive.Services.City
{
    public class GetCoordinatorsForCity
    {
        public class Query : IRequest<CityCoordinator[]>
        {
            public int CityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, CityCoordinator[]>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }


            public async Task<CityCoordinator[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var coordinatorsForFacilitiesInCity = await GetCoordinatorsForFacilitiesInCity(request.CityId);

                List<CityCoordinator> coordinatorForCityList = CreateCoordinatorsForCityList(coordinatorsForFacilitiesInCity);

                return coordinatorForCityList.ToArray();
            }

            private static List<CityCoordinator> CreateCoordinatorsForCityList(List<Coordinator> coordinatorsForFacilitiesInCity)
            {
                var coordinatorsListForCity = new List<CityCoordinator>();

                foreach (var coordinator in coordinatorsForFacilitiesInCity)
                {
                    var coordinatorForCity = coordinatorsListForCity.FirstOrDefault(k => k.Email == coordinator.Email);
                    if (coordinatorForCity == null)
                    {
                        coordinatorForCity = CreateCoordinatorForCity(coordinatorsListForCity, coordinator);
                    }

                    AddFacility(coordinator, coordinatorForCity);
                }

                return coordinatorsListForCity;
            }

            private static void AddFacility(Coordinator coordinator, CityCoordinator coordinatorForCity)
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

                coordinatorForCity.Facilities.Add(facilityReport);
            }

            private static CityCoordinator CreateCoordinatorForCity(List<CityCoordinator> coordinatorForCityList, Coordinator coordinator)
            {
                var coordinatorForCity = new CityCoordinator
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
                coordinatorForCityList.Add(coordinatorForCity);
                return coordinatorForCity;
            }

            private async Task<List<Coordinator>> GetCoordinatorsForFacilitiesInCity(int cityId)
            {
                return await _context.User.OfType<Coordinator>()
                    .AsNoTracking()
                    .Include(b => b.Facility)
                        .ThenInclude(i => i.City)
                    .Include(b => b.Facility)
                        .ThenInclude(i => i.FacilityType)
                    .Where(b => b.Facility.City.Id == cityId &&
                                !b.IsDeactivated)
                    .OrderBy(b => b.LastName)
                    .ToListAsync();
            }
        }
    }
}
