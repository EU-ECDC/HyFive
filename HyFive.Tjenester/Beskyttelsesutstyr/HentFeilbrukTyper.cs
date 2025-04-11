using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class HentFeilbrukTyper
    {
        public class Query : IRequest<List<MisuseType>>
        {
            public int UtstyrTypeId { get; set; }
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
                var feilbrukTyperForUtstyr = await _context.MisuseType
                    .AsNoTracking()
                    .Include(ft => ft.BeskyttelsesutstyrType)
                    .Where(ft => ft.BeskyttelsesutstyrType.Id == request.UtstyrTypeId)
                    .OrderBy(ft => ft.Name)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<MisuseType>>(feilbrukTyperForUtstyr);
                return mapped;
            }
        }
    }
}
