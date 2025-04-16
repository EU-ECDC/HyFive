using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Department
{
    public class UpdateDepartment
    {
        public class Command : IRequest<Models.V1.Institution.Department>
        {
            public Command() { Role = new List<Models.V1.Observation.Role>(); }

            public int Id { get; set; }
            public string Name { get; set; }
            public int DepartmentTypeId { get; set; }
            public List<Models.V1.Observation.Role> Role { get; set; }
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
                var department = await _context.Department
                                             .Include(a => a.Role)
                                             .FirstOrDefaultAsync(a => a.Id == command.Id);

                if (command.DepartmentTypeId > 0)
                {
                    var avdelingtype = _context.SectionType.FirstOrDefault(a => a.Id == command.DepartmentTypeId);
                    if (avdelingtype == null)
                        throw new Exception("Could not find department type with ID " + command.DepartmentTypeId);
                    department.DepartmentType = avdelingtype;
                }

                if (command.Role.Any())
                {
                    var departmentRoleIds = command.Role.Select(ar => ar.Id).ToList();
                    var departmentRoles = _context.Role
                                                             .Include(r => r.Departments)
                                                             .Where(r => departmentRoleIds.Contains(r.Id))
                                                             .ToList();
                    department.Role = departmentRoles;
                }

                department.Name = command.Name;

                _context.Update(department);
                await _context.SaveChangesAsync();
                var mapped = _mapper.Map<Models.V1.Institution.Department>(department);
                return mapped;
            }
        }
    }
}
