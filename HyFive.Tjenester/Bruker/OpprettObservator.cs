using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Bruker
{
    public class OpprettObservator
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
                    throw new ArgumentException("Observatør må ha fornavn, etternavn og enten HPR-nummer eller pseudonym");
                }
                
                var institusjon = await _context.Institusjon.FirstOrDefaultAsync(i => i.Id == command.Bruker.InstitusjonId);
                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Bruker.InstitusjonId);
                }

                var observator = new Observator()
                {
                    Fornavn = command.Bruker.Fornavn,
                    Etternavn = command.Bruker.Etternavn,
                    Epost = command.Bruker.Epost,
                    Institusjon = institusjon,
                    HPRNummer = command.Bruker.HPRNummer,
                    IdentPseudonym = command.Bruker.IdentPseudonym,
                    Opprettettidspunkt = DateTime.UtcNow,
                    ErDeaktivert = false
                };
                _context.Bruker.Add(observator);
                await _context.SaveChangesAsync();
                return _mapper.Map<Modeller.V1.Bruker.Bruker>(observator);
            }
        }
    }
}
