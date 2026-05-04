using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace HyFive.Services.City
{
    public class GetCoordinatorsForCity
    {
        public class Query : IRequest<CityCoordinator[]>
        {
            public string City { get; set; }
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
                if (string.IsNullOrWhiteSpace(request.City))
                    return Array.Empty<CityCoordinator>();

                var city = request.City.Trim();

                var facilityIdsInCity = await GetFacilityIdsInCity(city, cancellationToken);
                if (facilityIdsInCity.Count == 0)
                    return Array.Empty<CityCoordinator>();

                var coordinatorFacilityPairs = await GetCoordinatorFacilityPairs(facilityIdsInCity, cancellationToken);
                if (coordinatorFacilityPairs.Count == 0)
                    return Array.Empty<CityCoordinator>();

                var facilityLookup = await GetFacilityLookup(facilityIdsInCity, cancellationToken);

                var result = CreateCoordinatorsForCityList(coordinatorFacilityPairs, facilityLookup);
                return result.ToArray();
            }

            private async Task<List<int>> GetFacilityIdsInCity(string city, CancellationToken ct)
            {
                return await
                    (from f in _context.OrganisationUnit.AsNoTracking()
                     join a in _context.Address.AsNoTracking() on f.AddressId equals a.Id
                     where f.ParentId == null
                           && a.City != null
                           && EF.Functions.ILike(a.City, city)
                     select f.Id)
                    .Distinct()
                    .ToListAsync(ct);
            }

            private async Task<List<(Coordinator Coordinator, int FacilityId)>> GetCoordinatorFacilityPairs(
                List<int> facilityIdsInCity,
                CancellationToken ct)
            {
                //Permission is directly on a Facility
                var case1 =
                    from u in _context.User.AsNoTracking().OfType<Coordinator>()
                    where !u.IsDeactivated
                    join p in _context.UserPermission.AsNoTracking() on u.Id equals p.UserId
                    join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId equals ou.Id
                    where ou.ParentId == null && facilityIdsInCity.Contains(ou.Id)
                    select new { Coordinator = u, FacilityId = ou.Id };

                //Permission is directly on a Facility
                var case2 =
                    from u in _context.User.AsNoTracking().OfType<Coordinator>()
                    where !u.IsDeactivated
                    join p in _context.UserPermission.AsNoTracking() on u.Id equals p.UserId
                    join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId equals ou.Id
                    join parent in _context.OrganisationUnit.AsNoTracking() on ou.ParentId equals parent.Id
                    where parent.ParentId == null && facilityIdsInCity.Contains(parent.Id)
                    select new { Coordinator = u, FacilityId = parent.Id };

                //Permission is on a Department
                var case3 =
                    from u in _context.User.AsNoTracking().OfType<Coordinator>()
                    where !u.IsDeactivated
                    join p in _context.UserPermission.AsNoTracking() on u.Id equals p.UserId
                    join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId equals ou.Id
                    join parent in _context.OrganisationUnit.AsNoTracking() on ou.ParentId equals parent.Id
                    join grandParent in _context.OrganisationUnit.AsNoTracking() on parent.ParentId equals grandParent.Id
                    where grandParent.ParentId == null && facilityIdsInCity.Contains(grandParent.Id)
                    select new { Coordinator = u, FacilityId = grandParent.Id };

                var result = await case1
                    .Union(case2)
                    .Union(case3)
                    .Distinct()
                    .ToListAsync(ct);

                return result
                    .Select(x => (x.Coordinator, x.FacilityId))
                    .ToList();
            }

            private async Task<Dictionary<int, FacilityReport>> GetFacilityLookup(List<int> facilityIds, CancellationToken ct)
            {
                return await
                    (from f in _context.OrganisationUnit.AsNoTracking()
                     where facilityIds.Contains(f.Id)
                     join a in _context.Address.AsNoTracking() on f.AddressId equals a.Id into a1
                     from a in a1.DefaultIfEmpty()
                     join t in _context.OrganisationUnitType.AsNoTracking() on f.TypeId equals t.Id
                     select new FacilityReport
                     {
                         Id = f.Id,
                         Name = f.Name,
                         Abbreviation = f.Abbreviation,
                         City = a.City,
                         Type = new Models.V1.OrganisationUnit.OrganisationUnitType
                         {
                             Id = t.Id,
                             Code = t.Code,
                             Name = t.Name,
                             Description = t.Description
                         }
                     })
                    .ToDictionaryAsync(x => x.Id, ct);
            }

            private static List<CityCoordinator> CreateCoordinatorsForCityList(
                List<(Coordinator Coordinator, int FacilityId)> pairs,
                Dictionary<int, FacilityReport> facilities)
            {
                var result = new List<CityCoordinator>();

                foreach (var (coordinator, facilityId) in pairs.OrderBy(x => x.Coordinator.LastName))
                {
                    var dto = result.FirstOrDefault(x => x.Id == coordinator.Id);
                    if (dto == null)
                    {
                        dto = new CityCoordinator
                        {
                            Id = coordinator.Id,
                            FirstName = coordinator.FirstName,
                            LastName = coordinator.LastName,
                            Email = coordinator.Email,
                            IdentityPseudonym = coordinator.IdentityPseudonym,
                            CreatedTime = coordinator.CreatedTime,
                            IsDeactivated = coordinator.IsDeactivated,
                            Facilities = new List<FacilityReport>()
                        };
                        result.Add(dto);
                    }

                    if (facilities.TryGetValue(facilityId, out var fac)
                        && !dto.Facilities.Any(f => f.Id == fac.Id))
                    {
                        dto.Facilities.Add(fac);
                    }
                }

                return result;
            }
        }
    }
}
