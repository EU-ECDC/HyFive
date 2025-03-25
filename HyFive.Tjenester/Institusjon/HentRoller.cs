using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
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
        public class Query : IRequest<IEnumerable<Modeller.V1.Observasjon.Rolle>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Modeller.V1.Observasjon.Rolle>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<IEnumerable<Modeller.V1.Observasjon.Rolle>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Rolle
                    .AsNoTracking()
                    .OrderBy(a => a.Navn)
                    .ProjectTo<Modeller.V1.Observasjon.Rolle>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
