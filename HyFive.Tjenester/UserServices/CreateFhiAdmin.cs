
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.User;
using Bruker = HyFive.Models.V1.User.User;

namespace HyFive.Services.UserServices
{
    public class CreateFhiAdmin
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public CreateFhiAdminRequest Request { get; set; }
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
                if (string.IsNullOrWhiteSpace(command.Request.IdentityPseudonym))
                {
                    throw new Exception("Missing pseudonym.");
                }
                if (!UserValidator.IsValidIdentityPseudonym(command.Request.IdentityPseudonym))
                {
                    throw new Exception("Pseudonym is not valid.");
                }

                var existingPseudonym = await _context.User.OfType<FhiAdmin>().AnyAsync(x => x.IdentityPseudonym == command.Request.IdentityPseudonym);
                if (existingPseudonym)
                    throw new Exception("User cannot be created. The pseudonym is already in use.");

                var fhiAdmin = new FhiAdmin()
                {
                    IdentityPseudonym = command.Request.IdentityPseudonym,
                    FirstName = command.Request.FirstName,
                    LastName = command.Request.LastName,
                    IsDeactivated = false,
                    CreatedTime = DateTime.UtcNow,
                };

                _context.User.Add(fhiAdmin);
                await _context.SaveChangesAsync();

                return _mapper.Map<Models.V1.User.User>(fhiAdmin);
            }

            
        }
    }
}
