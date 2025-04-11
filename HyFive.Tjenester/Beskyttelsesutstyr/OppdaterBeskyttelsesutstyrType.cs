using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class OppdaterBeskyttelsesutstyrType
    {
        public class Command : IRequest<ProtectiveEquipmentType>
        {
            public ProtectiveEquipmentType UtstyrType { get; set; }
        }

        public class Handler : IRequestHandler<Command, ProtectiveEquipmentType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ProtectiveEquipmentType> Handle(Command request, CancellationToken cancellationToken)
            {
                var beskyttelsesutstyrType = await _context.ProtectiveEquipmentType
                    .FirstOrDefaultAsync(x => x.Id == request.UtstyrType.Id, cancellationToken);

                if (beskyttelsesutstyrType == null) throw new Exception($"Fant ikke beskyttelsesutstyrType med id {request.UtstyrType.Id}");

                beskyttelsesutstyrType.Name = request.UtstyrType.Name;

                _context.ProtectiveEquipmentType.Update(beskyttelsesutstyrType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<ProtectiveEquipmentType>(beskyttelsesutstyrType);
                return mapped;
            }
        }
    }
}
