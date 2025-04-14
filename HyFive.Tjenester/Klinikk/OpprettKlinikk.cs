using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Klinikk
{
    public class OpprettKlinikk
    {
        public class Command : IRequest<Models.V1.Institution.Clinic>
        {
            public Models.V1.Institution.Clinic Klinikk { get; set; }
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
                var institusjon = await _context
                    .Institution
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Klinikk.InstitutionId);
                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Klinikk.InstitutionId);
                }
                else if (command.Klinikk.Departments.Any(x => x.InstitusjonId != institusjon.Id))
                {
                    throw new InvalidOperationException($"Minst en avdeling er ikke tilknyttet institusjon med id {command.Klinikk.InstitutionId}");
                }

                var klinikk = new Domene.Place.Clinic()
                {
                    Institution = institusjon,
                    Name = command.Klinikk.Name,
                };

                var avdelinger = await _context.Department
                    .Where(a => a.InstitusjonId == institusjon.Id)
                    .Where(a => command.Klinikk.Departments.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();

                klinikk.Departments = avdelinger;

                _context.Clinic.Add(klinikk);
                await _context.SaveChangesAsync();
                return _mapper.Map<Models.V1.Institution.Clinic>(klinikk);
            }
        }
    }
}
