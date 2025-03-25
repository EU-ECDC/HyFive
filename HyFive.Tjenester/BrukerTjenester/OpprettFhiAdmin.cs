
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Tjenester.Bruker;
using Bruker = HyFive.Modeller.V1.Bruker.Bruker;

namespace HyFive.Tjenester.BrukerTjenester
{
    public class OpprettFhiAdmin
    {
        public class Command : IRequest<Modeller.V1.Bruker.Bruker>
        {
            public OpprettFhiAdminRequest Request { get; set; }
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
                if (string.IsNullOrWhiteSpace(command.Request.IdentPseudonym))
                {
                    throw new Exception("Mangler pseudonym.");
                }
                if (!BrukerValidator.ErGyldigIdentPseudonym(command.Request.IdentPseudonym))
                {
                    throw new Exception("Pseudonym er ikke gyldig.");
                }

                var eksisterendePseudonym = await _context.Bruker.OfType<FhiAdmin>().AnyAsync(x => x.IdentPseudonym == command.Request.IdentPseudonym);
                if (eksisterendePseudonym)
                    throw new Exception("Bruker kan ikke opprettes. Pseudonymet er allerede i bruk.");

                var fhiAdmin = new FhiAdmin()
                {
                    IdentPseudonym = command.Request.IdentPseudonym,
                    Fornavn = command.Request.Fornavn,
                    Etternavn = command.Request.Etternavn,
                    ErDeaktivert = false,
                    Opprettettidspunkt = DateTime.Now,
                };

                _context.Bruker.Add(fhiAdmin);
                await _context.SaveChangesAsync();

                return _mapper.Map<Modeller.V1.Bruker.Bruker>(fhiAdmin);
            }

            
        }
    }
}
