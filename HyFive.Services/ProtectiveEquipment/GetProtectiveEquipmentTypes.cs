using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.ProtectiveEquipment
{
    public class GetProtectiveEquipmentTypes
    {
        public class Query : IRequest<IEnumerable<ProtectiveEquipmentType>>
        {

        }

        public class Handler : IRequestHandler<Query, IEnumerable<ProtectiveEquipmentType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ProtectiveEquipmentType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var protectiveEquipmentTypes = await _context.ProtectiveEquipmentType
                    .Include(but => but.MisuseTypes)
                    .AsNoTracking()
                    .ProjectTo<ProtectiveEquipmentType>(_mapper.ConfigurationProvider)
                    .OrderBy(but => but.Name)
                    .ToListAsync(cancellationToken);

                return protectiveEquipmentTypes;
            }
        }
    }
}
