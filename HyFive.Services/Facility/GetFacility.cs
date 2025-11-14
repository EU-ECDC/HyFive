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
    public class GetFacility
    {
        public class Query : IRequest<Models.V1.Facility.Facility>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.Facility>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }



            public async Task<Models.V1.Facility.Facility> Handle(Query request, CancellationToken cancellationToken)
            {
                var facility = await _context.Facility
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Include(i => i.PredefinedComment)
                    .Include(i => i.FacilityType)
                    .ProjectTo<Models.V1.Facility.Facility>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(i => i.Id == request.FacilityId, cancellationToken);
                
                if (facility == null)
                {
                    throw new KeyNotFoundException($"Did not find facility with ID: {request.FacilityId}");
                }

                // Pre-fetch the department IDs for this facility that have sessions
                var departmentIds = facility.Departments?.Select(d => d.Id).ToList() ?? new List<int>();

                var hasObservations = departmentIds.Count > 0 &&
                    await _context.Session
                        .Where(s => departmentIds.Contains(s.Department.Id))
                        .AnyAsync(cancellationToken);

                facility.HasObservations = hasObservations;

                // Sort the departments
                facility.Departments = facility.Departments?
                    .OrderBy(d => d.Name)
                    .ToList() ?? new List<Models.V1.Facility.Department>();

                return facility;
            }
        }
    }
}
