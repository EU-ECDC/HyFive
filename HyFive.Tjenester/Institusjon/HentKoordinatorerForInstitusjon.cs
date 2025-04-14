using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Institusjon
{
    public class HentKoordinatorerForInstitusjon
    {
        public class Query : IRequest<Models.V1.User.User[]>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.User.User[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.User.User[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.User
                    .OfType<Domene.Bruker.Koordinator>()
                    .AsNoTracking()
                    .Include(o => o.Institusjon)
                    .Where(o => o.Institusjon.Id == request.InstitusjonId)
                    .OrderBy(o => o.Etternavn)
                    .ProjectTo<Models.V1.User.User>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
