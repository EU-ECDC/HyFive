using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Bruker
{
    public class OppdaterObservator
    {
        public class Command : IRequest<Modeller.V1.User.User>
        {
            public Modeller.V1.User.User Bruker { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.User.User>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.User.User> Handle(Command command, CancellationToken cancellationToken)
            {
                if (!BrukerValidator.HarNavnOgHprNummerEllerGyldigPseudonym(command.Bruker))
                {
                    throw new ArgumentException("Observatør må ha fornavn, etternavn og enten HPR-nummer eller pseudonym");
                }
                
                var bruker = await _context.User.OfType<Observator>().FirstOrDefaultAsync(i => i.Id == command.Bruker.Id);
                bruker.Fornavn = command.Bruker.FirstName;
                bruker.Etternavn = command.Bruker.Surname;
                bruker.Epost = command.Bruker.Email;
                bruker.HPRNummer = command.Bruker.HPRNumber;
                bruker.IdentPseudonym = command.Bruker.IdentityPseudonym;
                bruker.ErDeaktivert = command.Bruker.IsDisabled;
                _context.User.Update(bruker);
                
                await _context.SaveChangesAsync();
                var mapped = _mapper.Map<Modeller.V1.User.User>(bruker);
                return mapped;
            }
        }
    }
}
