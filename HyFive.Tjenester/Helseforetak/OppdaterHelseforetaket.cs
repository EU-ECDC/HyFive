using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class OppdaterHelseforetaket
    {
        public class Command : IRequest<bool>
        {
            public Modeller.V1.Institusjon.Helseforetak Helseforetak { get; set; }
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
                var helseforetak = _context.Helseforetak.Include(h => h.RegionaltHelseforetak)
                                                        .FirstOrDefault(h => h.Id == command.Helseforetak.Id);

                helseforetak.Navn = command.Helseforetak.Navn;
                helseforetak.RegionaltHelseforetak = _context.RegionaltHelseforetak.Find(command.Helseforetak.RegionaltHelseforetakId);

                _context.Helseforetak.Update(helseforetak);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
