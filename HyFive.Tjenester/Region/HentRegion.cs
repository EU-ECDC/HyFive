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
    public class HentRegion
    {
        public class Query : IRequest<Models.V1.Institution.Region>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Institution.Region>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Institution.Region> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionstyper = await _context.Region
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rt => rt.Id == request.Id, cancellationToken);

                var mapped = _mapper.Map<Models.V1.Institution.Region>(regionstyper);
                return mapped;
            }
        }
    }
}
