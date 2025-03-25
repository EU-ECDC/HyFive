using System;
using System.Collections.Generic;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observasjon;

namespace HyFive.Tjenester.Roller
{
    public class OpprettRolle
    {
        public class Command : IRequest<Modeller.V1.Observasjon.Rolle>
        {
            public OpprettRolleRequest Request { get; set; }
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
                // sjekk institusjon
                var rolle = new Domene.Observasjon.Rolle()
                {
                    Navn = command.Request.Navn,
                    Beskrivelse = command.Request.Beskrivelse
                };

                _context.Rolle.Add(rolle);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<Modeller.V1.Observasjon.Rolle>(rolle);
                return mapped;
            }
        }
    }
}
