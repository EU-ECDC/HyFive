using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class HentInstitiusjonerForHelseforetak
    {
        public class Query : IRequest<InstitutionReport[]>
        {
            public int HelseforetakId { get; set; }
        }

        public class Handler : IRequestHandler<Query, InstitutionReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<InstitutionReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Institution.Where(x=>x.HealthcareProvider.Id == request.HelseforetakId);

                var result = await query
                    .ProjectTo<InstitutionReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
            }
        }
    }
}
