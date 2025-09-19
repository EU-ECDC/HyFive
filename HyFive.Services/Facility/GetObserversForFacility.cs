using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class GetObserversForFacility
    {
        public class Query : IRequest<Models.V1.User.User[]>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.User.User[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.User.User[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.User
                    .OfType<Domain.User.Observer>()
                    .AsNoTracking()
                    .Include(o => o.Facility)
                    .Where(o => o.Facility.Id == request.FacilityId)
                    .OrderBy(o => o.LastName)
                    .ProjectTo<Models.V1.User.User>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
