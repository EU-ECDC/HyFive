using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
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
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
            }


            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var foresporsel = await _context.ForesporselOmBrukertilgang.FindAsync(command.ForespørselId);
                
                if (foresporsel == null) return false;

                var institusjon = await HentInstitusjon(foresporsel);

                if (institusjon == null) return false;

                if (!FinnesObservatørForInstitusjon(foresporsel.IdentPseudonym, institusjon.Id))
                    LagObservatør(foresporsel, institusjon);

                var bruker = _context.Bruker.FirstOrDefault(b => b.IdentPseudonym == command.IdentPseudonym 
                                                                    || b.HPRNummer == command.HPRNummer);

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
                return _context.Bruker.OfType<Observator>().Any(x => x.IdentPseudonym == foresporselIdentPseudonym &&
                                                                     x.Institusjon.Id == institusjonId);
            }

            private void LagObservatør(Domene.Bruker.ForesporselOmBrukertilgang foresporsel, Domene.Sted.Institusjon institusjon)
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
                _context.Bruker.Add(observator);
            }

            private async Task<Domene.Sted.Institusjon> HentInstitusjon(Domene.Bruker.ForesporselOmBrukertilgang foresporsel)
            {
                Domene.Sted.Institusjon institusjon = null;
                if (foresporsel.InstitusjonId != null)
                {
                    institusjon = await _context.Institusjon.FindAsync(foresporsel.InstitusjonId);
                }
                
                return institusjon;
            }
        }
    }
}
