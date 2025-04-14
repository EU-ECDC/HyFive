using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Bruker
{
    public class OppdaterKoordinator
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public Models.V1.User.User Bruker { get; set; }
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
                if (!BrukerValidator.HarNavnOgHprNummerEllerGyldigPseudonym(command.Bruker))
                {
                    throw new ArgumentException("Koordinator må ha fornavn, etternavn og enten HPR-nummer eller pseudonym");
                }
                var bruker = await _context.User.OfType<Koordinator>().FirstOrDefaultAsync(i => i.Id == command.Bruker.Id);
                bruker.Fornavn = command.Bruker.FirstName;
                bruker.Etternavn = command.Bruker.Surname;
                bruker.Epost = command.Bruker.Email;
                bruker.HPRNummer = command.Bruker.HPRNumber;
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
