using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Klinikk
{
    public class HentKlinikkerForInstitusjon
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Institusjon.Klinikk>>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Modeller.V1.Institusjon.Klinikk>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Modeller.V1.Institusjon.Klinikk>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Klinikk
                    .AsNoTracking()
                    .Include(k => k.Institusjon)
                    .Include(k => k.Avdelinger)
                    .Where(k => k.Institusjon.Id == request.InstitusjonId)
                    .OrderBy(k => k.Navn)
                    .ProjectTo<Modeller.V1.Institusjon.Klinikk>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
