using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Unit
{
    public class CreateUnit
    {
        public class Command : IRequest<Models.V1.OrganisationUnit.UnitResponse>
        {
            public CreateUnitRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.OrganisationUnit.UnitResponse>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.OrganisationUnit.UnitResponse> Handle(Command command, CancellationToken cancellationToken)
            {
                var req = command.Request;

                if (req.DepartmentIds == null || !req.DepartmentIds.Any())
                    throw new DomainException("DepartmentIdsRequired");

                var departmentIds = req.DepartmentIds
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var departments = await _context.OrganisationUnit
                    .Include(ou => ou.LevelRef)
                    .Where(ou => departmentIds.Contains(ou.Id))
                    .ToListAsync(cancellationToken);

                if (departments.Count != departmentIds.Count)
                {
                    var foundIds = departments.Select(d => d.Id).ToHashSet();
                    var missingId = departmentIds.First(id => !foundIds.Contains(id));
                    throw new DomainException("DepartmentNotFound", missingId);
                }

                if (departments.Any(d => d.LevelRef?.Level != OrganisationUnitLevels.Department))
                {
                    var invalidDepartment = departments.First(d => d.LevelRef?.Level != OrganisationUnitLevels.Department);
                    throw new DomainException("OrganisationUnitIsNotDepartment", invalidDepartment.Id);
                }

                if (departments.Any(d => d.ParentId != req.FacilityId))
                    throw new DomainException("DepartmentNotLinkedToFacility", req.FacilityId);

                var unitLevelId = await _context.OrganisationUnitLevel
                    .AsNoTracking()
                    .Where(l => l.Level == OrganisationUnitLevels.Unit)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (unitLevelId == 0)
                    throw new DomainException("OrganisationUnitLevelNotFound", OrganisationUnitLevels.Unit);

                var unitName = req.Name?.Trim();
                if (string.IsNullOrWhiteSpace(unitName))
                    throw new ValidationException("UnitNameRequired");

                // Optional: decide whether uniqueness is per primary department or whole facility.
                // This version checks only the primary department.
                var primaryDepartmentId = departmentIds.First();

                var nameExists = await _context.OrganisationUnit
                    .AsNoTracking()
                    .AnyAsync(ou => ou.ParentId == primaryDepartmentId && ou.Name == unitName, cancellationToken);

                if (nameExists)
                    throw new ValidationException("UnitNameExists", unitName);

                var unit = new Domain.Place.OrganisationUnit
                {
                    ParentId = primaryDepartmentId,
                    Name = unitName,
                    LevelId = unitLevelId,
                    Abbreviation = req.Abbreviation?.Trim(),
                    Description = req.Description?.Trim(),
                    TypeId = null
                };

                _context.OrganisationUnit.Add(unit);
                await _context.SaveChangesAsync(cancellationToken);

                var extraDepartmentIds = departmentIds
                    .Where(id => id != primaryDepartmentId)
                    .ToList();

                if (extraDepartmentIds.Any())
                {
                    var associations = extraDepartmentIds.Select(departmentId =>
                        new Domain.Place.OrganisationUnitAssociation
                        {
                            SourceOrganisationUnitId = unit.Id,
                            TargetOrganisationUnitId = departmentId,
                            AssociationType = Domain.Place.OrganisationUnitAssociationType.UnitDepartment
                        }).ToList();

                    _context.OrganisationUnitAssociation.AddRange(associations);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return new Models.V1.OrganisationUnit.UnitResponse
                {
                    Id = unit.Id,
                    FacilityId = req.FacilityId,
                    Name = unitName,
                    Departments = _mapper.Map<List<Models.V1.OrganisationUnit.OrganisationUnit>>(departments)
                };
            }
        }
    }
}
