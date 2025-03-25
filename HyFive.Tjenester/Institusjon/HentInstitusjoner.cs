using System.Linq;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace HyFive.Tjenester.Institusjon
{
    public class HentInstitusjoner
    {
        public class Query : IRequest<Modeller.V1.Institusjon.InstitusjonRapport[]>
        {
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institusjon.InstitusjonRapport[]>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Institusjon.InstitusjonRapport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Institusjon
                    .AsNoTracking()
                    .Include(i => i.Institusjontype)
                    .ProjectTo<Modeller.V1.Institusjon.InstitusjonRapport>(_mapper.ConfigurationProvider)
                    .OrderBy(i => i.Navn)
                    .ToArrayAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
