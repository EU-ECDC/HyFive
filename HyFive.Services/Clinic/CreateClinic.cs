using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Clinic
{
    public class CreateClinic
    {
        public class Command : IRequest<Models.V1.Facility.Clinic>
        {
            public Models.V1.Facility.Clinic Clinic { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Facility.Clinic>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.Clinic> Handle(Command command, CancellationToken cancellationToken)
            {
                var facility = await _context
                    .Facility
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Clinic.FacilityId);
                if (facility == null)
                {
                    throw new Exception("Did not find facility with ID: " + command.Clinic.FacilityId);
                }
                else if (command.Clinic.Departments.Any(x => x.FacilityId != facility.Id))
                {
                    throw new InvalidOperationException($"At least one department is not linked to the facility with ID: {command.Clinic.FacilityId}");
                }

                var clinic = new Domain.Place.Clinic()
                {
                    Facility = facility,
                    Name = command.Clinic.Name,
                };

                var departments = await _context.Department
                    .Where(a => a.FacilityId == facility.Id)
                    .Where(a => command.Clinic.Departments.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();

                clinic.Departments = departments;

                _context.Clinic.Add(clinic);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.Facility.Clinic>(clinic);
            }
        }
    }
}
