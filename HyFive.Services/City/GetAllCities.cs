using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class GetAllCities
    {
        public class Query : IRequest<List<Models.V1.OrganisationUnit.City>>
        {  }

        public class Handler : IRequestHandler<Query, List<Models.V1.OrganisationUnit.City>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Models.V1.OrganisationUnit.City>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.City
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new Models.V1.OrganisationUnit.City
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
            }
        }
    }
}
