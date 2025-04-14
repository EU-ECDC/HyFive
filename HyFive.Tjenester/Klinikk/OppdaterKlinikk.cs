using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Klinikk
{
    public class OppdaterKlinikk
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
                var klinikk = await _context
                    .Clinic
                    .Include(k => k.Institution)
                    .Include(k => k.Departments)
                    .FirstOrDefaultAsync(a => a.Id == command.Klinikk.Id);
                if (klinikk.Institusjon.Id != command.Klinikk.InstitutionId)
                {
                    throw new Exception($"Klinikk med id {command.Klinikk.Id} er ikke tilknyttet institusjon med id {command.Klinikk.InstitutionId}");
                }
                else if (command.Klinikk.Departments.Any(x => x.InstitusjonId != klinikk.Institusjon.Id))
                {
                    throw new InvalidOperationException($"Minst en avdeling er ikke tilknyttet institusjon med id {command.Klinikk.InstitutionId}");
                }

                var avdelinger = await _context
                    .Department
                    .Include(a => a.Klinikker)
                    .Where(a => command.Klinikk.Departments.Select(av => av.Id).Contains(a.Id))
                    .ToListAsync();

                klinikk.Avdelinger = avdelinger;
                klinikk.Navn = command.Klinikk.Name;

                _context.Entry(klinikk).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<Models.V1.Institution.Clinic>(klinikk);
                return mapped;
            }
        }
    }
}
