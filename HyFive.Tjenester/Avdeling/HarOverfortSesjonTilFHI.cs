using HyFive.Dataaksess;
using HyFive.Modeller.V1.Konstanter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Avdeling
{
    public class HarOverfortSesjonTilFHI
    {
        public class Command : IRequest<bool>
        {
            public int AvdelingId { get; set; }
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
                var sesjonerOverfortTilFHI = await _context.Sesjon.Where(s => s.Avdeling.Id == request.AvdelingId && s.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi).AnyAsync();

                return sesjonerOverfortTilFHI;
            }
        }
    }

}
