using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
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
                if (!UserValidator.HasNameAndEmail(command.User))
                {
                    throw new ArgumentException("Observer must have first name, last name, and email.");
                }
                
                var user = await _context.User.OfType<Observer>().FirstOrDefaultAsync(i => i.Id == command.User.Id);
                user.FirstName = command.User.FirstName;
                user.LastName = command.User.LastName;
                user.Email = command.User.Email;
                user.HPRNumber = command.User.HPRNumber;
                user.IdentityPseudonym = command.User.IdentityPseudonym;
                user.IsDeactivated = command.User.IsDisabled;
                _context.User.Update(user);
                
                await _context.SaveChangesAsync();
                var mapped = _mapper.Map<Models.V1.User.User>(user);
                return mapped;
            }
        }
    }
}
