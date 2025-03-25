using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class HentBeskyttelsesutstyrsettingTyper
    {
        public class Query : IRequest<IEnumerable<BeskyttelsesutstyrsettingType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<BeskyttelsesutstyrsettingType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<BeskyttelsesutstyrsettingType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var beskyttelsesutstyrsettingTyper = await _context.BeskyttelsesutstyrsettingType
                    .Include(b => b.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper)
                    .ThenInclude(b => b.BeskyttelsesutstyrType)
                    .ThenInclude(bt => bt.Feilbruktyper)
                    .AsNoTracking()
                    .ProjectTo<BeskyttelsesutstyrsettingType>(_mapper.ConfigurationProvider)
                    .OrderBy(b => b.Navn)
                    .ToListAsync(cancellationToken);

                return beskyttelsesutstyrsettingTyper;
            }
        }
    }
}
