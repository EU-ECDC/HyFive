using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.User;
using Bruker = HyFive.Models.V1.User.User;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
                    throw new ArgumentException("Missing first name.");
                }
                if (string.IsNullOrWhiteSpace(command.User.LastName))
                {
                    throw new ArgumentException("Missing last name.");
                }

                var user = await _context.User.OfType<Admin>().FirstOrDefaultAsync(i => i.Id == command.User.Id);
                if (user == null)
                    throw new ArgumentException($"User not found with Id: {command.User.Id}");

                var currentUserEmail = _httpContextAccessor.HttpContext?.User?
                    .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;

                // Prevent self-update if emails match
                if (!string.IsNullOrEmpty(currentUserEmail) &&
                    string.Equals(currentUserEmail, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("User cannot change themselves");
                }

                if (user.Email != command.User.Email)
                {
                    var existingEmail = await _context.User.OfType<Admin>().AnyAsync(x => x.Email == command.User.Email, cancellationToken);
                    if (existingEmail)
                        throw new ArgumentException("User cannot be updated. The email is already in use.");
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
