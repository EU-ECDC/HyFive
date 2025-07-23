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
                        throw new Exception("S-FIO-01: Did not find observation with ID: " + request.ObservationId);
                    }

                    var indicationTypes = _context.IndicationTypes.Where(i => observation.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observation.IndicationTypes = indicationTypes;

                    var activity = _context.Activity.FirstOrDefault(a => a.Id == observation.Activity.Id);
                    observation.Activity = activity;

                    var session = _context.FiveIndicationsSession
                        .Include(s => s.Observations)
                        .FirstOrDefault(s => s.Id == new Guid(request.SessionId));

                    if (activity != null)
                    {
                        _context.Remove(activity);
                    }

                    if (session != null && session.Observations.Count == 1 && session.Observations.Select(o => o.Id).Contains(observation.Id))
                    {
                        _context.Remove(session);
                    }

                    _context.Remove(observation);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"S-FIO-02: Error while deleting five indication observations with ID: {request.ObservationId}");
                    throw;
                }

                return true;
            }
        }
    }
}
