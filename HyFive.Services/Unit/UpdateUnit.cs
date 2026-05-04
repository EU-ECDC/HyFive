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
    public class UpdateUnit
    {
        public class Command : IRequest<UnitResponse>
        {
            public UpdateUnitRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, UnitResponse>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<UnitResponse> Handle(Command command, CancellationToken cancellationToken)
            {
                var req = command.Request;

                if (req.DepartmentIds == null || !req.DepartmentIds.Any())
                    throw new DomainException("DepartmentIdsRequired");

                var requestedDepartmentIds = req.DepartmentIds
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var unit = await _context.OrganisationUnit
                    .Include(x => x.LevelRef)
                    .FirstOrDefaultAsync(x => x.Id == req.Id, cancellationToken);

                if (unit == null)
                    throw new DomainException("UnitNotFound", req.Id);

                if (unit.LevelRef?.Level != OrganisationUnitLevels.Unit)
                    throw new DomainException("OrganisationUnitIsNotUnit", req.Id);

                var belongsToFacility = await IsAncestorAsync(req.FacilityId, unit.Id, cancellationToken);
                if (!belongsToFacility)
                    throw new DomainException("UnitNotLinkedToFacility", req.Id, req.FacilityId);

                var departments = await _context.OrganisationUnit
                    .Include(x => x.LevelRef)
                    .Where(x => requestedDepartmentIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                if (departments.Count != requestedDepartmentIds.Count)
                {
                    var foundIds = departments.Select(d => d.Id).ToHashSet();
                    var missingDepartmentId = requestedDepartmentIds.First(id => !foundIds.Contains(id));
                    throw new DomainException("DepartmentNotFound", missingDepartmentId);
                }

                var invalidDepartment = departments.FirstOrDefault(d => d.LevelRef?.Level != OrganisationUnitLevels.Department);
                if (invalidDepartment != null)
                    throw new DomainException("OrganisationUnitIsNotDepartment", invalidDepartment.Id);

                if (departments.Any(d => d.ParentId != req.FacilityId))
                    throw new DomainException("DepartmentNotLinkedToFacility", req.FacilityId);

                var newName = string.IsNullOrWhiteSpace(req.Name)
                    ? unit.Name
                    : req.Name.Trim();

                var newAbbreviation = req.Abbreviation?.Trim();
                var newDescription = req.Description?.Trim();

                var primaryDepartmentId = requestedDepartmentIds.First();

                // Uniqueness under the primary department, excluding this unit itself
                var nameExists = await _context.OrganisationUnit
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id != unit.Id &&
                        x.ParentId == primaryDepartmentId &&
                        x.Name == newName,
                        cancellationToken);

                if (nameExists)
                    throw new ValidationException("UnitNameExists", newName);

                // Update the single unit row
                unit.ParentId = primaryDepartmentId;
                unit.Name = newName;
                unit.Abbreviation = newAbbreviation ?? unit.Abbreviation;
                unit.Description = newDescription ?? unit.Description;

                // Replace extra department associations
                var existingAssociations = await _context.OrganisationUnitAssociation
                    .Where(a =>
                        a.SourceOrganisationUnitId == unit.Id &&
                        a.AssociationType == Domain.Place.OrganisationUnitAssociationType.UnitDepartment)
                    .ToListAsync(cancellationToken);

                if (existingAssociations.Any())
                    _context.OrganisationUnitAssociation.RemoveRange(existingAssociations);

                var extraDepartmentIds = requestedDepartmentIds
                    .Where(id => id != primaryDepartmentId)
                    .ToList();

                if (extraDepartmentIds.Any())
                {
                    var newAssociations = extraDepartmentIds.Select(departmentId =>
                        new Domain.Place.OrganisationUnitAssociation
                        {
                            SourceOrganisationUnitId = unit.Id,
                            TargetOrganisationUnitId = departmentId,
                            AssociationType = Domain.Place.OrganisationUnitAssociationType.UnitDepartment
                        }).ToList();

                    _context.OrganisationUnitAssociation.AddRange(newAssociations);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new UnitResponse
                {
                    Id = unit.Id,
                    FacilityId = req.FacilityId,
                    Name = newName,
                    Departments = _mapper.Map<List<Models.V1.OrganisationUnit.OrganisationUnit>>(departments)
                };
            }

            private async Task<bool> IsAncestorAsync(int ancestorId, int nodeId, CancellationToken ct)
            {
                if (ancestorId == nodeId) return true;

                var currentParentId = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(x => x.Id == nodeId)
                    .Select(x => x.ParentId)
                    .FirstOrDefaultAsync(ct);

                var safety = 0;
                while (currentParentId.HasValue && safety++ < 50)
                {
                    if (currentParentId.Value == ancestorId)
                        return true;

                    currentParentId = await _context.OrganisationUnit
                        .AsNoTracking()
                        .Where(x => x.Id == currentParentId.Value)
                        .Select(x => x.ParentId)
                        .FirstOrDefaultAsync(ct);
                }

                return false;
            }
        }
    }
}
