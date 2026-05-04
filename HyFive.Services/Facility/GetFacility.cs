using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Services.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class GetFacility
    {
        public class Query : IRequest<Models.V1.OrganisationUnit.OrganisationUnit>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.OrganisationUnit.OrganisationUnit>
        {
            private readonly HandHygieneContext _context;
            private readonly IMediator _mediator;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMediator mediator, IMapper mapper)
            {
                _context = context;
                _mediator = mediator;
                _mapper = mapper;
            }



            public async Task<Models.V1.OrganisationUnit.OrganisationUnit> Handle(Query request, CancellationToken cancellationToken)
            {
                var facility = await _mediator.Send(new GetOrganisationUnit.Query
                {
                    Id = request.FacilityId,
                    IncludeChildren = true
                });

                if (facility == null)
                {
                    throw new DomainException("FacilityNotFound", request.FacilityId);
                }

                // Departments are the facility's children
                var departments = facility.Children ?? new List<Models.V1.OrganisationUnit.OrganisationUnit>();

                // Get department ids
                var departmentIds = departments.Select(d => d.Id).ToList();

                // Get unit ids under those departments (DB query, because Children likely doesn't include grandchildren)
                var unitEntities = departmentIds.Count == 0
                    ? new List<Domain.Place.OrganisationUnit>()
                    : await _context.OrganisationUnit
                        .AsNoTracking()
                        .Include(u => u.Type)
                        .Include(u => u.LevelRef)
                        .Include(u => u.Address)
                        .Include(u => u.OrganisationUnitRoles)
                            .ThenInclude(our => our.Role)
                        .Where(u => u.ParentId.HasValue && departmentIds.Contains(u.ParentId.Value))
                        .OrderBy(u => u.Name)
                        .ToListAsync(cancellationToken);

                var unitsByDepartmentId = unitEntities
                    .GroupBy(u => u.ParentId!.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => _mapper.Map<List<Models.V1.OrganisationUnit.OrganisationUnit>>(g.ToList()));

                foreach (var department in departments)
                {
                    department.Children = unitsByDepartmentId.TryGetValue(department.Id, out var departmentUnits)
                        ? departmentUnits.OrderBy(u => u.Name).ToList()
                        : new List<Models.V1.OrganisationUnit.OrganisationUnit>();
                }

                var unitIds = unitEntities.Select(u => u.Id).ToList();

                // HasObservations = any session recorded at unit level under this facility
                facility.HasObservations = unitIds.Count > 0 &&
                    await _context.Session.AsNoTracking()
                        .AnyAsync(s => unitIds.Contains(s.OrganisationUnitId), cancellationToken);

                // Sort departments by name (children list)
                facility.Children = departments
                    .OrderBy(d => d.Name)
                    .ToList();

                return facility;
            }
        }
    }
}
