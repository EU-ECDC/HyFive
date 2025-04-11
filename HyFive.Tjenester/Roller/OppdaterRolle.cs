using AutoMapper;
using HyFive.DataAccess;
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
        public class Command : IRequest<Modeller.V1.Observasjon.Role>
        {
            public OppdaterRolleRequest Request { get; set; }
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
                var rolle = await _context.Role.FirstOrDefaultAsync(i => i.Id == command.Request.Id);
                rolle.Name = command.Request.Navn;
                rolle.Description = command.Request.Beskrivelse;

                _context.Role.Update(rolle);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<Modeller.V1.Observasjon.Role>(rolle);
                return mapped;
            }
        }
    }
}
