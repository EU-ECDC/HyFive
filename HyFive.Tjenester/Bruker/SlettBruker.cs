using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Domene.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Bruker
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
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                Domene.Bruker.Bruker bruker = null;
                var type = command.Brukertype;

                if (type == typeof(Koordinator))
                {
                    bruker = await _context.User.OfType<Koordinator>()
                        .FirstOrDefaultAsync(i => i.Id == command.BrukerId, cancellationToken: cancellationToken);
                }
                else if (type == typeof(Observator))
                {
                    bruker = await _context.User.OfType<Observator>()
                        .FirstOrDefaultAsync(i => i.Id == command.BrukerId, cancellationToken: cancellationToken);

                    var harBrukerSesjoner = _context.Sesjon.Any(s => s.Observer.Id == bruker.Id);

                    if (harBrukerSesjoner)
                    {
                        SlettSesjonerOgObservasjoner(bruker.Id);
                    }
                }

                _context.User.Remove(bruker);
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
                var sesjoner = _context.Sesjon.OfType<ProtectiveEquipmentSession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Beskyttelsesutstyrliste)
                    .Where(s => s.Observator.Id == brukerId);

                foreach (var sesjon in sesjoner)
                {
                    var beskyttelsesutstyrListe = sesjon.Observasjoner.SelectMany(o => o.Beskyttelsesutstyrliste);
                    _context.RemoveRange(beskyttelsesutstyrListe);
                    _context.ProtectiveEquipmentObservation.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHanskeSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<GloveSession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Observator.Id == brukerId);

                foreach (var sesjon in sesjoner)
                {
                    _context.GloveObservation.RemoveRange(sesjon.Observasjoner);

                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHandsmykkeSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<HandJewelrySession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Observator.Id == brukerId).ToList();

                foreach (var sesjon in sesjoner)
                {
                    _context.HandJewelryObservation.RemoveRange(sesjon.Observasjoner);

                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettFireIndikasjonerSesjonerOgObservasjoner(int brukerId)
            {
                var sesjoner = _context.Sesjon.OfType<FourIndicationsSession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Aktivitet)
                    .Where(s => s.Observator.Id == brukerId).ToList();

                foreach (var sesjon in sesjoner)
                {
                    var aktiviteter =
                        sesjon.Observasjoner.Select(o => o.Aktivitet).ToList();

                    _context.Activity.RemoveRange(aktiviteter);
                    _context.FourIndicationsObservation.RemoveRange(sesjon.Observasjoner);

                    _context.Sesjon.Remove(sesjon);
                }
            }
        }
    }
}
