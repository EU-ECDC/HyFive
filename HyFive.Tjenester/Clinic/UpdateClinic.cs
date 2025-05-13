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
        public class Command : IRequest<Models.V1.Institution.Clinic>
        {
            public Models.V1.Institution.Clinic Clinic { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Institution.Clinic>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Institution.Clinic> Handle(Command command, CancellationToken cancellationToken)
            {
                var clinic = await _context
                    .Clinic
                    .Include(k => k.Institution)
                    .Include(k => k.Departments)
                    .FirstOrDefaultAsync(a => a.Id == command.Clinic.Id);
                if (clinic.Institution.Id != command.Clinic.InstitutionId)
                {
                    throw new Exception($"Clinic with id {command.Clinic.Id} is not associated with institution with id: {command.Clinic.InstitutionId}");
                }
                else if (command.Clinic.Departments.Any(x => x.InstitutionId != clinic.Institution.Id))
                {
                    throw new InvalidOperationException($"At least one department is not associated with the institution with id: {command.Clinic.InstitutionId}");
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

                var mapped = _mapper.Map<Models.V1.Institution.Clinic>(clinic);
                return mapped;
            }
        }
    }
}
