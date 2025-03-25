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

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class HentAktivitetTyper
    {
        public class Query : IRequest<IEnumerable<AktivitetType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<AktivitetType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<AktivitetType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var aktivitetTyper = await _context.AktivitetType
                    .AsNoTracking()
                    .ProjectTo<AktivitetType>(_mapper.ConfigurationProvider)
                    .OrderBy(a => a.Navn)
                    .ToListAsync(cancellationToken);
                
                return aktivitetTyper;
            }
        }
    }
}
