using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Bruker
{
    public class OpprettKoordinator
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
                var institusjon = await _context.Institution.FirstOrDefaultAsync(i => i.Id == command.Bruker.InstitutionId);
                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Bruker.InstitutionId);
                }

                if (!BrukerValidator.HarNavnOgHprNummerEllerGyldigPseudonym(command.Bruker))
                {
                    throw new ArgumentException("Koordinator må ha fornavn, etternavn og enten HPR-nummer eller pseudonym");
                }
                
                var koordinator = new Koordinator()
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


                // Coordinator skal også være observatør for samme institusjon
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

                _context.User.Add(koordinator);
                await _context.SaveChangesAsync();
                return _mapper.Map<Modeller.V1.User.User>(koordinator);
            }
        }
    }
}
