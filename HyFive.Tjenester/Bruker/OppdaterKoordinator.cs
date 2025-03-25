using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Bruker
{
    public class OppdaterKoordinator
    {
        public class Command : IRequest<Modeller.V1.Bruker.Bruker>
        {
            public Modeller.V1.Bruker.Bruker Bruker { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Bruker.Bruker>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Bruker.Bruker> Handle(Command command, CancellationToken cancellationToken)
            {
                if (!BrukerValidator.HarNavnOgHprNummerEllerGyldigPseudonym(command.Bruker))
                {
                    throw new ArgumentException("Koordinator må ha fornavn, etternavn og enten HPR-nummer eller pseudonym");
                }
                var bruker = await _context.Bruker.OfType<Koordinator>().FirstOrDefaultAsync(i => i.Id == command.Bruker.Id);
                bruker.Fornavn = command.Bruker.Fornavn;
                bruker.Etternavn = command.Bruker.Etternavn;
                bruker.Epost = command.Bruker.Epost;
                bruker.HPRNummer = command.Bruker.HPRNummer;
                bruker.IdentPseudonym = command.Bruker.IdentPseudonym;
                bruker.ErDeaktivert = command.Bruker.ErDeaktivert;
                _context.Bruker.Update(bruker);

                await _context.SaveChangesAsync();
                var mapped = _mapper.Map<Modeller.V1.Bruker.Bruker>(bruker);
                return mapped;
            }
        }
    }
}
