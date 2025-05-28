using System.Linq;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace HyFive.Services.Institution
{
    public class GetInstitutions
    {
        public class Query : IRequest<Models.V1.Institution.InstitutionReport[]>
        {
        }

        public class Handler : IRequestHandler<Query, Models.V1.Institution.InstitutionReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Institution.InstitutionReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Institution
                    .AsNoTracking()
                    .Include(i => i.InstitutionType)
                    .ProjectTo<Models.V1.Institution.InstitutionReport>(_mapper.ConfigurationProvider)
                    .OrderBy(i => i.Name)
                    .ToArrayAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
