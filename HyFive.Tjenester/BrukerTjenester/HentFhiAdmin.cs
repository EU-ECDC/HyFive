using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Modeller.V1.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.BrukerTjenester
{
    public class HentFhiAdmin
    {
        public class Query : IRequest<Modeller.V1.User.User[]>
        {
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.User.User[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.User.User[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.User
                    .OfType<Domene.Bruker.FhiAdmin>()
                    .AsNoTracking()
                    .ProjectTo<Modeller.V1.User.User>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
