using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class HentAlleHelseforetak
    {
        public class Query : IRequest<List<Modeller.V1.Institusjon.Helseforetak>>
        {  }

        public class Handler : IRequestHandler<Query, List<Modeller.V1.Institusjon.Helseforetak>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Modeller.V1.Institusjon.Helseforetak>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Helseforetak.Any())
                {
                    var alleHelseforetak = await _context.Helseforetak
                                                         .AsNoTracking()
                                                         .OrderBy(h => h.Navn)
                                                         .ProjectTo<Modeller.V1.Institusjon.Helseforetak>(_mapper.ConfigurationProvider)
                                                         .ToListAsync();

                    return alleHelseforetak;
                }

                return null;
            }
        }
    }
}
