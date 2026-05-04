using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class GetDepartment
    {
        public class Query : IRequest<Models.V1.OrganisationUnit.OrganisationUnit>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.OrganisationUnit.OrganisationUnit>
        {

            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.OrganisationUnit.OrganisationUnit> Handle(Query request, CancellationToken cancellationToken)
            {
                // Load Unit + Parent (facility) + Roles
                var orgUnit = await _context.Set<Domain.Place.OrganisationUnit>()
                    .AsNoTracking()
                    .Include(x => x.Parent)
                    .Include(x => x.Children)
                    .Include(x => x.LevelRef)
                    .Include(x => x.OrganisationUnitRoles)
                        .ThenInclude(our => our.Role)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (orgUnit == null)
                    return null;

                // Ensure it is actually a Department OU
                if (orgUnit.LevelRef?.Level != "Department")
                    throw new DomainException("OrganisationUnitIsNotDepartment");

                return _mapper.Map<Models.V1.OrganisationUnit.OrganisationUnit>(orgUnit);

            }
        }
    }
}
