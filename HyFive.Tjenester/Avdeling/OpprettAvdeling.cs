using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institusjon;

namespace HyFive.Tjenester.Avdeling
{
    public class OpprettAvdeling
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Avdeling>
        {
            public OpprettAvdelingRequest Request { get; set; }
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
                var institusjon = await _context
                    .Institusjon
                    .Include(i => i.Avdelinger)
                    .FirstOrDefaultAsync(i => i.Id == command.Request.InstitusjonId);

                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Request.InstitusjonId);
                }

                if (command.Request.RolleIder.Any() == false)
                    throw new Exception($"Rolle-liste er tom. Avdeling må opprettes med minst en rolle.");

                var avdelingtype = _context.AvdelingType.FirstOrDefault(a => a.Id == command.Request.AvdelingTypeId);
                if (avdelingtype == null)
                {
                    throw new Exception("Kunne ikke finne avdelingtype med ID " + command.Request.AvdelingTypeId);
                }

                var avdeling = new Domene.Sted.Avdeling()
                {
                    InstitusjonId = institusjon.Id,
                    Navn = command.Request.Navn,
                    Roller = HentRoller(command.Request.RolleIder),
                    Avdelingtype = avdelingtype
                };

                _context.Avdeling.Add(avdeling);
                await _context.SaveChangesAsync();
                return _mapper.Map<Modeller.V1.Institusjon.Avdeling>(avdeling);
            }

            private ICollection<Domene.Observasjon.Rolle> HentRoller(List<int> requestRolleIder)
            {
                var roller = _context.Rolle.Where(r => requestRolleIder.Contains(r.Id)).ToList();
                return roller;
            }
        }
    }
}
