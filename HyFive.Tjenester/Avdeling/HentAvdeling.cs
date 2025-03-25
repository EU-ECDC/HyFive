using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Avdeling
{
    public class HentAvdeling
    {
        public class Query : IRequest<Modeller.V1.Institusjon.Avdeling>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institusjon.Avdeling>
        {

            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institusjon.Avdeling> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Avdeling
                    .AsNoTracking()
                    .Where(a => a.Id == request.Id)
                    .ProjectTo<Modeller.V1.Institusjon.Avdeling>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
