using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Institusjon
{
    public class HentPredefinerteKommentarer
    {
        public class Query : IRequest<IEnumerable<string>>
        {
            public int InstitusjonId { get; set; }
            public SessionType Sesjontype { get; set; }
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
                if (request.Sesjontype == SessionType.ProtectiveEquipment)
                {
                    var predefinerteKommentarer = await _context.PredefinedComments
                        .Where(pk =>
                            pk.InstitutionId == request.InstitusjonId &&
                            pk.SessionType == Domene.Place.SessionType.ProtectiveEquipment)
                        .Select(pk => pk.Comment)
                        .ToListAsync(cancellationToken);

                    return predefinerteKommentarer;
                }

                return null;
            }
        }
    }
}
