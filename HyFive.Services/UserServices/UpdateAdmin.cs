using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Services.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

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

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.User.User> Handle(Command command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(command.User.FirstName))
                {
                    throw new ValidationException("FirstNameRequired");
                }
                if (string.IsNullOrWhiteSpace(command.User.LastName))
                {
                    throw new ValidationException("LastNameRequired");
                }
                if (string.IsNullOrWhiteSpace(command.User.Email))
                {
                    throw new ValidationException("EmailRequired");
                }
                try
                {
                    var addr = new MailAddress(command.User.Email);
                
                    if (addr.Address != command.User.Email)
                        throw new ValidationException("EmailNotValid", command.User.Email);
                }
                catch (FormatException)
                {
                    throw new DomainException("UserNotFound", command.User.Id);
                }


                var user = await _context.User.OfType<Admin>().FirstOrDefaultAsync(i => i.Id == command.User.Id);
                    if (user == null)
                        throw new DomainException("UserNotFound", command.User.Id);

                if (!string.Equals(user.Email, command.User.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailInUse = await _context.User.OfType<Admin>()
                        .AnyAsync(x => x.Email == command.User.Email && x.Id != user.Id, cancellationToken);

                    if (emailInUse)
                        throw new ValidationException("EmailAlreadyUsed", command.User.Email);
                }

                user.FirstName = command.User.FirstName;
                user.LastName = command.User.LastName;
                user.Email = command.User.Email;
                user.IsDeactivated = command.User.IsDisabled;

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<Models.V1.User.User>(user);
                return mapped;
            }
        }
    }
}
