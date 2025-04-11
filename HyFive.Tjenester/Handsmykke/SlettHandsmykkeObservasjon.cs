using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.Handsmykke
{
    public class SlettHandsmykkeObservasjon
    {
        public class Command : IRequest<bool>
        {
            public string ObservasjonId { get; set; }
            public string SesjonId { get; set; }
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
                    var observasjon = await _context.HandJewelryObservation
                        .Include(o => o.HandJewelry)
                        .Include(o => o.Role)
                        .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservasjonId));

                    if (observasjon == null)
                    {
                        throw new Exception("S-HS-03: Kunne ikke finne observasjon med ID " + request.ObservasjonId);
                    }

                    var handsmykker = _context.HandJewelryType.Where(i => observasjon.HandJewelry.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observasjon.HandJewelry = handsmykker;

                    var sesjon = _context.HandJewelrySession
                        .Include(s => s.Observations)
                        .FirstOrDefault(s => s.Id == new Guid(request.SesjonId));

                    if (sesjon != null && sesjon.Observations.Count == 1 && sesjon.Observations.Select(o => o.Id).Contains(observasjon.Id))
                    {
                        _context.Remove(sesjon);
                    }

                    _context.Remove(observasjon);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"S-HS-02: Feil under sletting av handsmykke-observasjon med ID {request.ObservasjonId}");
                    throw;
                }

                return true;
            }
        }
    }
}
