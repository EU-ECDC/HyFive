using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.Hanske
{
    public class SlettHanskeObservasjon
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
                    var observasjon = await _context.GloveObservation
                        .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservasjonId));

                    if (observasjon == null)
                    {
                        throw new Exception("S-H-01: Kunne ikke finne hanske-observasjon med ID " + request.ObservasjonId);
                    }

                    var sesjon = _context.GloveSession
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
                    _logger.LogError(e, $"S-H-02: Feil under sletting av hanske-observasjon med ID {request.ObservasjonId}");
                    throw;
                }

                return true;
            }
        }
    }
}
