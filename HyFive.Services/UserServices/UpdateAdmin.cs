using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.User;
using Bruker = HyFive.Models.V1.User.User;
using Microsoft.AspNetCore.Http;

namespace HyFive.Services.UserServices
{
    public class UpdateAdmin
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public Models.V1.User.User User { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.User.User>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(HandHygieneContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Models.V1.User.User> Handle(Command command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(command.User.FirstName))
                {
                    throw new Exception("Missing first name.");
                }
                if (string.IsNullOrWhiteSpace(command.User.LastName))
                {
                    throw new Exception("Missing last name.");
                }

                var user = await _context.User.OfType<Admin>().FirstOrDefaultAsync(i => i.Id == command.User.Id);
                if (user == null)
                    throw new Exception($"User with Id not found {command.User.Id}");
                //if (_currentUser.PidPseudonym == user.IdentityPseudonym)
                //    throw new Exception($"User cannot change themselves.");
                if (user.IdentityPseudonym != command.User.IdentityPseudonym)
                {
                    var existingPseudonym = await _context.User.OfType<Admin>().AnyAsync(x => x.IdentityPseudonym == command.User.IdentityPseudonym);
                    if (existingPseudonym)
                        throw new Exception("User cannot be updated. The pseudonym is already in use.");
                }

                user.FirstName = command.User.FirstName;
                user.LastName = command.User.LastName;
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
