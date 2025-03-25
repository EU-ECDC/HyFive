using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class HentInstitusjonstyper
    {
        public class Query : IRequest<IEnumerable<InstitusjonType>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<InstitusjonType>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<InstitusjonType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjonstyper = await _context.InstitusjonType.AsNoTracking()
                    .OrderBy(i => i.Id)
                    .ToListAsync(cancellationToken);
                var mapped = _mapper.Map<List<InstitusjonType>>(institusjonstyper);
                return mapped;
            }
        }
    }
}