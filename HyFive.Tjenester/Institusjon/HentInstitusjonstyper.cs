using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class HentInstitusjonstyper
    {
        public class Query : IRequest<IEnumerable<InstitutionType>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<InstitutionType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<InstitutionType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjonstyper = await _context.InstitutionType.AsNoTracking()
                    .OrderBy(i => i.Id)
                    .ToListAsync(cancellationToken);
                var mapped = _mapper.Map<List<InstitutionType>>(institusjonstyper);
                return mapped;
            }
        }
    }
}