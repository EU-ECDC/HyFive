using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Handsmykke
{
    public class OppdaterHandsmykkeType
    {
        public class Command : IRequest<HandsmykkeType>
        {
            public HandsmykkeType Handsmykketype { get; set; }
        }

        public class Handler : IRequestHandler<Command, HandsmykkeType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<HandsmykkeType> Handle(Command command, CancellationToken cancellationToken)
            {
                var handsmykketype = await _context.HandsmykkeType
                    .FirstOrDefaultAsync(i => i.Id == command.Handsmykketype.Id, cancellationToken);

                if (handsmykketype == null) throw new Exception($"Fant ikke handsmykketype med id {command.Handsmykketype.Id}");

                handsmykketype.Navn = command.Handsmykketype.Navn;

                _context.HandsmykkeType.Update(handsmykketype);

                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<HandsmykkeType>(handsmykketype);
                return mapped;
            }
        }
    }
}