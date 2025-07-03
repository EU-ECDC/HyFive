using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.FiveIndication
{
    public class GetActivityTypes
    {
        public class Query : IRequest<IEnumerable<ActivityType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<ActivityType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ActivityType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activityTypes = await _context.ActivityType
                    .AsNoTracking()
                    .ProjectTo<ActivityType>(_mapper.ConfigurationProvider)
                    .OrderBy(a => a.Name)
                    .ToListAsync(cancellationToken);
                
                return activityTypes;
            }
        }
    }
}
