using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.RegionalHealthOrganization
{
    public class GetAllRegionalHealthOrganization
    {
        public class Query : IRequest<List<Models.V1.Institution.RegionalInstitution>>
        {

        }

        public class Handler : IRequestHandler<Query, List<Models.V1.Institution.RegionalInstitution>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public Task<List<Models.V1.Institution.RegionalInstitution>> Handle(Query request, CancellationToken cancellationToken)
            {
                var regionaltHealthcareOrganization = _context.RegionaltHealthcareOrganization
                                                    .AsNoTracking()
                                                    .ProjectTo<Models.V1.Institution.RegionalInstitution>(_mapper.ConfigurationProvider)
                                                    .ToListAsync(cancellationToken);

                return regionaltHealthcareOrganization;
            }
        }
    }
}
