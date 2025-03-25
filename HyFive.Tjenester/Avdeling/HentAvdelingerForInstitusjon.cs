using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Avdeling
{
    public class HentAvdelingerForInstitusjon
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Institusjon.Avdeling>>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Modeller.V1.Institusjon.Avdeling>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Modeller.V1.Institusjon.Avdeling>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Avdeling
                    .Include(a => a.Roller)
                    .Include(a => a.Avdelingtype)
                    .AsNoTracking()
                    .Where(a => a.InstitusjonId == request.InstitusjonId)
                    .OrderBy(a => a.Navn)
                    .ProjectTo<Modeller.V1.Institusjon.Avdeling>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
