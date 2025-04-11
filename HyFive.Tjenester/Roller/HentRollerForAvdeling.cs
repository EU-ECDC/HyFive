using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Roller
{
    public class HentRollerForAvdeling
    {
        public class Query : IRequest<List<Modeller.V1.Observasjon.Role>>
        {
            public int AvdelingId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Modeller.V1.Observasjon.Role>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Modeller.V1.Observasjon.Role>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roller = await _context.Department
                    .Where(a => a.Id == request.AvdelingId)
                    .SelectMany(a => a.Roller)
                    .ProjectTo<Modeller.V1.Observasjon.Role>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return roller;
            }
        }
    }
}
