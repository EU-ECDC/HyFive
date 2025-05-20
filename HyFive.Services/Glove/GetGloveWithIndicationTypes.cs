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
    public class GetGloveWithIndicationTypes
    {
        public class Query : IRequest<IEnumerable<GloveWithIndicationType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<GloveWithIndicationType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<GloveWithIndicationType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var gloveWithIndicationTypes = await _context.GloveWithIndicationType
                    .AsNoTracking()
                    .ProjectTo<GloveWithIndicationType>(_mapper.ConfigurationProvider)
                    .OrderBy(h => h.Name)
                    .ToListAsync(cancellationToken);
                return gloveWithIndicationTypes;
            }
        }
    }
}
