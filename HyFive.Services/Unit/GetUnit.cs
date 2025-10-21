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
    public class GetUnit
    {
        public class Query : IRequest<Models.V1.Facility.Unit>
        {
            public int Id { get; set; }
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.Unit>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.Unit> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Unit
                    .AsNoTracking()
                    .Include(k => k.Facility)
                    .Where(k => k.Id == request.Id && k.Facility.Id == request.FacilityId)
                    .ProjectTo<Models.V1.Facility.Unit>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
