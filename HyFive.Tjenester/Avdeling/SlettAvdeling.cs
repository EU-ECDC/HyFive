using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Avdeling
{
    public class SlettAvdeling
    {
        public class Command : IRequest<bool>
        {
            public int AvdelingId { get; set; }
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
                var institusjon = HentAvdeling(command.AvdelingId);
                

                SlettAvdelingMedTilhørendeData(institusjon);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private Domene.Place.Avdeling HentAvdeling(int avdelingId)
            {
                var avdeling = _context.Department
                                .FirstOrDefault(i => i.Id == avdelingId);

                if (avdeling == null)
                {
                    throw new Exception($"Kunne ikke finne institusjon med id {avdelingId}");
                }

                return avdeling;
            }
        
            private void SlettAvdelingMedTilhørendeData(Domene.Place.Avdeling avdeling)
            {
                    SlettFireIndikasjonerSesjonerOgObservasjoner(avdeling.Id);
                    SlettHandsmykkeSesjonerOgObservasjoner(avdeling.Id);
                    SlettHanskeSesjonerOgObservasjoner(avdeling.Id);
                    SlettBeskyttelsesutstyrSesjonerOgObservasjoner(avdeling.Id);

                    SlettAvdeling(avdeling.Id);
            }

            private void SlettAvdeling(int avdelingId)
            {
                var avdeling = _context.Department.Find(avdelingId);
                _context.Department.Remove(avdeling);
            }

            private void SlettFireIndikasjonerSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<FourIndicationsSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Aktivitet)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    var aktiviteter = sesjon.Observasjoner.Select(o => o.Aktivitet).ToList();
                    _context.Activity.RemoveRange(aktiviteter);
                    _context.FourIndicationsObservation.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHandsmykkeSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<HandJewelrySession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    _context.HandJewelryObservation.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettHanskeSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<GloveSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observasjoner)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    _context.GloveObservation.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }

            private void SlettBeskyttelsesutstyrSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<ProtectiveEquipmentSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o => o.Beskyttelsesutstyrliste)
                    .Where(s => s.Avdeling.Id == avdelingId).ToList();

                foreach (var sesjon in sesjonerForAvdeling)
                {
                    var beskyttelsesutstyrListe = sesjon.Observasjoner.SelectMany(o => o.Beskyttelsesutstyrliste).ToList();
                    _context.RemoveRange(beskyttelsesutstyrListe);
                    _context.ProtectiveEquipmentObservation.RemoveRange(sesjon.Observasjoner);
                    _context.Sesjon.Remove(sesjon);
                }
            }
        }
    }
}