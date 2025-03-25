using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Domene.Sesjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Bruker
{
    public class SlettBruker
    {
        public class Command : IRequest<bool>
        {
            public Type Brukertype { get; set; }
            public int BrukerId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                Domene.Bruker.Bruker bruker = null;
                var type = command.Brukertype;

                if (type == typeof(Koordinator))
                {
                    bruker = await _context.Bruker.OfType<Koordinator>()
                        .FirstOrDefaultAsync(i => i.Id == command.BrukerId, cancellationToken: cancellationToken);
                }
                else if (type == typeof(Observator))
                {
                    bruker = await _context.Bruker.OfType<Observator>()
                        .FirstOrDefaultAsync(i => i.Id == command.BrukerId, cancellationToken: cancellationToken);

                    var harBrukerSesjoner = _context.Sesjon.Any(s => s.Observator.Id == bruker.Id);

                    if (harBrukerSesjoner)
                    {
                        SlettSesjonerOgObservasjoner(bruker.Id);
                    }
                }

                _context.Bruker.Remove(bruker);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }

            private void SlettSesjonerOgObservasjoner(int brukerId)
            {
                SlettFireIndikasjonerSesjonerOgObservasjoner(brukerId);
                SlettHandsmykkeSesjonerOgObservasjoner(brukerId);
                SlettHanskeSesjonerOgObservasjoner(brukerId);
                SlettBeskyttelsesutstyrSesjonerOgObservasjoner(brukerId);
            }

            private void SlettBeskyttelsesutstyrSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<BeskyttelsesutstyrSesjon>()
                    .Include(s => s.Observator)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Beskyttelsesutstyrliste)
                    .Where(s => s.Observator.Id == brukerId);

                foreach (var sesjon in sesjoner)
                {
                    var beskyttelsesutstyrListe = sesjon.Observasjoner.SelectMany(o => o.Beskyttelsesutstyrliste);
                    _context.RemoveRange(beskyttelsesutstyrListe);
                    _context.BeskyttelsesutstyrObservasjon.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHanskeSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<HanskeSesjon>()
                    .Include(s => s.Observator)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Observator.Id == brukerId);

                foreach (var sesjon in sesjoner)
                {
                    _context.HanskeObservasjon.RemoveRange(sesjon.Observasjoner);

                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHandsmykkeSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<HandsmykkeSesjon>()
                    .Include(s => s.Observator)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Observator.Id == brukerId).ToList();

                foreach (var sesjon in sesjoner)
                {
                    _context.HandsmykkeObservasjon.RemoveRange(sesjon.Observasjoner);

                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettFireIndikasjonerSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<FireIndikasjonerSesjon>()
                    .Include(s => s.Observator)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Aktivitet)
                    .Where(s => s.Observator.Id == brukerId).ToList();

                foreach (var sesjon in sesjoner)
                {
                    var aktiviteter =
                        sesjon.Observasjoner.Select(o => o.Aktivitet).ToList();

                    _context.Aktivitet.RemoveRange(aktiviteter);
                    _context.FireIndikasjonerObservasjon.RemoveRange(sesjon.Observasjoner);

                    _context.Sesjon.Remove(sesjon);
                }
            }
        }
    }
}
