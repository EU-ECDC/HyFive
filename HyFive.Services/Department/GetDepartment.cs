using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class GetDepartment
    {
        public class Query : IRequest<Models.V1.Facility.Department>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.Department>
        {

            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.Department> Handle(Query request, CancellationToken cancellationToken)
            {
                var entity = await _context.Department
                    .AsNoTracking()
                    .Include(d => d.Facility)
                    .Include(d => d.Roles)
                    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

                if (entity == null)
                    return null;

                return _mapper.Map<Models.V1.Facility.Department>(entity);

            }
        }
    }
}
