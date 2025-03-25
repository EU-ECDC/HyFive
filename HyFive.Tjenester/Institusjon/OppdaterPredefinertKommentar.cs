using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Domene.Sted;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PredefinertKommentar = HyFive.Modeller.V1.Institusjon.PredefinertKommentar;

namespace HyFive.Tjenester.Institusjon
{
    public class OppdaterPredefinertKommentar
    {
        public class Command : IRequest<bool>
        {
            public PredefinertKommentar PredefinertKommentar { get; set; }
            public int Institusjonid { get; set; }
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
                var kommentar = await _context.PredefinertKommentar.FirstOrDefaultAsync(pk => pk.Id == request.PredefinertKommentar.Id 
                                                                       && pk.InstitusjonId == request.Institusjonid 
                                                                       && pk.SesjonType == SesjonType.Beskyttelsesutstyr, cancellationToken);
                if (kommentar == null)
                    return false;

                kommentar.Kommentar = request.PredefinertKommentar.Kommentar;

                _context.Update(kommentar);
                _context.SaveChanges();

                return true;
            }
        }
    }
}
