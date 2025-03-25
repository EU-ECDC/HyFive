using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class HentInstitusjoner
    {
        public class Query : IRequest<List<Modeller.V1.ForesporselOmBrukertilgang.InstitusjonForForesporselOmBrukertilgang>>
        {
        }

        public class Handler : IRequestHandler<Query, List<Modeller.V1.ForesporselOmBrukertilgang.InstitusjonForForesporselOmBrukertilgang>>
        {
            private readonly int InstitusjonIdForFHI = 1;

            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }
            public async Task<List<Modeller.V1.ForesporselOmBrukertilgang.InstitusjonForForesporselOmBrukertilgang>> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjoner = _context.Institusjon.AsNoTracking()
                             .Where(i=> i.Id != InstitusjonIdForFHI)
                             .Select(i => new Modeller.V1.ForesporselOmBrukertilgang.InstitusjonForForesporselOmBrukertilgang()
                             {
                                 Id = i.Id, 
                                 Navn = i.Navn
                             })
                             .OrderBy(i=>i.Navn)
                             .ToList();

                return institusjoner;
            }
        }
    }
}
