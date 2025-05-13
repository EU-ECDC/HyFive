using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
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
            private readonly ILogger<Handler> _logger;

            public Handler(HandHygieneContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var observation = await _context.HandJewelryObservation
                        .Include(o => o.HandJewelry)
                        .Include(o => o.Role)
                        .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservationId));

                    if (observation == null)
                    {
                        throw new Exception("S-HS-03: Could not find observation with ID: " + request.ObservationId);
                    }

                    var handJewelry = _context.HandJewelryType.Where(i => observation.HandJewelry.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observation.HandJewelry = handJewelry;

                    var session = _context.HandJewelrySession
                        .Include(s => s.Observations)
                        .FirstOrDefault(s => s.Id == new Guid(request.SessionId));

                    if (session != null && session.Observations.Count == 1 && session.Observations.Select(o => o.Id).Contains(observation.Id))
                    {
                        _context.Remove(session);
                    }

                    _context.Remove(observation);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"S-HS-02: Error while deleting bracelet observation with ID: {request.ObservationId}");
                    throw;
                }

                return true;
            }
        }
    }
}
