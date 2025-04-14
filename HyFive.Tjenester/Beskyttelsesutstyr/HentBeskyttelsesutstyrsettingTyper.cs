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

namespace HyFive.Services.Beskyttelsesutstyr
{
    public class HentBeskyttelsesutstyrsettingTyper
    {
        public class Query : IRequest<IEnumerable<ProtectiveEquipmentSettingType>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<ProtectiveEquipmentSettingType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ProtectiveEquipmentSettingType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var beskyttelsesutstyrsettingTyper = await _context.ProtectiveEquipmentSettingType
                    .Include(b => b.PPEConfigurationTypes)
                    .ThenInclude(b => b.ProtectiveEquipmentType)
                    .ThenInclude(bt => bt.MisuseTypes)
                    .AsNoTracking()
                    .ProjectTo<ProtectiveEquipmentSettingType>(_mapper.ConfigurationProvider)
                    .OrderBy(b => b.Name)
                    .ToListAsync(cancellationToken);

                return beskyttelsesutstyrsettingTyper;
            }
        }
    }
}
