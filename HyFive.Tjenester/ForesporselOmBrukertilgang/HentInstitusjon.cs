using HyFive.Dataaksess;
using HyFive.Modeller.V1.ForesporselOmBrukertilgang;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class HentInstitusjon
    {
        public class Query : IRequest<InstitusjonForForesporselOmBrukertilgang>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, InstitusjonForForesporselOmBrukertilgang>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }
            public async Task<InstitusjonForForesporselOmBrukertilgang> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjon = _context.Institusjon.AsNoTracking()
                    .FirstOrDefault(i => i.Id == request.InstitusjonId);

                var institusjonForForesporselOmBrukertilgang = new InstitusjonForForesporselOmBrukertilgang
                {
                    Id = institusjon.Id,
                    Navn = institusjon.Navn
                };

                return institusjonForForesporselOmBrukertilgang;
            }
        }
    }
}