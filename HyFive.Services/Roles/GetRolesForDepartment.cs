using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Roles
{
    public class GetRolesForDepartment
    {
        public class Query : IRequest<List<Models.V1.Observation.Role>>
        {
            public int OrganisationUnitId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.Observation.Role>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Models.V1.Observation.Role>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roles = await _context.OrganisationUnitRole
                .AsNoTracking()
                .Where(link => link.OrganisationUnitId == request.OrganisationUnitId)
                .Select(link => link.Role)
                .OrderBy(r => r.Name)
                .ProjectTo<Models.V1.Observation.Role>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

                return roles;
            }
        }
    }
}
