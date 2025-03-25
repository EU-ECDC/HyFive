using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.RegionaltHelseforetak
{
    public class HentAlleRegionaltHelseforetak
    {
        public class Query : IRequest<List<Modeller.V1.Institusjon.RegionaltHelseforetak>>
        {

        }

        public class Handler : IRequestHandler<Query, List<Modeller.V1.Institusjon.RegionaltHelseforetak>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public Task<List<Modeller.V1.Institusjon.RegionaltHelseforetak>> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionaltHelseforetak = _context.RegionaltHelseforetak
                                                    .AsNoTracking()
                                                    .ProjectTo<Modeller.V1.Institusjon.RegionaltHelseforetak>(_mapper.ConfigurationProvider)
                                                    .ToListAsync(cancellationToken);

                return regionaltHelseforetak;
            }
        }
    }
}
