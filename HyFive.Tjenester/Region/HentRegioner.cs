using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Region
{
    public class HentRegioner
    {
        public class Query : IRequest<IEnumerable<Models.V1.Institution.Region>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.Institution.Region>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.Institution.Region>> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionstyper = await _context.Region
                    .AsNoTracking()
                    .OrderBy(rt => rt.Id)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<Models.V1.Institution.Region>>(regionstyper);
                return mapped;
            }
        }
    }
}
