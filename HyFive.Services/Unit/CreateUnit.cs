using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Unit
{
    public class CreateUnit
    {
        public class Command : IRequest<Models.V1.Facility.Unit>
        {
            public Models.V1.Facility.Unit Unit { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Facility.Unit>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var facility = await _context
                    .Facility
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Unit.FacilityId);
                if (facility == null)
                {
                    throw new DomainException("FacilityNotFound", command.Unit.FacilityId);
                }
                else if (command.Unit.Departments.Any(x => x.FacilityId != facility.Id))
                {
                    throw new DomainException("DepartmentNotLinkedToFacility", command.Unit.FacilityId);
                }

                var unit = new Domain.Place.Unit()
                {
                    Facility = facility,
                    Name = command.Unit.Name,
                };

                var departments = await _context.Department
                    .Where(a => a.FacilityId == facility.Id)
                    .Where(a => command.Unit.Departments.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();

                unit.Departments = departments;

                _context.Unit.Add(unit);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.Facility.Unit>(unit);
            }
        }
    }
}
