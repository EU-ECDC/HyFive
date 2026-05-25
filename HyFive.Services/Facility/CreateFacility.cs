using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class CreateFacility
    {
        public class Command : IRequest<Models.V1.OrganisationUnit.OrganisationUnit>
        {
            public CreateOrganisationUnitRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.OrganisationUnit.OrganisationUnit>
        {
            private readonly HandHygieneContext _context;

            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.OrganisationUnit.OrganisationUnit> Handle(Command command, CancellationToken cancellationToken)
            {
                var request = command.Request;

                //Validate OrganisationUnitType
                var type = await _context.Set<Domain.Place.OrganisationUnitType>()
                    .FirstOrDefaultAsync(t => t.Id == request.OrganisationUnitTypeId, cancellationToken);

                if (type == null)
                    throw new DomainException("OrganisationUnitTypeNotFound");

                var level = await _context.Set<Domain.Place.OrganisationUnitLevel>()
                    .FirstOrDefaultAsync(l => l.Level == OrganisationUnitLevels.Facility, cancellationToken);

                if (level == null)
                    throw new DomainException("OrganisationUnitLevelNotConfigured", "Facility");

                //Validate uniqueness (no duplicate root facility name)
                var exists = await _context.OrganisationUnit
                    .AnyAsync(x => x.ParentId == null && x.Name == request.Name, cancellationToken);

                if (exists)
                    throw new ValidationException("FacilityNameExists", request.Name);

                var address = new Domain.Place.Address
                {
                    CityId = request.CityId                    
                };

                //Create Coordinator Request
                var normalizedEmail = request.Email?.Trim();

                var existingUser = await _context.User
                    .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

                Domain.User.User coordinator;

                if (existingUser == null)
                {
                    coordinator = new Domain.User.User
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        IdentityPseudonym = request.Pseudonym
                    };
                    _context.User.Add(coordinator);
                }
                else {
                    coordinator = existingUser;
                }
                    

                //Create Unit
                var facility = new Domain.Place.OrganisationUnit
                {
                    ParentId = null,
                    Name = request.Name,
                    Abbreviation = request.Abbreviation,
                    Description = request.Description,
                    TypeId = type.Id,
                    LevelId = level.Id,
                    Address = address
                };

                _context.Add(facility);

                //Assign permission via UserPermission
                if (existingUser == null)
                {
                    var permission = new Domain.User.UserPermission
                    {
                        User = coordinator,
                        OrganisationUnit = facility,
                        PermissionLevel = PermissionLevelConstants.Coordinator
                    };

                    _context.Add(permission);
                }
                else
                {
                    var permission = new Domain.User.UserPermission
                    {
                        UserId = existingUser.Id,
                        OrganisationUnit = facility,
                        PermissionLevel = PermissionLevelConstants.Coordinator
                    };

                    _context.Add(permission);
                }

                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<OrganisationUnit>(facility);
                return mapped;
            }
        }
    }
}
