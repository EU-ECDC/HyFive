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
    public class GetClinic
    {
        public class Query : IRequest<Models.V1.Facility.Clinic>
        {
            public int Id { get; set; }
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.Clinic>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.Clinic> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Clinic
                    .AsNoTracking()
                    .Include(k => k.Facility)
                    .Where(k => k.Id == request.Id && k.Facility.Id == request.FacilityId)
                    .ProjectTo<Models.V1.Facility.Clinic>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
