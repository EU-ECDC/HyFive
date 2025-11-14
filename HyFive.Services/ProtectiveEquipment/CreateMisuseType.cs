using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Services.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.ProtectiveEquipment
{
    public class CreateMisuseType
    {
        public class Command : IRequest<MisuseType>
        {
            public int EquipmentTypeId { get; set; }
            public CreateIncorrectUseTypeRequest MisuseType { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Command, MisuseType>
        {
            public Handler(HandHygieneContext context, IMapper mapper)
               : base(context, mapper) { }

            public async Task<MisuseType> Handle(Command request, CancellationToken cancellationToken)
            {
                var equipmentType = await GetEquipmentTypeWithMisuseTypesAsync(request.EquipmentTypeId, cancellationToken);

                var misuseType = new Domain.Observation.ProtectiveEquipment.MisuseType()
                {
                    Name = request.MisuseType.Name
                };

                equipmentType.MisuseTypes.Add(misuseType);

                _context.ProtectiveEquipmentType.Update(equipmentType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<MisuseType>(misuseType);
                return mapped;
            }
        }
    }
}