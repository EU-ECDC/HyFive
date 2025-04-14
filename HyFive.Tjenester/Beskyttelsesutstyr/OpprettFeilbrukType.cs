using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Beskyttelsesutstyr
{
    public class OpprettFeilbrukType
    {
        public class Command : IRequest<IncorrectType>
        {
            public int UtstyrTypeId { get; set; }
            public CreateErrorTypeRequest FeilbrukType { get; set; }
        }

        public class Handler : IRequestHandler<Command, IncorrectType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IncorrectType> Handle(Command request, CancellationToken cancellationToken)
            {
                var utstyrType = await _context.ProtectiveEquipmentType
                    .Include(but => but.MisuseTypes)
                    .FirstOrDefaultAsync(but => but.Id == request.UtstyrTypeId, cancellationToken);
                if (utstyrType == null)
                {
                    throw new Exception("Kunne ikke finne utstyrType med ID " + request.UtstyrTypeId);
                }

                var feilbrukType = new Domain.Observation.ProtectiveEquipment.MisuseType()
                {
                    Name = request.FeilbrukType.Name
                };

                utstyrType.MisuseTypes.Add(feilbrukType);

                _context.ProtectiveEquipmentType.Update(utstyrType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<IncorrectType>(feilbrukType);
                return mapped;
            }
        }
    }
}