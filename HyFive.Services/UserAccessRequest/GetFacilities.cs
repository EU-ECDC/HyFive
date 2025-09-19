using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.UserAccessRequest
{
    public class GetFacilities
    {
        public class Query : IRequest<List<Models.V1.UserAccessRequest.FacilityForUserAccessRequest>>
        {
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.UserAccessRequest.FacilityForUserAccessRequest>>
        {
            private readonly int FacilityIdForFHI = 1;

            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<List<Models.V1.UserAccessRequest.FacilityForUserAccessRequest>> Handle(Query request, CancellationToken cancellationToken)
            {
                var facilities = _context.Facility.AsNoTracking()
                             .Where(i=> i.Id != FacilityIdForFHI)
                             .Select(i => new Models.V1.UserAccessRequest.FacilityForUserAccessRequest()
                             {
                                 Id = i.Id, 
                                 Name = i.Name
                             })
                             .OrderBy(i=>i.Name)
                             .ToList();

                return facilities;
            }
        }
    }
}
