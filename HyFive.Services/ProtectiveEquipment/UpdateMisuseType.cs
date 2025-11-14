using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Services.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.ProtectiveEquipment
{
    public class UpdateMisuseType
    {
        public class Command : IRequest<MisuseType>
        {
            public int EquipmentTypeId { get; set; }
            public MisuseType MisuseType { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Command, MisuseType>
        {
            public Handler(HandHygieneContext context, IMapper mapper)
               : base(context, mapper) { }

            public async Task<MisuseType> Handle(Command request, CancellationToken cancellationToken)
            {
                var equipmentType = await GetEquipmentTypeWithMisuseTypesAsync(request.EquipmentTypeId, cancellationToken);

                var misuseType = equipmentType.MisuseTypes.FirstOrDefault(fbt => fbt.Id == request.MisuseType.Id);
                if (misuseType == null)
                {
                    throw new ArgumentException("Did not find misuse type with ID: " + request.MisuseType.Id);
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