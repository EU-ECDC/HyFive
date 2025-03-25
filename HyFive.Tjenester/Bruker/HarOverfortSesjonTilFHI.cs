using HyFive.Dataaksess;
using HyFive.Modeller.V1.Konstanter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Bruker
{
    public class HarOverfortSesjonTilFHI
    {
        public class Command : IRequest<bool>
        {
            public int ObervasjonsId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var sesjonerOverfortTilFHI = await _context.Sesjon.Where(s => s.Observator.Id == request.ObervasjonsId && s.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi).AnyAsync();

                return sesjonerOverfortTilFHI;
            }
        }
    }
}
