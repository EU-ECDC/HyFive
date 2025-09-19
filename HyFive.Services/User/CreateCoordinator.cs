using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.User
{
    public class CreateCoordinator
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public Models.V1.User.User User { get; set; }
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
                var facility = await _context.Facility.FirstOrDefaultAsync(i => i.Id == command.User.FacilityId);
                if (facility == null)
                {
                    throw new Exception("Did not find facility with ID: " + command.User.FacilityId);
                }

                if (!UserValidator.HasNameAndEmail(command.User))
                {
                    throw new ArgumentException("Coordinator must have first name, last name, and email");
                }

                var coordinator = new Coordinator()
                {
                    FirstName = command.User.FirstName,
                    LastName = command.User.LastName,
                    Email = command.User.Email,
                    Facility = facility,
                    HPRNumber = command.User.HPRNumber,
                    IdentityPseudonym = command.User.IdentityPseudonym,
                    CreatedTime = DateTime.UtcNow,
                    IsDeactivated = false
                };


                // The coordinator must also be an observer for the same facility
                var observer = new Observer()
                {
                    FirstName = command.User.FirstName,
                    LastName = command.User.LastName,
                    Email = command.User.Email,
                    Facility = facility,
                    HPRNumber = command.User.HPRNumber,
                    IdentityPseudonym = command.User.IdentityPseudonym,
                    CreatedTime = DateTime.UtcNow,
                    IsDeactivated = false
                };

                _context.User.Add(coordinator);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.User.User>(coordinator);
            }
        }
    }
}
