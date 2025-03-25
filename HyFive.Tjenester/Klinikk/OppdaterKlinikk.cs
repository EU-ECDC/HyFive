using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Klinikk
{
    public class OppdaterKlinikk
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Klinikk>
        {
            public Modeller.V1.Institusjon.Klinikk Klinikk { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institusjon.Klinikk>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institusjon.Klinikk> Handle(Command command, CancellationToken cancellationToken)
            {
                var klinikk = await _context
                    .Klinikk
                    .Include(k => k.Institusjon)
                    .Include(k => k.Avdelinger)
                    .FirstOrDefaultAsync(a => a.Id == command.Klinikk.Id);
                if (klinikk.Institusjon.Id != command.Klinikk.InstitusjonId)
                {
                    throw new Exception($"Klinikk med id {command.Klinikk.Id} er ikke tilknyttet institusjon med id {command.Klinikk.InstitusjonId}");
                }
                else if (command.Klinikk.Avdelinger.Any(x => x.InstitusjonId != klinikk.Institusjon.Id))
                {
                    throw new InvalidOperationException($"Minst en avdeling er ikke tilknyttet institusjon med id {command.Klinikk.InstitusjonId}");
                }

                var avdelinger = await _context
                    .Avdeling
                    .Include(a => a.Klinikker)
                    .Where(a => command.Klinikk.Avdelinger.Select(av => av.Id).Contains(a.Id))
                    .ToListAsync();

                klinikk.Avdelinger = avdelinger;
                klinikk.Navn = command.Klinikk.Navn;

                _context.Entry(klinikk).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<Modeller.V1.Institusjon.Klinikk>(klinikk);
                return mapped;
            }
        }
    }
}
