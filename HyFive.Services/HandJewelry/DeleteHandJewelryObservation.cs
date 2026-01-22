using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.HandJewelry
{
    public class DeleteHandJewelryObservation
    {
        public class Command : IRequest<bool>
        {
            public string ObservationId { get; set; }
            public string SessionId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                
                var observation = await _context.HandJewelryObservation
                    .Include(o => o.HandJewelries)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservationId));

                if (observation == null)
                {
                    throw new DomainException("ObservationNotFound", request.ObservationId);
                }

                    var handJewelry = await _context.HandJewelryType.Where(i => observation.HandJewelries.Select(oi => oi.Id).Contains(i.Id)).ToListAsync(cancellationToken);
                    observation.HandJewelries = handJewelry;

                    var session = await _context.HandJewelrySession
                        .Include(s => s.Observations)
                        .FirstOrDefaultAsync(s => s.Id == new Guid(request.SessionId), cancellationToken);

                if (session != null && session.Observations.Count == 1 && session.Observations.Select(o => o.Id).Contains(observation.Id))
                {
                    _context.Remove(session);
                }

                _context.Remove(observation);

                return true;
            }
        }
    }
}
