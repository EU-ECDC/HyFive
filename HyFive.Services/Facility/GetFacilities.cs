using System.Linq;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace HyFive.Services.Facility
{
    public class GetFacilities
    {
        public class Query : IRequest<Models.V1.Facility.FacilityReport[]>
        {
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.FacilityReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Facility.FacilityReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Facility
                    .AsNoTracking()
                    .Include(i => i.FacilityType)
                    .ProjectTo<Models.V1.Facility.FacilityReport>(_mapper.ConfigurationProvider)
                    .OrderBy(i => i.Name)
                    .ToArrayAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
