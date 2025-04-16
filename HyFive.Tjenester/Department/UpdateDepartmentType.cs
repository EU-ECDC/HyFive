using System;
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
    public class UpdateDepartmentType
    {
        public class Command : IRequest<DepartmentType>
        {
            public DepartmentType DepartmentType { get; set; }
        }

        public class Handler : IRequestHandler<Command, DepartmentType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<DepartmentType> Handle(Command request, CancellationToken cancellationToken)
            {
                var departmentType = await _context.SectionType.FirstOrDefaultAsync(a => a.Id == request.DepartmentType.Id);
                if (departmentType != default(Domain.Place.DepartmentType))
                {
                    departmentType.Name = request.DepartmentType.Name;

                    _context.Update(departmentType);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<Models.V1.Institution.DepartmentType>(departmentType);
                }

                throw new ArgumentException($"Could not find department type with ID {request.DepartmentType.Id}");
            }
        }
    }
}