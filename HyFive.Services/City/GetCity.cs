using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class GetCity
    {
        public class Query : IRequest<FacilityReport[]>
        {
            public int CityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FacilityReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<FacilityReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.CityId <= 0)
                    return Array.Empty<FacilityReport>();

                var cityId = request.CityId;

                var result = await
                    (from ou in _context.OrganisationUnit.AsNoTracking()
                     where ou.ParentId == null
                     join a in _context.Address.AsNoTracking()
                         on ou.AddressId equals a.Id into a1
                     from a in a1.DefaultIfEmpty()
                     join t in _context.OrganisationUnitType.AsNoTracking()
                         on ou.TypeId equals t.Id
                     where a.City != null && a.CityId == cityId
                     orderby ou.Name
                     select new FacilityReport
                     {
                         Id = ou.Id,
                         Name = ou.Name,
                         Abbreviation = ou.Abbreviation,
                         City = a.City.Name,
                         Type = new HyFive.Models.V1.OrganisationUnit.OrganisationUnitType
                         {
                             Id = t.Id,
                             Code = t.Code,
                             Name = t.Name,
                             Description = t.Description
                         }
                     })
                    .ToArrayAsync(cancellationToken);

                return result;
            }
        }
    }
}
