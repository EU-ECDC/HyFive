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
    public class CreateCoordinator
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

                if (!UserValidator.HasNameAndEmail(req))
                    throw new ValidationException("CoordinatorMissingDetails");

                //Validate facility OU exists + is Facility
                var facilityOrgUnit = await _context.OrganisationUnit
                    .Include(ou => ou.LevelRef)
                    .FirstOrDefaultAsync(ou => ou.Id == req.FacilityId, cancellationToken);

                if (facilityOrgUnit == null)
                    throw new DomainException("FacilityNotFound", req.FacilityId);

                if (facilityOrgUnit.LevelRef?.Level != OrganisationUnitLevels.Facility)
                    throw new DomainException("OrganisationUnitIsNotFacility", req.FacilityId);

                var normalizedEmail = req.Email?.Trim();

                var existingUser = await _context.User
        .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

                Domain.User.User coordinator;

                if (existingUser == null)
                {
                    coordinator = new Coordinator
                    {
                        FirstName = req.FirstName?.Trim(),
                        LastName = req.LastName?.Trim(),
                        Email = normalizedEmail,
                        IdentityPseudonym = req.IdentityPseudonym?.Trim(),
                        CreatedTime = DateTime.UtcNow,
                        IsDeactivated = false
                    };

                    _context.User.Add(coordinator);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    var isCoordinatorInThisFacility = await _context.UserPermission
                        .AnyAsync(p =>
                            p.UserId == existingUser.Id &&
                            p.OrganisationUnitId == req.FacilityId &&
                            p.PermissionLevel == PermissionLevelConstants.Coordinator,
                            cancellationToken);

                    if (isCoordinatorInThisFacility)
                        throw new ValidationException("EmailAlreadyUsed", req.Email);

                    coordinator = existingUser;
                }

                _context.UserPermission.Add(new Domain.User.UserPermission
                {
                    UserId = coordinator.Id,
                    OrganisationUnitId = req.FacilityId,
                    PermissionLevel = PermissionLevelConstants.Coordinator
                });

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Models.V1.User.User>(coordinator);
            }
        }
    }
}
