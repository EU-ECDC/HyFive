using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class SlettFireIndikasjonerObservasjon
    {
        public class Command : IRequest<bool>
        {
            public string ObservasjonId { get; set; }
            public string SesjonId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandhygieneContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(HandhygieneContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var observasjon = await _context.FireIndikasjonerObservasjon
                        .Include(o => o.Indikasjonstyper)
                        .Include(o => o.Aktivitet)
                        .Include(o => o.Rolle)
                        .FirstOrDefaultAsync(o => o.Id == new Guid(request.ObservasjonId));

                    if (observasjon == null)
                    {
                        throw new Exception("S-FIO-01: Kunne ikke finne observasjon med ID " + request.ObservasjonId);
                    }

                    var indikasjonstyper = _context.Indikasjon.Where(i => observasjon.Indikasjonstyper.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observasjon.Indikasjonstyper = indikasjonstyper;

                    var aktivitet = _context.Aktivitet.FirstOrDefault(a => a.Id == observasjon.Aktivitet.Id);
                    observasjon.Aktivitet = aktivitet;

                    var sesjon = _context.FireIndikasjonerSesjon
                        .Include(s => s.Observasjoner)
                        .FirstOrDefault(s => s.Id == new Guid(request.SesjonId));

                    if (aktivitet != null)
                    {
                        _context.Remove(aktivitet);
                    }

                    if (sesjon != null && sesjon.Observasjoner.Count == 1 && sesjon.Observasjoner.Select(o => o.Id).Contains(observasjon.Id))
                    {
                        _context.Remove(sesjon);
                    }

                    _context.Remove(observasjon);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"S-FIO-02: Feil under sletting av fire indikasoner-observasjon med ID {request.ObservasjonId}");
                    throw;
                }

                return true;
            }
        }
    }
}
