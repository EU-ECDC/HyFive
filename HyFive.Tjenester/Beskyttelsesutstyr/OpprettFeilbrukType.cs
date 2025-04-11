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
    public class OpprettFeilbrukType
    {
        public class Command : IRequest<MisuseType>
        {
            public int UtstyrTypeId { get; set; }
            public OpprettFeilbrukTypeRequest FeilbrukType { get; set; }
        }

        public class Handler : IRequestHandler<Command, MisuseType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<MisuseType> Handle(Command request, CancellationToken cancellationToken)
            {
                var utstyrType = await _context.ProtectiveEquipmentType
                    .Include(but => but.MisuseTypes)
                    .FirstOrDefaultAsync(but => but.Id == request.UtstyrTypeId, cancellationToken);
                if (utstyrType == null)
                {
                    throw new Exception("Kunne ikke finne utstyrType med ID " + request.UtstyrTypeId);
                }

                var feilbrukType = new Domene.Observation.ProtectiveEquipment.MisuseType()
                {
                    Name = request.FeilbrukType.Navn
                };

                utstyrType.MisuseTypes.Add(feilbrukType);

                _context.ProtectiveEquipmentType.Update(utstyrType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<MisuseType>(feilbrukType);
                return mapped;
            }
        }
    }
}