using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Roller
{
    public class HentRolle
    {
        public class Query : IRequest<Models.V1.Observation.Role>
        {
            public int Id = 0;
        }

        public class Handler : IRequestHandler<Query, Models.V1.Observation.Role>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Observation.Role> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Role
                    .AsNoTracking()
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Models.V1.Observation.Role>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
