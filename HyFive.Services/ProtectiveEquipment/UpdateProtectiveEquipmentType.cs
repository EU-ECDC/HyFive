using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.ProtectiveEquipment
{
    public class UpdateProtectiveEquipmentType
    {
        public class Command : IRequest<ProtectiveEquipmentType>
        {
            public ProtectiveEquipmentType EquipmentType { get; set; }
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
                var protectiveEquipmentType = await _context.ProtectiveEquipmentType
                    .FirstOrDefaultAsync(x => x.Id == request.EquipmentType.Id, cancellationToken);

                if (protectiveEquipmentType == null) throw new Exception($"Did not find protective equipment type with ID: {request.EquipmentType.Id}");

                protectiveEquipmentType.Name = request.EquipmentType.Name;

                _context.ProtectiveEquipmentType.Update(protectiveEquipmentType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<ProtectiveEquipmentType>(protectiveEquipmentType);
                return mapped;
            }
        }
    }
}
