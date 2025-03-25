using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Konstanter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Institusjon
{
    public class OppdaterInstitusjon
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Institusjon>
        {
            public Modeller.V1.Institusjon.Institusjon Institusjon { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institusjon.Institusjon>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Institusjon.Institusjon> Handle(Command command, CancellationToken cancellationToken)
            {
                // sjekk institusjontype:
                var institusjontype = await _context.InstitusjonType.FirstOrDefaultAsync(i => i.Id == command.Institusjon.Institusjontype.Id);
                if (institusjontype == null)
                    throw new ArgumentException(
                        $"Institusjonstype med id {command.Institusjon.Institusjontype.Id} fantes ikke i databasen");

                var institusjon = await _context.Institusjon
                                                .Include(i => i.Kommune)
                                                .Include(i => i.Helseforetak)
                                                .FirstOrDefaultAsync(i => i.Id == command.Institusjon.Id);

                if (command.Institusjon.Kommune != null && command.Institusjon.Institusjontype.Kode == InstitusjonstypeKonstanter.Sykehjem)
                {
                    var kommune = await _context.Kommune.FirstOrDefaultAsync(k => k.Id == command.Institusjon.Kommune.Id);
                    institusjon.Kommune = kommune;
                }
                else
                {
                    institusjon.Kommune = null;
                }

                if (command.Institusjon.Helseforetak != null && command.Institusjon.Institusjontype.Kode == InstitusjonstypeKonstanter.Sykehus)
                {
                    var helseforetak = await _context.Helseforetak.FirstOrDefaultAsync(h => h.Id == command.Institusjon.Helseforetak.Id);
                    institusjon.Helseforetak = helseforetak;
                }
                else
                {
                    institusjon.Helseforetak = null;
                }
                
                institusjon.Navn = command.Institusjon.Navn;
                institusjon.Forkortelse = command.Institusjon.Forkortelse;
                institusjon.HERId = command.Institusjon.HERId;
                institusjon.Institusjontype = institusjontype;

                _context.Institusjon.Update(institusjon);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Modeller.V1.Institusjon.Institusjon>(institusjon);
                return mapped;
            }
        }
    }
}
