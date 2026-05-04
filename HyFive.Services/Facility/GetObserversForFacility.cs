using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class GetObserversForFacility
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
                const string observerPermission = PermissionLevelConstants.Observer;

                var userIdsQuery =
                    from p in _context.UserPermission.AsNoTracking()
                    join ou in _context.OrganisationUnit.AsNoTracking() on p.OrganisationUnitId equals ou.Id

                    join parent in _context.OrganisationUnit.AsNoTracking()
                        on ou.ParentId equals parent.Id into p1
                    from parent in p1.DefaultIfEmpty()

                    join grandParent in _context.OrganisationUnit.AsNoTracking()
                        on parent.ParentId equals grandParent.Id into p2
                    from grandParent in p2.DefaultIfEmpty()

                    where p.PermissionLevel == observerPermission
                          && (
                              (ou.Id == request.FacilityId && ou.ParentId == null)
                              ||
                              (parent != null && parent.Id == request.FacilityId && parent.ParentId == null)
                              ||
                              (grandParent != null && grandParent.Id == request.FacilityId && grandParent.ParentId == null)
                          )
                    select p.UserId;

                var userIds = await userIdsQuery
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (userIds.Count == 0)
                    return Array.Empty<Models.V1.User.User>();

                var users = await _context.User
                    .AsNoTracking()
                    .Where(u => userIds.Contains(u.Id))
                    .Include(u => u.UserPermissions)
                        .ThenInclude(up => up.OrganisationUnit)
                    .Include(u => u.UserIdentifiers)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync(cancellationToken);

                return users
                    .Select(x => _mapper.Map<Models.V1.User.User>(x))
                    .ToArray();
            }
        }
    }
}
