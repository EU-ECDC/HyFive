using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Roller
{
    public class HentRolle
    {
        public class Query : IRequest<Modeller.V1.Observasjon.Rolle>
        {
            public int Id = 0;
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Observasjon.Rolle>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Observasjon.Rolle> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Rolle
                    .AsNoTracking()
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Modeller.V1.Observasjon.Rolle>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
