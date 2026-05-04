using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Department
{
    public class UpdateDepartment
    {
        public class Command : IRequest<OrganisationUnit>
        {
            public int Id { get; set; }
            public int FacilityId { get; set; }
            public string Name { get; set; }
            public int OrganisationUnitTypeId { get; set; }
            public List<int> RoleIds { get; set; }
        }

        public class Handler : IRequestHandler<Command, OrganisationUnit>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<OrganisationUnit> Handle(Command command, CancellationToken cancellationToken)
            {
                // 1) Load department OU + level + roles join
                var departmentOrgUnit = await _context.OrganisationUnit
                    .Include(ou => ou.LevelRef)
                    .Include(ou => ou.OrganisationUnitRoles)
                        .ThenInclude(our => our.Role)
                    .FirstOrDefaultAsync(ou => ou.Id == command.Id, cancellationToken);

                if (departmentOrgUnit == null)
                    throw new DomainException("DepartmentNotFound", command.Id);

                // Ensure it is a Department OU
                if (departmentOrgUnit.LevelRef?.Level != "Department")
                    throw new DomainException("OrganisationUnitIsNotDepartment");

                // 2) Update type (OrganisationUnitType)
                if (command.OrganisationUnitTypeId > 0)
                {
                    var type = await _context.OrganisationUnitType
                        .FirstOrDefaultAsync(t => t.Id == command.OrganisationUnitTypeId, cancellationToken);

                    if (type == null)
                        throw new DomainException("OrganisationUnitTypeNotFound");

                    departmentOrgUnit.TypeId = type.Id;
                }

                // 3) Update roles via OrganisationUnitRole join table
                if (command.RoleIds != null && command.RoleIds.Any())
                {
                    var roleIds = command.RoleIds.Distinct().ToList();

                    // Validate roles exist
                    var existingRoleIds = await _context.Role
                        .AsNoTracking()
                        .Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Id)
                        .ToListAsync(cancellationToken);

                    if (existingRoleIds.Count == 0)
                        throw new ArgumentException("None of the provided Role IDs exist in the database.");

                    // Remove links not in request
                    var toRemove = departmentOrgUnit.OrganisationUnitRoles
                        .Where(link => !roleIds.Contains(link.RoleId))
                        .ToList();
                    if (toRemove.Any())
                        _context.RemoveRange(toRemove);

                    // Add missing links
                    var currentRoleIds = departmentOrgUnit.OrganisationUnitRoles.Select(l => l.RoleId).ToHashSet();
                    var toAdd = roleIds
                        .Where(id => !currentRoleIds.Contains(id))
                        .Select(id => new Domain.Observation.OrganisationUnitRole
                        {
                            OrganisationUnitId = departmentOrgUnit.Id,
                            RoleId = id
                        })
                        .ToList();

                    if (toAdd.Any())
                        _context.AddRange(toAdd);
                }

                // 4) Update name (unique under same parent due to index ParentId+Name)
                departmentOrgUnit.Name = command.Name?.Trim();

                var nameExists = await _context.OrganisationUnit
                    .AnyAsync(ou => ou.ParentId == departmentOrgUnit.ParentId
                                 && ou.Id != departmentOrgUnit.Id
                                 && ou.Name == departmentOrgUnit.Name, cancellationToken);

                if (nameExists)
                    throw new ValidationException("DepartmentNameExists", departmentOrgUnit.Name);

                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<OrganisationUnit>(departmentOrgUnit);
                return mapped;
            }
        }
    }
}
