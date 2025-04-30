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

namespace HyFive.Services.Institution
{
    public class GetInstitutionsForCoordinator
    {
        public class Query : IRequest<Models.V1.Institution.InstitutionReport[]>
        {
            public string CoordinatorHprNumber { get; set; }
            public string CoordinatorPseudonym { get; set; }
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
                if (string.IsNullOrEmpty(request.CoordinatorHprNumber) && string.IsNullOrEmpty(request.CoordinatorPseudonym))
                {
                    throw new ArgumentException(
                        $"The coordinator's HPR number must be greater than 0, or CoordinatorPseudonym must be filled in. HPR number was: {request.CoordinatorHprNumber}. ");
                }
                
                var query = _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Include(i => i.Users)
                    .Include(i => i.PredefinedComments)
                    .Include(i => i.InstitutionType)
                    .Where(i => i.Users
                        .Where(b => b.IsDeactivated == false
                                    && ((HasHprNumber(request.CoordinatorHprNumber) && b.HPRNumber == request.CoordinatorHprNumber) ||
                                        (HasIdentityPseudonym(request.CoordinatorPseudonym) && b.IdentityPseudonym == request.CoordinatorPseudonym)))
                        .Any(b => b.Discriminator == nameof(Coordinator))
                    );

                var result = await query
                    .ProjectTo<Models.V1.Institution.InstitutionReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
            }

            private static bool HasIdentityPseudonym(string identityPseudonym)
            {
                return !string.IsNullOrEmpty(identityPseudonym);
            }
            private static bool HasHprNumber(string hprnumber)
            {
                if (string.IsNullOrEmpty(hprnumber))
                    return false;

                return true;
            }
        }
    }
}
