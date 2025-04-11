using AutoMapper;
using HyFive.DataAccess;
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
        public class Command : IRequest<HandJewelryType>
        {
            public HandJewelryType Handsmykketype { get; set; }
        }

        public class Handler : IRequestHandler<Command, HandJewelryType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<HandJewelryType> Handle(Command command, CancellationToken cancellationToken)
            {
                var handsmykketype = await _context.HandJewelryType
                    .FirstOrDefaultAsync(i => i.Id == command.Handsmykketype.Id, cancellationToken);

                if (handsmykketype == null) throw new Exception($"Fant ikke handsmykketype med id {command.Handsmykketype.Id}");

                handsmykketype.Name = command.Handsmykketype.Name;

                _context.HandJewelryType.Update(handsmykketype);

                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<HandJewelryType>(handsmykketype);
                return mapped;
            }
        }
    }
}