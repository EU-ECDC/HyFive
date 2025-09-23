using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class GetCity
    {
        public class Query : IRequest<FacilityReport[]>
        {
            public int CityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FacilityReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<FacilityReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Facility.Where(x=>x.City.Id == request.CityId);

                var result = await query
                    .ProjectTo<FacilityReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
            }
        }
    }
}
