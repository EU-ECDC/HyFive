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
    public class HentHanskeUtenIndikasjonTyper
    {
        public class Query : IRequest<IEnumerable<HanskeUtenIndikasjonType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<HanskeUtenIndikasjonType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HanskeUtenIndikasjonType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var hanskeUtenIndikasjonTyper = await _context.HanskeUtenIndikasjonType
                    .AsNoTracking()
                    .ProjectTo<HanskeUtenIndikasjonType>(_mapper.ConfigurationProvider)
                    .OrderBy(h => h.Navn)
                    .ToListAsync(cancellationToken);
                return hanskeUtenIndikasjonTyper;
            }
        }
    }
}
