using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Roles
{
    public class HentRollerForAvdeling
    {
        public class Query : IRequest<List<Models.V1.Observation.Role>>
        {
            public int DepartmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.Observation.Role>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Models.V1.Observation.Role>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roller = await _context.Department
                    .Where(a => a.Id == request.DepartmentId)
                    .SelectMany(a => a.Role)
                    .ProjectTo<Models.V1.Observation.Role>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return roller;
            }
        }
    }
}
