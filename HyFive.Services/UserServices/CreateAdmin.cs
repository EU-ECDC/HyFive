
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using HyFive.Services.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Bruker = HyFive.Models.V1.User.User;

namespace HyFive.Services.UserServices
{
    public class CreateAdmin
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public CreateAdminRequest Request { get; set; }
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
                if (string.IsNullOrWhiteSpace(command.Request.FirstName))
                {
                    throw new ArgumentException("Missing first name.");
                }
                if (string.IsNullOrWhiteSpace(command.Request.LastName))
                {
                    throw new ArgumentException("Missing last name.");
                }
                if (string.IsNullOrWhiteSpace(command.Request.Email))
                {
                    throw new ArgumentException("Missing email.");
                }

                try
                {
                    _ = new MailAddress(command.Request.Email);
                }
                catch
                {
                    throw new ArgumentException($"Email '{command.Request.Email}' is not valid.");
                }


                var existingUser = await _context.User
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == command.Request.Email, cancellationToken);

                if (existingUser != null)
                    throw new ArgumentException($"Email '{command.Request.Email}' is already in use.");

                var admin = new Admin()
                {
                    IdentityPseudonym = command.Request.IdentityPseudonym,
                    FirstName = command.Request.FirstName,
                    LastName = command.Request.LastName,
                    Email   = command.Request.Email,
                    IsDeactivated = false,
                    CreatedTime = DateTime.UtcNow,
                };

                _context.User.Add(admin);
                await _context.SaveChangesAsync();

                return _mapper.Map<Models.V1.User.User>(admin);
            }

            
        }
    }
}
