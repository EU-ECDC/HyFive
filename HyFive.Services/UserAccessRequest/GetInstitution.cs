using HyFive.DataAccess;
using HyFive.Models.V1.UserAccessRequest;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.UserAccessRequest
{
    public class GetInstitution
    {
        public class Query : IRequest<InstitutionForUserAccessRequest>
        {
            public int InstitutionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, InstitutionForUserAccessRequest>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<InstitutionForUserAccessRequest> Handle(Query request, CancellationToken cancellationToken)
            {
                var institution = _context.Institution.AsNoTracking()
                    .FirstOrDefault(i => i.Id == request.InstitutionId);

                var institutionForUserAccessRequest = new InstitutionForUserAccessRequest
                {
                    Id = institution.Id,
                    Name = institution.Name
                };

                return institutionForUserAccessRequest;
            }
        }
    }
}