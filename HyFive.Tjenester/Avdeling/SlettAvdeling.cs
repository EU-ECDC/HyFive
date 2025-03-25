using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Domene.Sesjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Avdeling
{
    public class SlettAvdeling
    {
        public class Command : IRequest<bool>
        {
            public int AvdelingId { get; set; }
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
                var institusjon = HentAvdeling(command.AvdelingId);
                

                SlettAvdelingMedTilhørendeData(institusjon);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private Domene.Sted.Avdeling HentAvdeling(int avdelingId)
            {
                var avdeling = _context.Avdeling
                                .FirstOrDefault(i => i.Id == avdelingId);

                if (avdeling == null)
                {
                    throw new Exception($"Kunne ikke finne institusjon med id {avdelingId}");
                }

                return avdeling;
            }
        
            private void SlettAvdelingMedTilhørendeData(Domene.Sted.Avdeling avdeling)
            {
                    SlettFireIndikasjonerSesjonerOgObservasjoner(avdeling.Id);
                    SlettHandsmykkeSesjonerOgObservasjoner(avdeling.Id);
                    SlettHanskeSesjonerOgObservasjoner(avdeling.Id);
                    SlettBeskyttelsesutstyrSesjonerOgObservasjoner(avdeling.Id);

                    SlettAvdeling(avdeling.Id);
            }

            private void SlettAvdeling(int avdelingId)
            {
                var avdeling = _context.Avdeling.Find(avdelingId);
                _context.Avdeling.Remove(avdeling);
            }

            private void SlettFireIndikasjonerSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<FireIndikasjonerSesjon>()
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Aktivitet)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    var aktiviteter = sesjon.Observasjoner.Select(o => o.Aktivitet).ToList();
                    _context.Aktivitet.RemoveRange(aktiviteter);
                    _context.FireIndikasjonerObservasjon.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHandsmykkeSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<HandsmykkeSesjon>()
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    _context.HandsmykkeObservasjon.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHanskeSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<HanskeSesjon>()
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    _context.HanskeObservasjon.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettBeskyttelsesutstyrSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<BeskyttelsesutstyrSesjon>()
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Beskyttelsesutstyrliste)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    var beskyttelsesutstyrListe = sesjon.Observasjoner.SelectMany(o => o.Beskyttelsesutstyrliste).ToList();
                    _context.RemoveRange(beskyttelsesutstyrListe);
                    _context.BeskyttelsesutstyrObservasjon.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }
        }
    }
}