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

namespace HyFive.Services.HandJewelry
{
    public class GetHandJewelryTypes
    {
        public class Query : IRequest<IEnumerable<HandJewelryType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<HandJewelryType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HandJewelryType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var handJewelryType = await _context.HandJewelryType.AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Order)
                    .ProjectTo<HandJewelryType>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                return handJewelryType;
            }
        }
    }
}
