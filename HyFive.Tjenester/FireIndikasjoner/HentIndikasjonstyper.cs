using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
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
        public class Query : IRequest<List<IndicationType>>
        {
        }

        public class Handler : IRequestHandler<Query, List<IndicationType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<IndicationType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var indikasjoner = await _context.IndicationTypes
                    .AsNoTracking()
                    .ProjectTo<IndicationType>(_mapper.ConfigurationProvider)
                    .OrderBy(i => i.Number)
                    .ToListAsync(cancellationToken);
                return indikasjoner;
            }
        }
    }
}