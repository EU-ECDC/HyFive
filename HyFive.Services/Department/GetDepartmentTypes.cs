using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
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
        public class Query : IRequest<IEnumerable<Models.V1.OrganisationUnit.OrganisationUnitType>>
        {
        }

        public class Handler : IRequestHandler<GetDepartmentTypes.Query, IEnumerable<Models.V1.OrganisationUnit.OrganisationUnitType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.OrganisationUnit.OrganisationUnitType>> Handle(GetDepartmentTypes.Query request, CancellationToken cancellationToken)
            {
                return await _context.OrganisationUnitType
                .AsNoTracking()
                .Where(t => t.Code.StartsWith("D_"))
                .OrderBy(t => t.Id)
                .ProjectTo<OrganisationUnitType>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            }
        }
    }
}
