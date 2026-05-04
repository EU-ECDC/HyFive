using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
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
    public class UpdateObserver
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
                var observer = await UserUpdateHelper.UpdateUserBaseFields(
                    _context,
                    command.Request,
                    PermissionLevelConstants.Observer,
                    cancellationToken
                );

                //ensure observer has permission for the facility OU
                var hasPermission = await _context.UserPermission
                    .AnyAsync(p => p.UserId == observer.Id && p.OrganisationUnitId == command.Request.FacilityId, cancellationToken);

                if (!hasPermission)
                {
                    _context.UserPermission.Add(new Domain.User.UserPermission
                    {
                        UserId = observer.Id,
                        OrganisationUnitId = command.Request.FacilityId,
                        PermissionLevel = PermissionLevelConstants.Observer
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Models.V1.User.User>(observer);
            }
        }
    }
}
