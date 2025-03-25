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
    public class HentBeskyttelsesutstyrTyper
    {
        public class Query : IRequest<IEnumerable<BeskyttelsesutstyrType>>
        {

        }

        public class Handler : IRequestHandler<Query, IEnumerable<BeskyttelsesutstyrType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<BeskyttelsesutstyrType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var beskyttelsesutstyrtyper = await _context.BeskyttelsesutstyrType
                    .Include(but => but.Feilbruktyper)
                    .AsNoTracking()
                    .ProjectTo<BeskyttelsesutstyrType>(_mapper.ConfigurationProvider)
                    .OrderBy(but => but.Navn)
                    .ToListAsync(cancellationToken);

                return beskyttelsesutstyrtyper;
            }
        }
    }
}
