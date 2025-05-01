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
    public class GetHandHygieneAfterGloveUseTypes
    {
        public class Query : IRequest<IEnumerable<PostGloveHandHygieneType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<PostGloveHandHygieneType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<PostGloveHandHygieneType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var handHygieneAfterGloveUseTypes = await _context.HandHygieneAfterGloveUseType
                    .AsNoTracking()
                    .ProjectTo<PostGloveHandHygieneType>(_mapper.ConfigurationProvider)
                    .OrderBy(h => h.Name)
                    .ToListAsync(cancellationToken);
                return handHygieneAfterGloveUseTypes;
            }
        }
    }
}
