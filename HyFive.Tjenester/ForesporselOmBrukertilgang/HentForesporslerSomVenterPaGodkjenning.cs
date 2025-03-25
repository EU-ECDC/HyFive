using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class HentForesporslerSomVenterPaGodkjenning
    {
        public class Query : IRequest<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang[]>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang[]>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.ForesporselOmBrukertilgang
                    .AsNoTracking()
                    .Where(f => f.Status == ForesporselOmBrukertilgangStatus.Registrert && f.InstitusjonId.Value == request.InstitusjonId)
                    .ProjectTo<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
