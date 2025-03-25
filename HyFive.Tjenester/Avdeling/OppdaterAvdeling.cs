using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Avdeling
{
    public class OppdaterAvdeling
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Avdeling>
        {
            public Command() { Roller = new List<Modeller.V1.Observasjon.Rolle>(); }

            public int Id { get; set; }
            public string Navn { get; set; }
            public int AvdelingTypeId { get; set; }
            public List<Modeller.V1.Observasjon.Rolle> Roller { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institusjon.Avdeling>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Institusjon.Avdeling> Handle(Command command, CancellationToken cancellationToken)
            {
                var avdeling = await _context.Avdeling
                                             .Include(a => a.Roller)
                                             .FirstOrDefaultAsync(a => a.Id == command.Id);

                if (command.AvdelingTypeId > 0)
                {
                    var avdelingtype = _context.AvdelingType.FirstOrDefault(a => a.Id == command.AvdelingTypeId);
                    if (avdelingtype == null)
                        throw new Exception("Kunne ikke finne avdelingtype med ID " + command.AvdelingTypeId);
                    avdeling.Avdelingtype = avdelingtype;
                }

                if (command.Roller.Any())
                {
                    var rolleIderForAvdeling = command.Roller.Select(ar => ar.Id).ToList();
                    var rollerForAvdeling = _context.Rolle
                                                             .Include(r => r.Avdelinger)
                                                             .Where(r => rolleIderForAvdeling.Contains(r.Id))
                                                             .ToList();
                    avdeling.Roller = rollerForAvdeling;
                }

                avdeling.Navn = command.Navn;

                _context.Update(avdeling);
                await _context.SaveChangesAsync();
                var mapped = _mapper.Map<Modeller.V1.Institusjon.Avdeling>(avdeling);
                return mapped;
            }
        }
    }
}
