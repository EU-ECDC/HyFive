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
        public class Query : IRequest<IEnumerable<Models.V1.OrganisationUnit.OrganisationUnit>>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.OrganisationUnit.OrganisationUnit>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.OrganisationUnit.OrganisationUnit>> Handle(Query request, CancellationToken cancellationToken)
            {
                // 1) Load department org units under facility
                var departments = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(o => o.LevelRef)
                    .Include(o => o.Type)
                    .Include(o => o.Address)
                    .Where(o => o.ParentId == request.FacilityId
                                && o.LevelRef.Level == "Department")
                    .OrderBy(o => o.Name)
                    .ToListAsync(cancellationToken);

                if (departments.Count == 0)
                    return new List<Models.V1.OrganisationUnit.OrganisationUnit>();

                var deptIds = departments.Select(d => d.Id).ToList();

                // 2) Load roles for these departments via join table
                var roleRows = await
                    (from our in _context.OrganisationUnitRole.AsNoTracking()
                     join r in _context.Role.AsNoTracking() on our.RoleId equals r.Id
                     where deptIds.Contains(our.OrganisationUnitId)
                     select new { our.OrganisationUnitId, Role = r })
                    .ToListAsync(cancellationToken);

                var rolesByDept = roleRows
                    .GroupBy(x => x.OrganisationUnitId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => _mapper.Map<Models.V1.Observation.Role>(x.Role)).ToList()
                    );

                // 3) Map to DTOs and attach Roles
                var result = departments
                    .Select(d =>
                    {
                        var dto = _mapper.Map<Models.V1.OrganisationUnit.OrganisationUnit>(d);

                        dto.Roles = rolesByDept.TryGetValue(d.Id, out var roles)
                            ? roles
                            : new List<Models.V1.Observation.Role>();

                        return dto;
                    })
                    .ToList();

                return result;
            }
        }
    }
}
