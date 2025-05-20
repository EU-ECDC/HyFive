using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.Gloves;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using System.Linq;

namespace HyFive.Services.Glove
{
    public class GetGloveWithoutIndicationTypes
    {
        public class Query : IRequest<IEnumerable<GloveWithoutIndicationType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<GloveWithoutIndicationType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<GloveWithoutIndicationType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var gloveWithoutIndicationTypes = await _context.GloveWithoutIndicationType
                    .AsNoTracking()
                    .ProjectTo<GloveWithoutIndicationType>(_mapper.ConfigurationProvider)
                    .OrderBy(h => h.Name)
                    .ToListAsync(cancellationToken);
                return gloveWithoutIndicationTypes;
            }
        }
    }
}
