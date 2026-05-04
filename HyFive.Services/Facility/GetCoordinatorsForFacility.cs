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
    public class GetCoordinatorsForFacility
    {
        public class Query : IRequest<Models.V1.User.User[]>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.User.User[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.User.User[]> Handle(Query request, CancellationToken cancellationToken)
            {
                const string CoordinatorPermission = PermissionLevelConstants.Coordinator;

                // 1) Collect userIds that have coordinator permission that maps to THIS facility
                var coordinatorUserIdsQuery =
                    // Permission directly on facility OU
                    (from p in _context.UserPermission.AsNoTracking()
                     where p.PermissionLevel == CoordinatorPermission
                           && p.OrganisationUnitId.HasValue
                     join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId.Value equals ou.Id
                     where ou.ParentId == null && ou.Id == request.FacilityId
                     select p.UserId)

                    // Permission on department: parent is facility
                    .Union(
                        from p in _context.UserPermission.AsNoTracking()
                        where p.PermissionLevel == CoordinatorPermission
                              && p.OrganisationUnitId.HasValue
                        join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId.Value equals ou.Id
                        join parent in _context.OrganisationUnit.AsNoTracking() on ou.ParentId equals parent.Id
                        where parent.ParentId == null && parent.Id == request.FacilityId
                        select p.UserId
                    )

                    // Permission on unit: grandparent is facility
                    .Union(
                        from p in _context.UserPermission.AsNoTracking()
                        where p.PermissionLevel == CoordinatorPermission
                              && p.OrganisationUnitId.HasValue
                        join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId.Value equals ou.Id
                        join parent in _context.OrganisationUnit.AsNoTracking() on ou.ParentId equals parent.Id
                        join grandParent in _context.OrganisationUnit.AsNoTracking() on parent.ParentId equals grandParent.Id
                        where grandParent.ParentId == null && grandParent.Id == request.FacilityId
                        select p.UserId
                    );

                var coordinatorUserIds = await coordinatorUserIdsQuery
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (coordinatorUserIds.Count == 0)
                    return Array.Empty<HyFive.Models.V1.User.User>();

                var users = await _context.User
                    .AsNoTracking()
                    .Where(u => coordinatorUserIds.Contains(u.Id))
                    .Include(u => u.UserPermissions)
                        .ThenInclude(up => up.OrganisationUnit)
                    .Include(u => u.UserIdentifiers)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync(cancellationToken);

                return users
                    .Select(u => _mapper.Map<Models.V1.User.User>(u))
                    .ToArray();
            }
        }
    }
}
