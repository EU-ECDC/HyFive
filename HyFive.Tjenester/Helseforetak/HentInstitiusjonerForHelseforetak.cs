using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class HentInstitiusjonerForHelseforetak
    {
        public class Query : IRequest<InstitusjonRapport[]>
        {
            public int HelseforetakId { get; set; }
        }

        public class Handler : IRequestHandler<Query, InstitusjonRapport[]>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<InstitusjonRapport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Institusjon.Where(x=>x.Helseforetak.Id == request.HelseforetakId);

                var result = await query
                    .ProjectTo<InstitusjonRapport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
            }
        }
    }
}
