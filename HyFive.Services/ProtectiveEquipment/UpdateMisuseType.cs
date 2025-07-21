using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.ProtectiveEquipment
{
    public class UpdateMisuseType
    {
        public class Command : IRequest<MisuseType>
        {
            public int EquipmentTypeId { get; set; }
            public MisuseType MisuseType { get; set; }
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
                var equipmentType = await _context.ProtectiveEquipmentType
                    .Include(but => but.MisuseTypes)
                    .FirstOrDefaultAsync(but => but.Id == request.EquipmentTypeId, cancellationToken);
                if (equipmentType == null)
                {
                    throw new Exception("Did not find equipment type with ID: " + request.EquipmentTypeId);
                }

                var misuseType = equipmentType.MisuseTypes.FirstOrDefault(fbt => fbt.Id == request.MisuseType.Id);
                if (misuseType == null)
                {
                    throw new Exception("Did not find misuse type with ID: " + request.MisuseType.Id);
                }

                misuseType.Name = request.MisuseType.Name;

                _context.ProtectiveEquipmentType.Update(equipmentType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<MisuseType>(misuseType);
                return mapped;
            }
        }
    }
}