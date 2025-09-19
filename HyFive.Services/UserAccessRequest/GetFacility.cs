using HyFive.DataAccess;
using HyFive.Models.V1.UserAccessRequest;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.UserAccessRequest
{
    public class GetFacility
    {
        public class Query : IRequest<FacilityForUserAccessRequest>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FacilityForUserAccessRequest>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<FacilityForUserAccessRequest> Handle(Query request, CancellationToken cancellationToken)
            {
                var facility = _context.Facility.AsNoTracking()
                    .FirstOrDefault(i => i.Id == request.FacilityId);

                var facilityForUserAccessRequest = new FacilityForUserAccessRequest
                {
                    Id = facility.Id,
                    Name = facility.Name
                };

                return facilityForUserAccessRequest;
            }
        }
    }
}