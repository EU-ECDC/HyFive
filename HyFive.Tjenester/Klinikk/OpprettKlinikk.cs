using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Klinikk
{
    public class OpprettKlinikk
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
                var institusjon = await _context
                    .Institusjon
                    .Include(i => i.Avdelinger)
                    .FirstOrDefaultAsync(i => i.Id == command.Klinikk.InstitusjonId);
                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Klinikk.InstitusjonId);
                }
                else if (command.Klinikk.Avdelinger.Any(x => x.InstitusjonId != institusjon.Id))
                {
                    throw new InvalidOperationException($"Minst en avdeling er ikke tilknyttet institusjon med id {command.Klinikk.InstitusjonId}");
                }

                var klinikk = new Domene.Sted.Klinikk()
                {
                    Institusjon = institusjon,
                    Navn = command.Klinikk.Navn,
                };

                var avdelinger = await _context.Avdeling
                    .Where(a => a.InstitusjonId == institusjon.Id)
                    .Where(a => command.Klinikk.Avdelinger.Select(x => x.Id).Contains(a.Id))
                    .ToListAsync();

                klinikk.Avdelinger = avdelinger;

                _context.Klinikk.Add(klinikk);
                await _context.SaveChangesAsync();
                return _mapper.Map<Modeller.V1.Institusjon.Klinikk>(klinikk);
            }
        }
    }
}
