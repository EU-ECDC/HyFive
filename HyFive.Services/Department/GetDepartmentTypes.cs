using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class GetDepartmentTypes
    {
        public class Query : IRequest<IEnumerable<Models.V1.Institution.DepartmentType>>
        {
            public int InstitutionId { get; set; }
        }

        public class Handler : IRequestHandler<GetDepartmentTypes.Query, IEnumerable<Models.V1.Institution.DepartmentType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.Institution.DepartmentType>> Handle(GetDepartmentTypes.Query request, CancellationToken cancellationToken)
            {
                return await _context.DepartmentType
                    .AsNoTracking()
                    .OrderBy(a => a.Name)
                    .ProjectTo<Models.V1.Institution.DepartmentType>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
