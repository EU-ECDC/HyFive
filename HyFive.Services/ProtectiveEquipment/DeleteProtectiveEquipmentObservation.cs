using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
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

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                
                var observation = await _context.ProtectiveEquipmentObservation
                    .Include(bu => bu.ProtectiveEquipmentList)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservationId));

                if (observation == null)
                {
                    throw new DomainException("ProtectiveEquipmentObservationNotFound", request.ObservationId);
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
                

                return true;
            }
        }
    }
}
