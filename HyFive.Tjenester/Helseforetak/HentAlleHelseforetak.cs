using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Helseforetak
{
    public class HentAlleHelseforetak
    {
        public class Query : IRequest<List<Models.V1.Institution.HealthcareEnterprise>>
        {  }

        public class Handler : IRequestHandler<Query, List<Models.V1.Institution.HealthcareEnterprise>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Models.V1.Institution.HealthcareEnterprise>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.HealthcareProvider.Any())
                {
                    var alleHelseforetak = await _context.HealthcareProvider
                                                         .AsNoTracking()
                                                         .OrderBy(h => h.Name)
                                                         .ProjectTo<Models.V1.Institution.HealthcareEnterprise>(_mapper.ConfigurationProvider)
                                                         .ToListAsync();

                    return alleHelseforetak;
                }

                return null;
            }
        }
    }
}
