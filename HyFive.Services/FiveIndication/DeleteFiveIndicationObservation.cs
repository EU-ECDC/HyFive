using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.FiveIndication
{
    public class DeleteFiveIndicationObservation
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
                    var observation = await _context.FiveIndicationsObservation
                        .Include(o => o.IndicationTypes)
                        .Include(o => o.Activity)
                        .Include(o => o.Role)
                        .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservationId));

                    if (observation == null)
                    {
                        throw new ArgumentException("S-FIO-01: Did not find observation with ID: " + request.ObservationId);
                    }

                    var indicationTypes = await _context.IndicationTypes.Where(i => observation.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToListAsync(cancellationToken);
                    observation.IndicationTypes = indicationTypes;

                    var activity = await _context.Activity.FirstOrDefaultAsync(a => a.Id == observation.Activity.Id, cancellationToken);
                    observation.Activity = activity;

                    var session = await _context.FiveIndicationsSession
                        .Include(s => s.Observations)
                        .FirstOrDefaultAsync(s => s.Id == new Guid(request.SessionId), cancellationToken);

                    if (activity != null)
                    {
                        _context.Remove(activity);
                    }

                    if (session != null && session.Observations.Count == 1 && session.Observations.Select(o => o.Id).Contains(observation.Id))
                    {
                        _context.Remove(session);
                    }

                    _context.Remove(observation);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while deleting five indication observations with ID: {ObservationId}", request.ObservationId);
                    return false;
                }

                return true;
            }
        }
    }
}
