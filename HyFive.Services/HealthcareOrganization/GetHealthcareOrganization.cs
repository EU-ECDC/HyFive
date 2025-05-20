using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class GetHealthcareOrganization
    {
        public class Query : IRequest<InstitutionReport[]>
        {
            public int HealthcareOrganizationId { get; set; }
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
                var query = _context.Institution.Where(x=>x.HealthcareOrganization.Id == request.HealthcareOrganizationId);

                var result = await query
                    .ProjectTo<InstitutionReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
            }
        }
    }
}
