using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.User;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.UserServices
{
    public class GetAdmin
    {
        public class Query : IRequest<Models.V1.User.User[]>
        {
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
                var users = await _context.User
                    .AsNoTracking()
                    .WithPermission(PermissionLevelConstants.Administrator)
                    .Include(u => u.UserPermissions)
                        .ThenInclude(p => p.OrganisationUnit)
                            .ThenInclude(ou => ou.Type)
                    .Include(u => u.UserPermissions)
                        .ThenInclude(p => p.OrganisationUnit)
                            .ThenInclude(ou => ou.LevelRef)
                    .Include(u => u.UserPermissions)
                        .ThenInclude(p => p.OrganisationUnit)
                            .ThenInclude(ou => ou.OrganisationUnitRoles)
                                .ThenInclude(our => our.Role)
                    .Include(u => u.UserIdentifiers)
                    .ToListAsync(cancellationToken);

                return _mapper.Map<Models.V1.User.User[]>(users);
            }
        }
    }
}
