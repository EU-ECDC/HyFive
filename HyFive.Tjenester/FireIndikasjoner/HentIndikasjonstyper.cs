using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class HentIndikasjonstyper
    {
        public class Query : IRequest<List<IndikasjonType>>
        {
        }

        public class Handler : IRequestHandler<Query, List<IndikasjonType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<IndikasjonType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var indikasjoner = await _context.Indikasjon
                    .AsNoTracking()
                    .ProjectTo<IndikasjonType>(_mapper.ConfigurationProvider)
                    .OrderBy(i => i.Nummer)
                    .ToListAsync(cancellationToken);
                return indikasjoner;
            }
        }
    }
}