using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Avdeling
{
    public class HentAvdelingTyper
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Institusjon.AvdelingType>>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<HentAvdelingTyper.Query, IEnumerable<Modeller.V1.Institusjon.AvdelingType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Modeller.V1.Institusjon.AvdelingType>> Handle(HentAvdelingTyper.Query request, CancellationToken cancellationToken)
            {
                return await _context.AvdelingType
                    .AsNoTracking()
                    .OrderBy(a => a.Navn)
                    .ProjectTo<Modeller.V1.Institusjon.AvdelingType>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
