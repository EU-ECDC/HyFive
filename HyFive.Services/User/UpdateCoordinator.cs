using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.User
{
    public class UpdateCoordinator
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public CreateUpdateUserRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.User.User>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.User.User> Handle(Command command, CancellationToken cancellationToken)
            {
                var coordinator = await UserUpdateHelper.UpdateUserBaseFields(
                    _context,
                    command.Request,
                    PermissionLevelConstants.Coordinator,
                    cancellationToken
                );

                //ensure permission exists for Facility OU
                var facilityId = command.Request.FacilityId;

                var hasPermission = await _context.UserPermission
                    .AnyAsync(p => p.UserId == coordinator.Id && p.OrganisationUnitId == facilityId, cancellationToken);

                if (!hasPermission)
                {
                    _context.UserPermission.Add(new Domain.User.UserPermission
                    {
                        UserId = coordinator.Id,
                        OrganisationUnitId = facilityId,
                        PermissionLevel = PermissionLevelConstants.Coordinator
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Models.V1.User.User>(coordinator);
            }
        }
    }
}
