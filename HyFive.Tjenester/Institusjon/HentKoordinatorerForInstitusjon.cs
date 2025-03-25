using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Institusjon
{
    public class HentKoordinatorerForInstitusjon
    {
        public class Query : IRequest<Modeller.V1.Bruker.Bruker[]>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Bruker.Bruker[]>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Bruker.Bruker[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Bruker
                    .OfType<Domene.Bruker.Koordinator>()
                    .AsNoTracking()
                    .Include(o => o.Institusjon)
                    .Where(o => o.Institusjon.Id == request.InstitusjonId)
                    .OrderBy(o => o.Etternavn)
                    .ProjectTo<Modeller.V1.Bruker.Bruker>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
