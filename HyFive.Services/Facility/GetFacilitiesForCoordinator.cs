using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class GetFacilitiesForCoordinator
    {
        public class Query : IRequest<Models.V1.OrganisationUnit.FacilityReport[]>
        {
            public string CoordinatorEmail { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.OrganisationUnit.FacilityReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.OrganisationUnit.FacilityReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var email = (request.CoordinatorEmail ?? string.Empty).Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(email))
                    return Array.Empty<Models.V1.OrganisationUnit.FacilityReport>();

                // 1) Resolve DB user id
                var userId = await _context.User.AsNoTracking()
                    .Where(u => !u.IsDeactivated && u.Email != null && u.Email.ToLower() == email)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (userId == 0)
                    return Array.Empty<Models.V1.OrganisationUnit.FacilityReport>();

                // 2) Facility ids the user can access (permission can be on facility, dept, or unit)
                var facilityIdsQuery =
                    // Permission directly on facility
                    from p in _context.UserPermission.AsNoTracking()
                    where p.UserId == userId && p.OrganisationUnitId.HasValue
                    join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId.Value equals ou.Id
                    where ou.ParentId == null
                    select ou.Id;

                var deptToFacilityQuery =
                    // Permission on department -> parent is facility
                    from p in _context.UserPermission.AsNoTracking()
                    where p.UserId == userId && p.OrganisationUnitId.HasValue
                    join dept in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId.Value equals dept.Id
                    join facility in _context.OrganisationUnit.AsNoTracking() on dept.ParentId equals facility.Id
                    where facility.ParentId == null
                    select facility.Id;

                var unitToFacilityQuery =
                    // Permission on unit -> parent is dept -> parent is facility
                    from p in _context.UserPermission.AsNoTracking()
                    where p.UserId == userId && p.OrganisationUnitId.HasValue
                    join unit in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId.Value equals unit.Id
                    join dept in _context.OrganisationUnit.AsNoTracking() on unit.ParentId equals dept.Id
                    join facility in _context.OrganisationUnit.AsNoTracking() on dept.ParentId equals facility.Id
                    where facility.ParentId == null
                    select facility.Id;

                var facilityIds = await facilityIdsQuery
                    .Union(deptToFacilityQuery)
                    .Union(unitToFacilityQuery)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (facilityIds.Count == 0)
                    return Array.Empty<Models.V1.OrganisationUnit.FacilityReport>();

                // 3) Return facility OUs
                return await _context.OrganisationUnit.AsNoTracking()
                    .Include(o => o.LevelRef)
                    .Include(o => o.Type)
                    .Include(o => o.Address)
                    .Where(o => facilityIds.Contains(o.Id))
                    .OrderBy(o => o.Name)
                    .ProjectTo<HyFive.Models.V1.OrganisationUnit.FacilityReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);
            }
        }
    }
}
