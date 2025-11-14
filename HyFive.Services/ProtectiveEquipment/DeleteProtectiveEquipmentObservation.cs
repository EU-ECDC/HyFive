using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.Glove
{
    public class DeleteProtectiveEquipmentObservation
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
                    var observation = await _context.ProtectiveEquipmentObservation
                        .Include(bu => bu.ProtectiveEquipmentList)
                        .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservationId));

                    if (observation == null)
                    {
                        throw new ArgumentException("S-BU-01: Did not find Protective Equipment observation with ID: " + request.ObservationId);
                    }

                    var session = await _context.ProtectiveEquipmentSession
                        .Include(s => s.Observations)
                        .FirstOrDefaultAsync(s => s.Id == new Guid(request.SessionId), cancellationToken);

                    if (session != null && session.Observations.Count == 1 && session.Observations.Select(o => o.Id).Contains(observation.Id))
                    {
                        _context.Remove(session);
                    }

                    if (observation.ProtectiveEquipmentList?.Any() == true)
                    {
                        _context.RemoveRange(observation.ProtectiveEquipmentList);    
                    }
                    _context.Remove(observation);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while deleting Protective Equipment observation with ID: {ObservationId}", request.ObservationId);
                    return false;
                }

                return true;
            }
        }
    }
}
