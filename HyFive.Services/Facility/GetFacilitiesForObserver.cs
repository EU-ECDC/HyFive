using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.Facility
{
    public class GetFacilitiesForObserver
    {
        public class Query : IRequest<Models.V1.OrganisationUnit.OrganisationUnit[]>
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.OrganisationUnit.OrganisationUnit[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _userService = userService;
            }

            public async Task<Models.V1.OrganisationUnit.OrganisationUnit[]> Handle(Query request, CancellationToken cancellationToken)
            {
                // 1) Load the observer
                var observer = await _context.User
                    .AsNoTracking()
                    .Where(_userService.HasEmailAndIsActive(request.Email))
                    .Where(u => u.UserPermissions.Any(p =>
                        p.PermissionLevel == PermissionLevelConstants.Observer))
                    .Select(u => new { u.Id })
                    .FirstOrDefaultAsync(cancellationToken);

                if (observer == null)
                    return Array.Empty<Models.V1.OrganisationUnit.OrganisationUnit>();

                // 2) Get OU ids the observer has permission to
                var permittedOuIds = await _context.UserPermission
                    .AsNoTracking()
                    .Where(p => p.UserId == observer.Id)
                    .Select(p => p.OrganisationUnitId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (permittedOuIds.Count == 0)
                    return Array.Empty<Models.V1.OrganisationUnit.OrganisationUnit>();

                // 3) Load OU index to resolve facility ancestors
                var ouIndex = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Select(x => new
                    {
                        x.Id,
                        x.ParentId,
                        Level = x.LevelRef.Level
                    })
                    .ToListAsync(cancellationToken);

                var byId = ouIndex.ToDictionary(x => x.Id);

                int? ResolveFacilityId(int startOuId)
                {
                    if (!byId.TryGetValue(startOuId, out var cur))
                        return null;

                    var safety = 0;
                    while (cur != null && safety++ < 50)
                    {
                        if (cur.Level == OrganisationUnitLevels.Facility)
                            return cur.Id;

                        if (!cur.ParentId.HasValue)
                            return null;

                        byId.TryGetValue(cur.ParentId.Value, out cur);
                    }

                    return null;
                }

                var facilityIds = permittedOuIds
                    .Where(id => id.HasValue)
                    .Select(id => ResolveFacilityId(id.Value))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToList();

                if (facilityIds.Count == 0)
                    return Array.Empty<Models.V1.OrganisationUnit.OrganisationUnit>();

                // 4) Load facilities + departments + units + roles
                var organisationUnits = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Include(x => x.Type)
                    .Include(x => x.Address)
                    .Include(x => x.OrganisationUnitRoles)
                        .ThenInclude(our => our.Role)
                    .Where(x =>
                        facilityIds.Contains(x.Id) ||
                        (x.ParentId.HasValue && facilityIds.Contains(x.ParentId.Value)) ||
                        (x.Parent != null && x.Parent.ParentId.HasValue && facilityIds.Contains(x.Parent.ParentId.Value)))
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                // 5) Map all OUs
                var mappedUnits = _mapper.Map<List<Models.V1.OrganisationUnit.OrganisationUnit>>(organisationUnits);

                var mappedById = mappedUnits.ToDictionary(x => x.Id);

                foreach (var item in mappedUnits)
                {
                    item.Children = new List<Models.V1.OrganisationUnit.OrganisationUnit>();

                    // keep roles only on departments
                    if (item.LevelRef?.Level != OrganisationUnitLevels.Department)
                    {
                        item.Roles = new List<Models.V1.Observation.Role>();
                    }
                }

                // 6) Build tree
                foreach (var item in mappedUnits)
                {
                    if (item.ParentId.HasValue && mappedById.TryGetValue(item.ParentId.Value, out var parent))
                    {
                        parent.Children.Add(item);
                    }
                }

                // 7) Return facilities only
                var facilities = mappedUnits
                    .Where(x => facilityIds.Contains(x.Id))
                    .OrderBy(x => x.Name)
                    .ToArray();

                return facilities;
            }
        }
    }
}