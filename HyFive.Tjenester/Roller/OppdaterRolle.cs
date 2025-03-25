using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Roller
{
    public class OppdaterRolle
    {
        public class Command : IRequest<Modeller.V1.Observasjon.Rolle>
        {
            public OppdaterRolleRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Observasjon.Rolle>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Observasjon.Rolle> Handle(Command command, CancellationToken cancellationToken)
            {
                var rolle = await _context.Rolle.FirstOrDefaultAsync(i => i.Id == command.Request.Id);
                rolle.Navn = command.Request.Navn;
                rolle.Beskrivelse = command.Request.Beskrivelse;

                _context.Rolle.Update(rolle);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<Modeller.V1.Observasjon.Rolle>(rolle);
                return mapped;
            }
        }
    }
}
