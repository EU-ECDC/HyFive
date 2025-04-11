using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Gloves;
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
        public class Query : IRequest<IEnumerable<GeneralPurposeGloveType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<GeneralPurposeGloveType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<GeneralPurposeGloveType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var hanskeUtenIndikasjonTyper = await _context.GeneralPurposeGloveType
                    .AsNoTracking()
                    .ProjectTo<GeneralPurposeGloveType>(_mapper.ConfigurationProvider)
                    .OrderBy(h => h.Name)
                    .ToListAsync(cancellationToken);
                return hanskeUtenIndikasjonTyper;
            }
        }
    }
}
