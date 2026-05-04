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
        public class Query : IRequest<List<string>>
        {  }

        public class Handler : IRequestHandler<Query, List<string>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Distinct city names from facility/root organisation units (ParentId == null)
                var cities = await
                    (from ou in _context.OrganisationUnit.AsNoTracking()
                     where ou.ParentId == null && ou.AddressId != null
                     join a in _context.Address.AsNoTracking()
                         on ou.AddressId equals a.Id
                     where a.City != null && a.City != ""
                     orderby a.City
                     select a.City)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                return cities;
            }
        }
    }
}
