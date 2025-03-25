using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Region
{
    public class HentRegion
    {
        public class Query : IRequest<Modeller.V1.Institusjon.Region>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institusjon.Region>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institusjon.Region> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionstyper = await _context.Region
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rt => rt.Id == request.Id, cancellationToken);

                var mapped = _mapper.Map<Modeller.V1.Institusjon.Region>(regionstyper);
                return mapped;
            }
        }
    }
}
