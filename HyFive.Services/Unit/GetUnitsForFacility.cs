using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Place;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Unit
{
    public class GetUnitsForFacility
    {
        public class Query : IRequest<IEnumerable<UnitResponse>>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<UnitResponse>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<UnitResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var unitLevelId = await _context.OrganisationUnitLevel
        .AsNoTracking()
        .Where(l => l.Level == OrganisationUnitLevels.Unit)
        .Select(l => l.Id)
        .FirstOrDefaultAsync(cancellationToken);

                if (unitLevelId == 0)
                    return Enumerable.Empty<UnitResponse>();

                var orgUnitIds = await GetDescendantOrganisationUnitIds(request.FacilityId, cancellationToken);

                var units = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(ou => orgUnitIds.Contains(ou.Id) && ou.LevelId == unitLevelId)
                    .Include(ou => ou.Parent)
                    .OrderBy(ou => ou.Name)
                    .ToListAsync(cancellationToken);

                var unitIds = units.Select(u => u.Id).ToList();

                var associations = await _context.OrganisationUnitAssociation
                    .AsNoTracking()
                    .Where(a =>
                        unitIds.Contains(a.SourceOrganisationUnitId) &&
                        a.AssociationType == OrganisationUnitAssociationType.UnitDepartment)
                    .ToListAsync(cancellationToken);

                var associationDepartmentIdsByUnit = associations
                    .GroupBy(a => a.SourceOrganisationUnitId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.TargetOrganisationUnitId).Distinct().OrderBy(x => x).ToList());

                // Collect all department ids used by all units
                var allDepartmentIds = new HashSet<int>();

                foreach (var unit in units)
                {
                    if (unit.ParentId.HasValue)
                        allDepartmentIds.Add(unit.ParentId.Value);

                    if (associationDepartmentIdsByUnit.TryGetValue(unit.Id, out var extraDepartmentIds))
                    {
                        foreach (var departmentId in extraDepartmentIds)
                            allDepartmentIds.Add(departmentId);
                    }
                }

                // Load department entities
                var departments = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(d => d.LevelRef)
                    .Include(d => d.Type)
                    .Include(d => d.Address)
                    .Include(d => d.OrganisationUnitRoles)
                        .ThenInclude(our => our.Role)
                    .Where(d => allDepartmentIds.Contains(d.Id))
                    .OrderBy(d => d.Name)
                    .ToListAsync(cancellationToken);

                var mappedDepartmentsById = departments
                    .Select(d => _mapper.Map<Models.V1.OrganisationUnit.OrganisationUnit>(d))
                    .ToDictionary(d => d.Id);

                var result = units
                    .Select(u =>
                    {
                        var departmentIds = new HashSet<int>();

                        if (u.ParentId.HasValue)
                            departmentIds.Add(u.ParentId.Value);

                        if (associationDepartmentIdsByUnit.TryGetValue(u.Id, out var extraDepartmentIds))
                        {
                            foreach (var departmentId in extraDepartmentIds)
                                departmentIds.Add(departmentId);
                        }

                        var mappedDepartments = departmentIds
                            .OrderBy(x => x)
                            .Where(id => mappedDepartmentsById.ContainsKey(id))
                            .Select(id => mappedDepartmentsById[id])
                            .ToList();

                        return new UnitResponse
                        {
                            Id = u.Id,
                            FacilityId = request.FacilityId,
                            Name = u.Name,
                            Departments = mappedDepartments
                        };
                    })
                    .OrderBy(x => x.Name)
                    .ToList();

                return result;
            }

            private async Task<List<int>> GetDescendantOrganisationUnitIds(int rootId, CancellationToken ct)
            {
                var all = new HashSet<int> { rootId };
                var frontier = new List<int> { rootId };

                while (frontier.Count > 0)
                {
                    var children = await _context.OrganisationUnit
                        .AsNoTracking()
                        .Where(x => x.ParentId != null && frontier.Contains(x.ParentId.Value))
                        .Select(x => x.Id)
                        .ToListAsync(ct);

                    frontier = children.Where(id => all.Add(id)).ToList();
                }

                return all.ToList();
            }
        }
    }
}
