using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class GetPredefinedComments
    {
        public class Query : IRequest<IEnumerable<string>>
        {
            public int FacilityId { get; set; }
            public SessionType SessionType { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<string>>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                
                var predefinedComments = await _context.PredefinedComment
                    .Where(pk =>
                        pk.OrganisationUnitId == request.FacilityId)
                    .Select(pk => pk.Comment)
                    .ToListAsync(cancellationToken);

                return predefinedComments;
            }
        }
    }
}
