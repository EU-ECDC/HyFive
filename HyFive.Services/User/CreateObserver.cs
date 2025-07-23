using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
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
                if (!UserValidator.HasNameAndHprNumberOrValidPseudonym(command.User))
                {
                    throw new ArgumentException("Observer must have first name, last name, and either HPR number or pseudonym");
                }
                
                var institution = await _context.Institution.FirstOrDefaultAsync(i => i.Id == command.User.InstitutionId);
                if (institution == null)
                {
                    throw new Exception("Did not find institution with ID. " + command.User.InstitutionId);
                }

                var observer = new Observer()
                {
                    FirstName = command.User.FirstName,
                    LastName = command.User.LastName,
                    Email = command.User.Email,
                    Institution = institution,
                    HPRNumber = command.User.HPRNumber,
                    IdentityPseudonym = command.User.IdentityPseudonym,
                    CreatedTime = DateTime.UtcNow,
                    IsDeactivated = false
                };
                _context.User.Add(observer);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.User.User>(observer);
            }
        }
    }
}
