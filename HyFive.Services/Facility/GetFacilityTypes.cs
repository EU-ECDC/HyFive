using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class GetFacilityTypes
    {
        public class Query : IRequest<IEnumerable<FacilityType>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<FacilityType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<FacilityType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var facilityTypes = await _context.FacilityType.AsNoTracking()
                    .OrderBy(i => i.Id)
                    .ToListAsync(cancellationToken);
                var mapped = _mapper.Map<List<FacilityType>>(facilityTypes);
                return mapped;
            }
        }
    }
}