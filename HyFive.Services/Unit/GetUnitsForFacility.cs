using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Unit
{
    public class GetUnitsForFacility
    {
        public class Query : IRequest<IEnumerable<Models.V1.Facility.Unit>>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.Facility.Unit>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.Facility.Unit>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Unit
                    .AsNoTracking()
                    .Include(k => k.Facility)
                    .Include(k => k.Departments)
                    .Where(k => k.Facility.Id == request.FacilityId)
                    .OrderBy(k => k.Name)
                    .ProjectTo<Models.V1.Facility.Unit>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
