using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Institution
{
    public class GetPredefinedComments
    {
        public class Query : IRequest<IEnumerable<string>>
        {
            public int InstitutionId { get; set; }
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
                if (request.SessionType == SessionType.ProtectiveEquipment)
                {
                    var predefinedComments = await _context.PredefinedComments
                        .Where(pk =>
                            pk.InstitutionId == request.InstitutionId &&
                            pk.SessionType == Domain.Place.SessionType.ProtectiveEquipment)
                        .Select(pk => pk.Comment)
                        .ToListAsync(cancellationToken);

                    return predefinedComments;
                }

                return null;
            }
        }
    }
}
