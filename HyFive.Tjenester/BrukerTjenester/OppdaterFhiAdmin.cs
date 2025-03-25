using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Tjenester.Autentisering.Bruker;
using Fhi.HelseId.Web.Services;
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
    public class OppdaterFhiAdmin
    {
        public class Command : IRequest<Modeller.V1.Bruker.Bruker>
        {
            public Modeller.V1.Bruker.Bruker Bruker { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Bruker.Bruker>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ICurrentUser _currentUser;

            public Handler(HandhygieneContext context, IMapper mapper, ICurrentUser currentUser)
            {
                _context = context;
                _mapper = mapper;
                _currentUser = currentUser;
            }

            public async Task<Modeller.V1.Bruker.Bruker> Handle(Command command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(command.Bruker.IdentPseudonym))
                {
                    throw new Exception("Mangler pseudonym.");
                }
                if (!BrukerValidator.ErGyldigIdentPseudonym(command.Bruker.IdentPseudonym))
                {
                    throw new Exception("Pseudonym er ikke gyldig.");
                }

                var bruker = await _context.Bruker.OfType<FhiAdmin>().FirstOrDefaultAsync(i => i.Id == command.Bruker.Id);
                if (bruker == null)
                    throw new Exception($"Fant ikke bruker med Id {command.Bruker.Id}");
                if (_currentUser.PidPseudonym == bruker.IdentPseudonym)
                    throw new Exception($"Bruker kan ikke endre på seg selv.");
                if (bruker.IdentPseudonym != command.Bruker.IdentPseudonym)
                {
                    var eksisterendePseudonym = await _context.Bruker.OfType<FhiAdmin>().AnyAsync(x => x.IdentPseudonym == command.Bruker.IdentPseudonym);
                    if (eksisterendePseudonym)
                        throw new Exception("Bruker kan ikke oppdateres. Pseudonymet er allerede i bruk.");
                }

                bruker.Fornavn = command.Bruker.Fornavn;
                bruker.Etternavn = command.Bruker.Etternavn;
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
