using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Services.Facility
{
    public class GetComplianceFacilities
    {
        public class Query : IRequest<List<Models.V1.Facility.Facility>>
        {
            public List<int> FacilityIds { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.Facility.Facility>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }



            public async Task<List<Models.V1.Facility.Facility>> Handle(Query request, CancellationToken cancellationToken)
            {
                var facilities = await _context.Facility
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Include(i => i.PredefinedComment)
                    .Include(i => i.FacilityType)
                    .Where(i => request.FacilityIds.Contains(i.Id))
                    .ProjectTo<Models.V1.Facility.Facility>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                //if (facility == null)
                //{
                //    throw new Exception($"Could not find facility with ID: {request.FacilityId}");
                //}

                foreach (var facility in facilities)
                {
                    facility.HasObservations =
                        facility.Departments != null &&
                        facility.Departments.Any() &&
                        _context.Session.Include(s => s.Department)
                            .Any(s => facility.Departments.Select(d => d.Id).Contains(s.Department.Id));

                    facility.Departments = facility.Departments.OrderBy(d => d.Name).ToList();
                }

                return facilities;
            }
        }
    }
}
