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
    public class UpdateUnit
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
                var unit = await _context
                    .Unit
                    .Include(k => k.Facility)
                    .Include(k => k.Departments)
                    .FirstOrDefaultAsync(a => a.Id == command.Unit.Id);
                if (unit.Facility.Id != command.Unit.FacilityId)
                {
                    throw new DomainException("UnitNotLinkedToFacility", command.Unit.Id, command.Unit.FacilityId);
                }
                else if (command.Unit.Departments.Any(x => x.FacilityId != unit.Facility.Id))
                {
                    throw new DomainException("DepartmentNotLinkedToFacility", command.Unit.FacilityId);
                }

                var departments = await _context
                    .Department
                    .Include(a => a.Units)
                    .Where(a => command.Unit.Departments.Select(av => av.Id).Contains(a.Id))
                    .ToListAsync();

                unit.Departments = departments;
                unit.Name = command.Unit.Name;

                _context.Entry(unit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<Models.V1.Facility.Unit>(unit);
                return mapped;
            }
        }
    }
}
