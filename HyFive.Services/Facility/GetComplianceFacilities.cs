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
        public class Query : IRequest<List<Models.V1.OrganisationUnit.OrganisationUnit>>
        {
            public List<int> FacilityIds { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.OrganisationUnit.OrganisationUnit>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Models.V1.OrganisationUnit.OrganisationUnit>> Handle(Query request, CancellationToken cancellationToken)
            {
                var facilities = await LoadFacilitiesAsync(request.FacilityIds, cancellationToken);

                if (facilities.Count == 0)
                    return facilities;

                var departmentIds = GetDepartmentIds(facilities);
                var (unitsByDepartmentId, unitIds) = await LoadUnitsByDepartmentAsync(departmentIds, cancellationToken);
                var facilityIdsWithSessionsSet = await GetFacilityIdsWithSessionsAsync(request.FacilityIds, unitIds, cancellationToken);
                var rolesByDept = await LoadRolesByDepartmentAsync(departmentIds, cancellationToken);

                AttachRolesAndUnits(facilities, rolesByDept, unitsByDepartmentId, facilityIdsWithSessionsSet);

                return facilities;
            }

            private async Task<List<Models.V1.OrganisationUnit.OrganisationUnit>> LoadFacilitiesAsync(List<int> facilityIds, CancellationToken cancellationToken)
            {
                return await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(f => f.LevelRef)
                    .Include(f => f.Type)
                    .Include(f => f.Address)
                    .Include(f => f.Children)
                        .ThenInclude(d => d.LevelRef)
                    .Include(f => f.Children)
                        .ThenInclude(d => d.Type)
                    .Where(f => facilityIds.Contains(f.Id))
                    .Select(f => _mapper.Map<Models.V1.OrganisationUnit.OrganisationUnit>(f))
                    .ToListAsync(cancellationToken);
            }

            private static List<int> GetDepartmentIds(List<Models.V1.OrganisationUnit.OrganisationUnit> facilities)
            {
                return facilities
                    .SelectMany(f => f.Children ?? Enumerable.Empty<Models.V1.OrganisationUnit.OrganisationUnit>())
                    .Select(d => d.Id)
                    .Distinct()
                    .ToList();
            } 

            private async Task<(Dictionary<int, List<Models.V1.OrganisationUnit.OrganisationUnit>>, List<int>)> LoadUnitsByDepartmentAsync(List<int> departmentIds, CancellationToken cancellationToken)
            {
                if (departmentIds.Count == 0)
                    return (new Dictionary<int, List<Models.V1.OrganisationUnit.OrganisationUnit>>(), new List<int>());

                var unitEntities = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(u => u.LevelRef)
                    .Include(u => u.Type)
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

                var unitIds = unitEntities.Select(u => u.Id).ToList();

                return (unitsByDepartmentId, unitIds);
            }

            private async Task<HashSet<int>> GetFacilityIdsWithSessionsAsync(List<int> facilityIds, List<int> unitIds, CancellationToken cancellationToken)
            {
                if (unitIds.Count == 0)
                    return new HashSet<int>();

                var facilityIdsWithSessions = await
                    (from s in _context.Session.AsNoTracking()
                     join u in _context.OrganisationUnit.AsNoTracking() on s.OrganisationUnitId equals u.Id
                     join d in _context.OrganisationUnit.AsNoTracking() on u.ParentId equals d.Id
                     join f in _context.OrganisationUnit.AsNoTracking() on d.ParentId equals f.Id
                     where facilityIds.Contains(f.Id)
                     select f.Id
                    )
                    .Distinct()
                    .ToListAsync(cancellationToken);
                        
                return facilityIdsWithSessions.ToHashSet();
            }

            private async Task<Dictionary<int, List<Models.V1.Observation.Role>>> LoadRolesByDepartmentAsync(List<int> departmentIds, CancellationToken cancellationToken)
            {
                if (departmentIds.Count == 0)
                    return new Dictionary<int, List<Models.V1.Observation.Role>>();

                var deptRoleEntities = await _context.OrganisationUnitRole
                    .AsNoTracking()
                    .Where(our => departmentIds.Contains(our.OrganisationUnitId))
                    .Include(our => our.Role)
                    .ToListAsync(cancellationToken);

                return deptRoleEntities
                    .GroupBy(x => x.OrganisationUnitId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => _mapper.Map<Models.V1.Observation.Role>(x.Role)).ToList()
                    );
            }

            private static void AttachRolesAndUnits(
                List<Models.V1.OrganisationUnit.OrganisationUnit> facilities,
                Dictionary<int, List<Models.V1.Observation.Role>> rolesByDept,
                Dictionary<int, List<Models.V1.OrganisationUnit.OrganisationUnit>> unitsByDepartmentId,
                HashSet<int> facilityIdsWithSessionsSet)
            {
                foreach (var facility in facilities)
                {
                    facility.HasObservations = facilityIdsWithSessionsSet.Contains(facility.Id);

                    if (facility.Children == null)
                    {
                        facility.Children = new List<Models.V1.OrganisationUnit.OrganisationUnit>();
                        continue;
                    }

                    foreach (var dept in facility.Children)
                    {
                        dept.Roles = rolesByDept.TryGetValue(dept.Id, out var roles)
                            ? roles
                            : new List<Models.V1.Observation.Role>();

                        dept.Children = unitsByDepartmentId.TryGetValue(dept.Id, out var units)
                            ? units.OrderBy(u => u.Name).ToList()
                            : new List<Models.V1.OrganisationUnit.OrganisationUnit>();
                    }

                    facility.Children = facility.Children
                        .OrderBy(d => d.Name)
                        .ToList();
                }
            }
        }
    }
}
