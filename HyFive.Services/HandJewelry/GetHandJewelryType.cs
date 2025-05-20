using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HandJewelry
{
    public class GetHandJewelryType
    {
        public class Query : IRequest<Models.V1.Observation.HandJewelryType>
        {
            public int Id = 0;
        }

        public class Handler : IRequestHandler<Query, Models.V1.Observation.HandJewelryType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Observation.HandJewelryType> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.HandJewelryType
                    .AsNoTracking()
                    .ProjectTo<Models.V1.Observation.HandJewelryType>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            }
        }
    }
}
