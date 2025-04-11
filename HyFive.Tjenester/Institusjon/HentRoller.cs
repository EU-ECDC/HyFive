using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Institusjon
{
    public class HentRoller
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Observasjon.Role>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Modeller.V1.Observasjon.Role>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<IEnumerable<Modeller.V1.Observasjon.Role>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Role
                    .AsNoTracking()
                    .OrderBy(a => a.Name)
                    .ProjectTo<Modeller.V1.Observasjon.Role>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
