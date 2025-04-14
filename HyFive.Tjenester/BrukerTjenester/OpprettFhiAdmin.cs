
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Models.V1.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.Bruker;
using Bruker = HyFive.Models.V1.User.User;

namespace HyFive.Services.BrukerTjenester
{
    public class OpprettFhiAdmin
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
                    throw new Exception("Mangler pseudonym.");
                }
                if (!BrukerValidator.ErGyldigIdentPseudonym(command.Request.IdentityPseudonym))
                {
                    throw new Exception("Pseudonym er ikke gyldig.");
                }

                var eksisterendePseudonym = await _context.User.OfType<FhiAdmin>().AnyAsync(x => x.IdentPseudonym == command.Request.IdentityPseudonym);
                if (eksisterendePseudonym)
                    throw new Exception("Bruker kan ikke opprettes. Pseudonymet er allerede i bruk.");

                var fhiAdmin = new FhiAdmin()
                {
                    IdentPseudonym = command.Request.IdentityPseudonym,
                    Fornavn = command.Request.FirstName,
                    Etternavn = command.Request.Surname,
                    ErDeaktivert = false,
                    Opprettettidspunkt = DateTime.Now,
                };

                _context.User.Add(fhiAdmin);
                await _context.SaveChangesAsync();

                return _mapper.Map<Models.V1.User.User>(fhiAdmin);
            }

            
        }
    }
}
