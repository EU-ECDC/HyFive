using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class GetAllHealthcareOrganizations
    {
        public class Query : IRequest<List<Models.V1.Institution.HealthcareOrganization>>
        {  }

        public class Handler : IRequestHandler<Query, List<Models.V1.Institution.HealthcareOrganization>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Models.V1.Institution.HealthcareOrganization>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.HealthcareOrganization.Any())
                {
                    var allHealthcareOrganizations = await _context.HealthcareOrganization
                                                         .AsNoTracking()
                                                         .OrderBy(h => h.Name)
                                                         .ProjectTo<Models.V1.Institution.HealthcareOrganization>(_mapper.ConfigurationProvider)
                                                         .ToListAsync();

                    return allHealthcareOrganizations;
                }

                return null;
            }
        }
    }
}
