using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Clinic
{
    public class UpdateClinic
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
                var clinic = await _context
                    .Clinic
                    .Include(k => k.Facility)
                    .Include(k => k.Departments)
                    .FirstOrDefaultAsync(a => a.Id == command.Clinic.Id);
                if (clinic.Facility.Id != command.Clinic.FacilityId)
                {
                    throw new Exception($"Clinic with id {command.Clinic.Id} is not associated with facility with id: {command.Clinic.FacilityId}");
                }
                else if (command.Clinic.Departments.Any(x => x.FacilityId != clinic.Facility.Id))
                {
                    throw new InvalidOperationException($"At least one department is not associated with the facility with id: {command.Clinic.FacilityId}");
                }

                var departments = await _context
                    .Department
                    .Include(a => a.Clinics)
                    .Where(a => command.Clinic.Departments.Select(av => av.Id).Contains(a.Id))
                    .ToListAsync();

                clinic.Departments = departments;
                clinic.Name = command.Clinic.Name;

                _context.Entry(clinic).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<Models.V1.Facility.Clinic>(clinic);
                return mapped;
            }
        }
    }
}
