using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class GetFacilitiesForCoordinator
    {
        public class Query : IRequest<Models.V1.Facility.FacilityReport[]>
        {
            public string CoordinatorEmail { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.FacilityReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.FacilityReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                                
                var query = _context.Facility
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Include(i => i.Users)
                    .Include(i => i.PredefinedComment)
                    .Include(i => i.FacilityType)
                    .Where(i => i.Users
                        .Where(b => !b.IsDeactivated 
                                    && (HasEmail(request.CoordinatorEmail) && b.Email == request.CoordinatorEmail))
                        .Any(b => b.Discriminator == nameof(Coordinator))
                    )
                    .OrderBy(i => i.Name);

                var result = await query
                    .ProjectTo<Models.V1.Facility.FacilityReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
            }

            private static bool HasEmail(string email)
            {
                if (string.IsNullOrEmpty(email))
                    return false;

                return true;
            }
        }
    }
}
