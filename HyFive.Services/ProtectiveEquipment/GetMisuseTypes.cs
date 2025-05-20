using System.Collections.Generic;
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
    public class GetMisuseTypes
    {
        public class Query : IRequest<List<MisuseType>>
        {
            public int EquipmentTypeId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<MisuseType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<MisuseType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var equipmentMisuseTypes = await _context.MisuseType
                    .AsNoTracking()
                    .Include(ft => ft.ProtectiveEquipmentType)
                    .Where(ft => ft.ProtectiveEquipmentType.Id == request.EquipmentTypeId)
                    .OrderBy(ft => ft.Name)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<MisuseType>>(equipmentMisuseTypes);
                return mapped;
            }
        }
    }
}
