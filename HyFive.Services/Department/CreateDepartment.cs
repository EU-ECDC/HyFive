using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Facility;

namespace HyFive.Services.Department
{
    public class CreateDepartment
    {
        public class Command : IRequest<Models.V1.Facility.Department>
        {
            public CreateDepartmentRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Facility.Department>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Facility.Department> Handle(Command command, CancellationToken cancellationToken)
            {
                var facility = await _context
                    .Facility
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Request.FacilityId);

                if (facility == null)
                {
                    throw new ArgumentException("Did not find facility with ID " + command.Request.FacilityId);
                }

                if (!command.Request.RoleIds.Any())
                    throw new ArgumentException($"Role list is empty. Department must be created with at least one role.");

                var departmentType = await _context.DepartmentType.FirstOrDefaultAsync(a => a.Id == command.Request.DepartmentTypeId, cancellationToken);
                if (departmentType == null)
                {
                    throw new ArgumentException("Did not find department type with ID " + command.Request.DepartmentTypeId);
                }

                var selectedRoles = await _context.Role
                    .Where(r => command.Request.RoleIds.Contains(r.Id))
                    .ToListAsync(cancellationToken);

                if (selectedRoles.Count == 0)
                    throw new ArgumentException("None of the provided Role IDs exist in the database.");

                bool nameExists = await _context.Department
                    .AnyAsync(d => d.Name == command.Request.Name && d.FacilityId == command.Request.FacilityId);
                if(nameExists)
                {
                    throw new ArgumentException($"A department with the name '{command.Request.Name}' already exists in this facility.");
                }

                var department = new Domain.Place.Department()
                {
                    FacilityId = facility.Id,
                    Name = command.Request.Name,
                    Roles = selectedRoles,
                    DepartmentType = departmentType
                };

                _context.Department.Add(department);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.Facility.Department>(department);
            }
        }
    }
}
