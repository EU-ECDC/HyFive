using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Domene.Sted;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PredefinertKommentar = HyFive.Modeller.V1.Institusjon.PredefinertKommentar;

namespace HyFive.Tjenester.Institusjon
{
    public class HentPredefinertKommentarerForKoordinator
    {
        public class Query : IRequest<List<PredefinertKommentar>>
        {
            public int Institusjonid { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<PredefinertKommentar>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<PredefinertKommentar>> Handle(Query request, CancellationToken cancellationToken)
            {
                var kommentarer = await _context.PredefinertKommentar
                    .AsNoTracking()
                    .Where(pk => pk.InstitusjonId == request.Institusjonid && pk.SesjonType == SesjonType.Beskyttelsesutstyr)
                    .ProjectTo<PredefinertKommentar>(_mapper.ConfigurationProvider)
                    .OrderBy(pk => pk.Kommentar)
                    .ToListAsync(cancellationToken);

                return kommentarer;
            }
        }
    }
}
