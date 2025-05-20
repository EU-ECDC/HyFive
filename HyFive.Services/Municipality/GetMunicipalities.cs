using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Municipality
{
    public class GetMunicipalities
    {
        public class Query : IRequest<List<Models.V1.Institution.Municipality>>
        { }

        public class Handler : IRequestHandler<Query, List<Models.V1.Institution.Municipality>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<Models.V1.Institution.Municipality>> Handle(Query request, CancellationToken cancellationToken)
            {
                var municipalityList = await _context.Municipality
                                           .AsNoTracking()
                                           .ProjectTo<Models.V1.Institution.Municipality>(_mapper.ConfigurationProvider)
                                           .OrderBy(k => k.Name)
                                           .ToListAsync();

                return municipalityList;
            }
        }
    }
}
