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
                var institution = await _context
                    .Institution
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Clinic.InstitutionId);
                if (institution == null)
                {
                    throw new Exception("Did not find institution with ID: " + command.Clinic.InstitutionId);
                }
                else if (command.Clinic.Departments.Any(x => x.InstitutionId != institution.Id))
                {
                    throw new InvalidOperationException($"At least one department is not linked to the institution with ID: {command.Clinic.InstitutionId}");
                }

                var clinic = new Domain.Place.Clinic()
                {
                    Institution = institution,
                    Name = command.Clinic.Name,
                };

                var departments = await _context.Department
                    .Where(a => a.InstitutionId == institution.Id)
                    .Where(a => command.Clinic.Departments.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();

                clinic.Departments = departments;

                _context.Clinic.Add(clinic);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.Institution.Clinic>(clinic);
            }
        }
    }
}
