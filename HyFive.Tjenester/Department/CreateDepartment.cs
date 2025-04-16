using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Institution;

namespace HyFive.Services.Department
{
    public class CreateDepartment
    {
        public class Command : IRequest<Models.V1.Institution.Department>
        {
            public CreateDepartmentRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Institution.Department>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Institution.Department> Handle(Command command, CancellationToken cancellationToken)
            {
                var institution = await _context
                    .Institution
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Request.InstitutionId);

                if (institution == null)
                {
                    throw new Exception("Could not find department type with ID " + command.Request.InstitutionId);
                }

                if (command.Request.RoleIds.Any() == false)
                    throw new Exception($"Role list is empty. Department must be created with at least one role.");

                var departmentType = _context.SectionType.FirstOrDefault(a => a.Id == command.Request.DepartmentTypeId);
                if (departmentType == null)
                {
                    throw new Exception("Could not find department type with ID " + command.Request.DepartmentTypeId);
                }

                var department = new Domain.Place.Department()
                {
                    InstitutionId = institution.Id,
                    Name = command.Request.Name,
                    Role = GetRoles(command.Request.RoleIds),
                    DepartmentType = departmentType
                };

                _context.Department.Add(department);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.Institution.Department>(department);
            }

            private ICollection<Domain.Observation.Role> GetRoles(List<int> requestRoleIds)
            {
                var roles = _context.Role.Where(r => requestRoleIds.Contains(r.Id)).ToList();
                return roles;
            }
        }
    }
}
