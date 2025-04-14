using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Services.Authentication.User;
using Fhi.HelseId.Web.Services;
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
    public class OppdaterFhiAdmin
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public Models.V1.User.User Bruker { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.User.User>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ICurrentUser _currentUser;

            public Handler(HandHygieneContext context, IMapper mapper, ICurrentUser currentUser)
            {
                _context = context;
                _mapper = mapper;
                _currentUser = currentUser;
            }

            public async Task<Models.V1.User.User> Handle(Command command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(command.Bruker.IdentityPseudonym))
                {
                    throw new Exception("Mangler pseudonym.");
                }
                if (!BrukerValidator.ErGyldigIdentPseudonym(command.Bruker.IdentityPseudonym))
                {
                    throw new Exception("Pseudonym er ikke gyldig.");
                }

                var bruker = await _context.User.OfType<FhiAdmin>().FirstOrDefaultAsync(i => i.Id == command.Bruker.Id);
                if (bruker == null)
                    throw new Exception($"Fant ikke bruker med Id {command.Bruker.Id}");
                if (_currentUser.PidPseudonym == bruker.IdentPseudonym)
                    throw new Exception($"Bruker kan ikke endre på seg selv.");
                if (bruker.IdentPseudonym != command.Bruker.IdentityPseudonym)
                {
                    var eksisterendePseudonym = await _context.User.OfType<FhiAdmin>().AnyAsync(x => x.IdentPseudonym == command.Bruker.IdentityPseudonym);
                    if (eksisterendePseudonym)
                        throw new Exception("Bruker kan ikke oppdateres. Pseudonymet er allerede i bruk.");
                }

                bruker.Fornavn = command.Bruker.FirstName;
                bruker.Etternavn = command.Bruker.Surname;
                bruker.IdentPseudonym = command.Bruker.IdentityPseudonym;
                bruker.ErDeaktivert = command.Bruker.IsDisabled;

                _context.User.Update(bruker);
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<Models.V1.User.User>(bruker);
                return mapped;
            }
        }
    }
}
