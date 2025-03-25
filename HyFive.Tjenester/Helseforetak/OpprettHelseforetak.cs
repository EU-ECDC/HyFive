using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class OpprettHelseforetak
    {
        public class Command : IRequest<bool>
        {
            public OpprettHelseforetakRequest Helseforetak { get; set; }
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
                var helseforetak = _context.Helseforetak.Include(h => h.RegionaltHelseforetak)
                                                        .FirstOrDefault(h => h.Navn.ToLower() == request.Helseforetak.Navn.ToLower());

                if (helseforetak != null)
                    return false;

                helseforetak = new Domene.Sted.Helseforetak
                {
                    Navn = request.Helseforetak.Navn,
                    RegionaltHelseforetak = _context.RegionaltHelseforetak.Find(request.Helseforetak.RegionaltHelseforetakId)
                };

                _context.Helseforetak.Add(helseforetak);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
