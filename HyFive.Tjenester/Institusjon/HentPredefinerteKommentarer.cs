using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Sesjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class HentPredefinerteKommentarer
    {
        public class Query : IRequest<IEnumerable<string>>
        {
            public int InstitusjonId { get; set; }
            public SesjonType Sesjontype { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<string>>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.Sesjontype == SesjonType.Beskyttelsesutstyr)
                {
                    var predefinerteKommentarer = await _context.PredefinertKommentar
                        .Where(pk =>
                            pk.InstitusjonId == request.InstitusjonId &&
                            pk.SesjonType == Domene.Sted.SesjonType.Beskyttelsesutstyr)
                        .Select(pk => pk.Kommentar)
                        .ToListAsync(cancellationToken);

                    return predefinerteKommentarer;
                }

                return null;
            }
        }
    }
}
