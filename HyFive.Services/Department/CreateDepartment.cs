using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Services.Localization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class CreateDepartment
    {
        public class Command : IRequest<OrganisationUnit>
        {
            public CreateDepartmentRequest Request { get; set; }
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
                var req = command.Request;

                var facility = await _context.OrganisationUnit
                    .Include(x => x.LevelRef)
                    .FirstOrDefaultAsync(x => x.Id == req.FacilityId, cancellationToken);

                if (facility == null)
                {
                    throw new DomainException("FacilityNotFound", req.FacilityId);
                   
                }

                if (facility.LevelRef?.Level != "Facility" || facility.ParentId != null)
                    throw new DomainException("OrganisationUnitIsNotFacility", req.FacilityId);

                // Department level (server-side)
                var deptLevel = await _context.OrganisationUnitLevel
                    .FirstOrDefaultAsync(l => l.Level == "Department", cancellationToken);

                // Department type (OU type)
                var deptType = await _context.OrganisationUnitType
                    .FirstOrDefaultAsync(t => t.Id == req.DepartmentTypeId, cancellationToken);

                if (deptType == null)
                    throw new DomainException("DepartmentTypeNotFound", req.DepartmentTypeId);

                if (!req.RoleIds.Any())
                    throw new DomainException("EmptyRoleList");

                // Validate roles
                var roleIds = req.RoleIds.Distinct().ToList();
                var roles = await _context.Role
                    .Where(r => roleIds.Contains(r.Id))
                    .ToListAsync(cancellationToken);

                if (roles.Count == 0)
                    throw new ArgumentException("None of the provided Role IDs exist in the database.");

                // Unique name under same parent (matches your unique index)
                var nameExists = await _context.OrganisationUnit
                    .AnyAsync(x => x.ParentId == req.FacilityId && x.Name == req.Name, cancellationToken);

                if (nameExists)
                    throw new ValidationException("DepartmentNameExists", req.Name);


                

                var departmentOrganizationUnit = new Domain.Place.OrganisationUnit()
                {
                    ParentId = req.FacilityId,
                    Name = req.Name.Trim(),
                    LevelId = deptLevel.Id,
                    TypeId = deptType.Id

                };

                _context.Add(departmentOrganizationUnit);
                await _context.SaveChangesAsync(cancellationToken);

                // Join table OrganisationUnitRole
                var links = roles.Select(r => new Domain.Observation.OrganisationUnitRole
                {
                    OrganisationUnitId = departmentOrganizationUnit.Id,
                    RoleId = r.Id
                }).ToList();

                _context.OrganisationUnitRole.AddRange(links);

                await _context.SaveChangesAsync();

                var createdDepartment = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.Children)
                    .Include(x => x.OrganisationUnitRoles)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.Type)
                    .Include(x => x.LevelRef)
                    .FirstAsync(x => x.Id == departmentOrganizationUnit.Id, cancellationToken);

                return _mapper.Map<OrganisationUnit>(createdDepartment);
            }
        }
    }
}
