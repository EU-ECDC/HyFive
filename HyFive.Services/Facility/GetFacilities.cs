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
        public class Query : IRequest<Models.V1.OrganisationUnit.FacilityReport[]>
        {
        }

        public class Handler : IRequestHandler<Query, Models.V1.OrganisationUnit.FacilityReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.OrganisationUnit.FacilityReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(ou => ou.ParentId == null)              // facility roots
                    .Include(ou => ou.Type)                        // replaces FacilityType
                    .ProjectTo<Models.V1.OrganisationUnit.FacilityReport>(_mapper.ConfigurationProvider)
                    .OrderBy(ou => ou.Name)
                    .ToArrayAsync(cancellationToken);
            }
        }
    }
}
