using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;

namespace HyFive.Services.ForesporselOmBrukertilgang
{
    public class OpprettBrukerFraForesporsel
    {
        public class Command : IRequest<bool>
        {
            public int ForespørselId { get; set; }
            public string IdentPseudonym { get; set; }
            public string HPRNummer { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
            }


            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var foresporsel = await _context.UserAccessRequest.FindAsync(command.ForespørselId);
                
                if (foresporsel == null) return false;

                var institusjon = await HentInstitusjon(foresporsel);

                if (institusjon == null) return false;

                if (!FinnesObservatørForInstitusjon(foresporsel.IdentPseudonym, institusjon.Id))
                    LagObservatør(foresporsel, institusjon);

                var bruker = _context.User.FirstOrDefault(b => b.IdentityPseudonym == command.IdentPseudonym 
                                                                    || b.HPRNumber == command.HPRNummer);

                if(bruker == null) return false;

                foresporsel.Status = ForesporselOmBrukertilgangStatus.Godkjent;
                foresporsel.BehandletTidspunkt = DateTime.Now;
                foresporsel.BehandletAvBrukerId = bruker.Id;
                foresporsel.BehandletAvBrukernavn = bruker.Fornavn + " " + bruker.Etternavn;

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }

            private bool FinnesObservatørForInstitusjon(string foresporselIdentPseudonym, int institusjonId)
            {
                return _context.User.OfType<Observator>().Any(x => x.IdentPseudonym == foresporselIdentPseudonym &&
                                                                     x.Institusjon.Id == institusjonId);
            }

            private void LagObservatør(Domene.Bruker.ForesporselOmBrukertilgang foresporsel, Domene.Place.Institution institusjon)
            {
                var observator = new Observator()
                {
                    Fornavn = foresporsel.BrukerFornavn,
                    Etternavn = foresporsel.BrukerEtternavn,
                    Institusjon = institusjon,
                    HPRNummer = foresporsel.HPRNummer,
                    IdentPseudonym = foresporsel.IdentPseudonym,
                    Opprettettidspunkt = DateTime.Now,
                    ErDeaktivert = false,
                };
                _context.User.Add(observator);
            }

            private async Task<Domene.Place.Institution> HentInstitusjon(Domene.Bruker.ForesporselOmBrukertilgang foresporsel)
            {
                Domene.Place.Institution institusjon = null;
                if (foresporsel.InstitusjonId != null)
                {
                    institusjon = await _context.Institution.FindAsync(foresporsel.InstitusjonId);
                }
                
                return institusjon;
            }
        }
    }
}
