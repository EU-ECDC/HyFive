using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Beskyttelsesutstyr
{
    public class HentFeilbrukTyper
    {
        public class Query : IRequest<List<IncorrectType>>
        {
            public int UtstyrTypeId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<IncorrectType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<IncorrectType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var feilbrukTyperForUtstyr = await _context.MisuseType
                    .AsNoTracking()
                    .Include(ft => ft.BeskyttelsesutstyrType)
                    .Where(ft => ft.BeskyttelsesutstyrType.Id == request.UtstyrTypeId)
                    .OrderBy(ft => ft.Name)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<IncorrectType>>(feilbrukTyperForUtstyr);
                return mapped;
            }
        }
    }
}
