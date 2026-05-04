using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.User
{
    public class CreateObserver
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
                var req = command.Request;

                if (!UserValidator.HasNameAndEmail(command.Request))
                {
                    throw new ValidationException("ObserverMissingDetails");
                }

                // validate facility OU exists and is actually a Facility
                var facility = await _context.OrganisationUnit
                    .Include(ou => ou.LevelRef)
                    .FirstOrDefaultAsync(ou => ou.Id == req.FacilityId, cancellationToken);

                if (facility == null)
                    throw new DomainException("FacilityNotFound", req.FacilityId);

                if (facility.LevelRef?.Level != OrganisationUnitLevels.Facility)
                    throw new DomainException("OrganisationUnitIsNotFacility", req.FacilityId);

                // create observer user
                var normalizedEmail = req.Email?.Trim();

                var existingUser = await _context.User
                    .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

                Domain.User.User observer;

                if (existingUser == null)
                {
                    observer = new Observer
                    {
                        FirstName = req.FirstName.Trim(),
                        LastName = req.LastName.Trim(),
                        Email = req.Email.Trim(),
                        IdentityPseudonym = req.IdentityPseudonym?.Trim(),
                        IsDeactivated = false
                    };

                    _context.User.Add(observer);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    var isObserverInThisFacility = await _context.UserPermission
                         .AnyAsync(p =>
                             p.UserId == existingUser.Id &&
                             p.OrganisationUnitId == req.FacilityId &&
                             p.PermissionLevel == PermissionLevelConstants.Coordinator,
                             cancellationToken);

                    if (isObserverInThisFacility)
                        throw new ValidationException("EmailAlreadyUsed", req.Email);

                    observer = existingUser;
                }

                // link observer to facility via permission
                var permission = new Domain.User.UserPermission
                {
                    UserId = observer.Id,
                    OrganisationUnitId = facility.Id,
                    PermissionLevel = PermissionLevelConstants.Observer
                };

                _context.UserPermission.Add(permission);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Models.V1.User.User>(observer);
            }
        }
    }
}
