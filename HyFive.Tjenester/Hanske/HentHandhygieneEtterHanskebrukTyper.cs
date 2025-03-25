using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon.Hansker;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using System.Linq;

namespace HyFive.Tjenester.Hanske
{
    public class HentHandhygieneEtterHanskebrukTyper
    {
        public class Query : IRequest<IEnumerable<HandhygieneEtterHanskebrukType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<HandhygieneEtterHanskebrukType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HandhygieneEtterHanskebrukType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var handhygieneEtterHanskebrukTyper = await _context.HandhygieneEtterHanskebrukType
                    .AsNoTracking()
                    .ProjectTo<HandhygieneEtterHanskebrukType>(_mapper.ConfigurationProvider)
                    .OrderBy(h => h.Navn)
                    .ToListAsync(cancellationToken);
                return handhygieneEtterHanskebrukTyper;
            }
        }
    }
}
