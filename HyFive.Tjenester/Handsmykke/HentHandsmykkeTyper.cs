using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Handsmykke
{
    public class HentHandsmykkeTyper
    {
        public class Query : IRequest<IEnumerable<HandsmykkeType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<HandsmykkeType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HandsmykkeType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var handsmykkeTyper = await _context.HandsmykkeType.AsNoTracking()
                    .Where(x => x.ErAktiv)
                    .OrderBy(x => x.Rekkefolge)
                    .ProjectTo<HandsmykkeType>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                return handsmykkeTyper;
            }
        }
    }
}
