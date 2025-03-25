using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Kommune
{
    public class HentKommuner
    {
        public class Query : IRequest<List<Modeller.V1.Institusjon.Kommune>>
        { }

        public class Handler : IRequestHandler<Query, List<Modeller.V1.Institusjon.Kommune>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<Modeller.V1.Institusjon.Kommune>> Handle(Query request, CancellationToken cancellationToken)
            {
                var kommuneListe = await _context.Kommune
                                           .AsNoTracking()
                                           .ProjectTo<Modeller.V1.Institusjon.Kommune>(_mapper.ConfigurationProvider)
                                           .OrderBy(k => k.Navn)
                                           .ToListAsync();

                return kommuneListe;
            }
        }
    }
}
