using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class GetFacilityTypes
    {
        public class Query : IRequest<IEnumerable<OrganisationUnitType>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<OrganisationUnitType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<OrganisationUnitType>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.OrganisationUnitType
                .AsNoTracking()
                .Where(t => t.Code.StartsWith("F_"))
                .OrderBy(t => t.Id)
                .ProjectTo<OrganisationUnitType>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            }
        }
    }
}