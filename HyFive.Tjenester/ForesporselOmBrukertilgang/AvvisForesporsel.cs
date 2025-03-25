using HyFive.Dataaksess;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class AvvisForesporsel
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

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var foresporsel = await _context.ForesporselOmBrukertilgang.FindAsync(command.ForespørselId);

                if (foresporsel == null) return false;

                var bruker = _context.Bruker.FirstOrDefault(b => b.IdentPseudonym == command.IdentPseudonym
                                                    || b.HPRNummer == command.HPRNummer);

                if (bruker == null) return false;

                foresporsel.Status = Domene.Bruker.ForesporselOmBrukertilgangStatus.Avvist;
                foresporsel.BehandletTidspunkt = DateTime.Now;
                foresporsel.BehandletAvBrukerId = bruker.Id;
                foresporsel.BehandletAvBrukernavn = bruker.Fornavn + " " + bruker.Etternavn;

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }


    }
}
