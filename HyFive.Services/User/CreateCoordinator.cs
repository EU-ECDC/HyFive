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
            public Models.V1.User.User USer { get; set; }
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
                var institution = await _context.Institution.FirstOrDefaultAsync(i => i.Id == command.USer.InstitutionId);
                if (institution == null)
                {
                    throw new Exception("Could not find institution with ID: " + command.USer.InstitutionId);
                }

                if (!UserValidator.HasNameAndHprNumberOrValidPseudonym(command.USer))
                {
                    throw new ArgumentException("Coordinator must have first name, last name, and either HPR number or pseudonym");
                }

                var coordinator = new Coordinator()
                {
                    FirstName = command.USer.FirstName,
                    LastName = command.USer.LastName,
                    Email = command.USer.Email,
                    Institution = institution,
                    HPRNumber = command.USer.HPRNumber,
                    IdentityPseudonym = command.USer.IdentityPseudonym,
                    CreatedTime = DateTime.UtcNow,
                    IsDeactivated = false
                };


                // The coordinator must also be an observer for the same institution
                var observer = new Observer()
                {
                    FirstName = command.USer.FirstName,
                    LastName = command.USer.LastName,
                    Email = command.USer.Email,
                    Institution = institution,
                    HPRNumber = command.USer.HPRNumber,
                    IdentityPseudonym = command.USer.IdentityPseudonym,
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
