using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class GetDepartmentsForFacility
    {
        public class Query : IRequest<IEnumerable<Models.V1.Facility.Department>>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.Facility.Department>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.Facility.Department>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Department
                    .Include(a => a.Roles)
                    .Include(a => a.DepartmentType)
                    .AsNoTracking()
                    .Where(a => a.FacilityId == request.FacilityId)
                    .OrderBy(a => a.Name)
                    .ProjectTo<Models.V1.Facility.Department>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
