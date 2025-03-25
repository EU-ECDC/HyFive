using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Roller
{
    public class HentRollerForAvdeling
    {
        public class Query : IRequest<List<Modeller.V1.Observasjon.Rolle>>
        {
            public int AvdelingId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Modeller.V1.Observasjon.Rolle>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Modeller.V1.Observasjon.Rolle>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roller = await _context.Avdeling
                    .Where(a => a.Id == request.AvdelingId)
                    .SelectMany(a => a.Roller)
                    .ProjectTo<Modeller.V1.Observasjon.Rolle>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return roller;
            }
        }
    }
}
