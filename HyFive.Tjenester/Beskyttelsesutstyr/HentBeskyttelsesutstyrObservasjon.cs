using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class HentBeskyttelsesutstyrObservasjon
    {
        public class Query : IRequest<BeskyttelsesutstyrObservasjon>
        {
            public string ObservasjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, BeskyttelsesutstyrObservasjon>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<BeskyttelsesutstyrObservasjon> Handle(Query request, CancellationToken cancellationToken)
            {
                var isGuid = Guid.TryParse(request.ObservasjonId, out Guid guidObservasjonId);

                if (!isGuid)
                {
                    throw new ArgumentException($"IBrukerService: Feil ved parsing av ID som GUID: {request.ObservasjonId}.");
                }

                var observasjon = await _context.BeskyttelsesutstyrObservasjon
                    .Include(b => b.Beskyttelsesutstyrliste)
                    .ThenInclude(bl => bl.Feilbruktyper)
                    .Include(b => b.Beskyttelsesutstyrliste)
                    .ThenInclude(bl => bl.Utstyrstype)
                    .ThenInclude(bu => bu.Feilbruktyper)
                    .Include(b => b.BeskyttelsesutstyrSesjon)
                    .ThenInclude(b => b.Avdeling)
                    .ThenInclude(b => b.Roller)
                    .Include(b => b.Settingtype)
                    .ThenInclude(b => b.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper)
                    .ThenInclude(b => b.BeskyttelsesutstyrType)
                    .FirstAsync(b => b.Id == guidObservasjonId);

                return _mapper.Map<BeskyttelsesutstyrObservasjon>(observasjon);
            }
        }
    }
}