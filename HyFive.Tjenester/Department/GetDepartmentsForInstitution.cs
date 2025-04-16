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
    public class GetDepartmentsForInstitution
    {
        public class Query : IRequest<IEnumerable<Models.V1.Institution.Department>>
        {
            public int InstitutionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.Institution.Department>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.Institution.Department>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Department
                    .Include(a => a.Role)
                    .Include(a => a.DepartmentType)
                    .AsNoTracking()
                    .Where(a => a.InstitutionId == request.InstitutionId)
                    .OrderBy(a => a.Name)
                    .ProjectTo<Models.V1.Institution.Department>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
