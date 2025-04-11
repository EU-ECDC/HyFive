using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class HentAlleForesporsler
    {
        public class Query : IRequest<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest[]>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.UserAccessRequest
                    .AsNoTracking()
                    .Where(f => f.InstitusjonId.Value == request.InstitusjonId)
                    .ProjectTo<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
