using System;
using System.Collections.Generic;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observasjon;

namespace HyFive.Tjenester.Roller
{
    public class OpprettRolle
    {
        public class Command : IRequest<Modeller.V1.Observasjon.Role>
        {
            public OpprettRolleRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Observasjon.Role>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Observasjon.Role> Handle(Command command, CancellationToken cancellationToken)
            {
                // sjekk institusjon
                var rolle = new Domene.Observation.Role()
                {
                    Name = command.Request.Navn,
                    Description = command.Request.Beskrivelse
                };

                _context.Role.Add(rolle);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<Modeller.V1.Observasjon.Role>(rolle);
                return mapped;
            }
        }
    }
}
