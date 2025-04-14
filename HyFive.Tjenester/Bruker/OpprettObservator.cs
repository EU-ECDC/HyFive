using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Bruker
{
    public class OpprettObservator
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
                    throw new ArgumentException("Observatør må ha fornavn, etternavn og enten HPR-nummer eller pseudonym");
                }
                
                var institusjon = await _context.Institution.FirstOrDefaultAsync(i => i.Id == command.Bruker.InstitutionId);
                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Bruker.InstitutionId);
                }

                var observator = new Observator()
                {
                    Fornavn = command.Bruker.FirstName,
                    Etternavn = command.Bruker.Surname,
                    Epost = command.Bruker.Email,
                    Institusjon = institusjon,
                    HPRNummer = command.Bruker.HPRNumber,
                    IdentPseudonym = command.Bruker.IdentityPseudonym,
                    Opprettettidspunkt = DateTime.Now,
                    ErDeaktivert = false
                };
                _context.User.Add(observator);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.User.User>(observator);
            }
        }
    }
}
