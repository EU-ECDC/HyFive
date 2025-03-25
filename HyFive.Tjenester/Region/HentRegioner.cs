using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Region
{
    public class HentRegioner
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Institusjon.Region>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Modeller.V1.Institusjon.Region>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Modeller.V1.Institusjon.Region>> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionstyper = await _context.Region
                    .AsNoTracking()
                    .OrderBy(rt => rt.Id)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<Modeller.V1.Institusjon.Region>>(regionstyper);
                return mapped;
            }
        }
    }
}
