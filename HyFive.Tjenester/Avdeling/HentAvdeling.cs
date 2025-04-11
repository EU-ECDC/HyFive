using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Avdeling
{
    public class HentAvdeling
    {
        public class Query : IRequest<Modeller.V1.Institution.Department>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institution.Department>
        {

            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institution.Department> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Department
                    .AsNoTracking()
                    .Where(a => a.Id == request.Id)
                    .ProjectTo<Modeller.V1.Institution.Department>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
