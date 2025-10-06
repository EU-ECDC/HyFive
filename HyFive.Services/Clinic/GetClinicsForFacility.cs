using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Clinic
{
    public class GetClinicsForFacility
    {
        public class Query : IRequest<IEnumerable<Models.V1.Facility.Clinic>>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Models.V1.Facility.Clinic>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Models.V1.Facility.Clinic>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Clinic
                    .AsNoTracking()
                    .Include(k => k.Facility)
                    .Include(k => k.Departments)
                    .Where(k => k.Facility.Id == request.FacilityId)
                    .OrderBy(k => k.Name)
                    .ProjectTo<Models.V1.Facility.Clinic>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
