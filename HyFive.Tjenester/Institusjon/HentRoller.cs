using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Institusjon
{
    public class HentRoller
    {
        public class Query : IRequest<IEnumerable<Models.V1.Observation.Role>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.Observation.Role>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<IEnumerable<Models.V1.Observation.Role>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Role
                    .AsNoTracking()
                    .OrderBy(a => a.Name)
                    .ProjectTo<Models.V1.Observation.Role>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
