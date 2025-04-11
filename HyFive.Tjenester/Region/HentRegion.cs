using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Region
{
    public class HentRegion
    {
        public class Query : IRequest<Modeller.V1.Institution.Region>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institution.Region>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institution.Region> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionstyper = await _context.Region
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rt => rt.Id == request.Id, cancellationToken);

                var mapped = _mapper.Map<Modeller.V1.Institution.Region>(regionstyper);
                return mapped;
            }
        }
    }
}
