using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Roller
{
    public class HentRolle
    {
        public class Query : IRequest<Modeller.V1.Observasjon.Role>
        {
            public int Id = 0;
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Observasjon.Role>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Observasjon.Role> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Role
                    .AsNoTracking()
                    .Where(r => r.Id == request.Id)
                    .ProjectTo<Modeller.V1.Observasjon.Role>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
