using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Klinikk
{
    public class HentKlinikk
    {
        public class Query : IRequest<Modeller.V1.Institusjon.Klinikk>
        {
            public int Id { get; set; }
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institusjon.Klinikk>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institusjon.Klinikk> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Klinikk
                    .AsNoTracking()
                    .Include(k => k.Institusjon)
                    .Where(k => k.Id == request.Id && k.Institusjon.Id == request.InstitusjonId)
                    .ProjectTo<Modeller.V1.Institusjon.Klinikk>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
