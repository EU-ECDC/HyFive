using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class SlettInstitusjon
    {
        public class Command : IRequest<bool>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var institusjon = HentInstitusjon(command.InstitusjonId);

                SlettAvdelingMedTilhørendeData(institusjon);
                SlettKlinikker(institusjon.Id);
                SlettPredefinerteKommentarer(institusjon.Id);
                SlettBrukere(institusjon.Id);
                SlettInstitusjon(institusjon);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private Domene.Place.Institution HentInstitusjon(int institusjonId)
            {
                var institusjon = _context.Institution
                                .Include(i=>i.Departments)
                                .FirstOrDefault(i=>i.Id == institusjonId);

                if (institusjon == null)
                {
                    throw new Exception($"Kunne ikke finne institusjon med id {institusjonId}");
                }

                return institusjon;
            }

            private void SlettInstitusjon(Domene.Place.Institution institusjon)
            {
                _context.Institution.Remove(institusjon);
            }

            private void SlettBrukere(int institusjonId)
            {
                var brukereForInstitusjon = _context.User.Where(b => b.Institusjon.Id == institusjonId);
                _context.User.RemoveRange(brukereForInstitusjon);
            }

            private void SlettKlinikker(int institusjonsId)
            {
                var klinikker = _context.Clinic.Where(k => k.Institution.Id == institusjonsId);
                _context.Clinic.RemoveRange(klinikker);
            }

            private void SlettPredefinerteKommentarer(int institusjonsId)
            {
                var preDefinerteKommentarer = _context.PredefinedComments.Where(p => p.InstitutionId == institusjonsId);
                _context.PredefinedComments.RemoveRange(preDefinerteKommentarer);
            }

            private void SlettAvdelingMedTilhørendeData(Domene.Place.Institution institusjon)
            {
                foreach (var avdeling in institusjon.Departments)
                {
                    SlettFireIndikasjonerSesjonerOgObservasjoner(avdeling.Id);
                    SlettHandsmykkeSesjonerOgObservasjoner(avdeling.Id);
                    SlettHanskeSesjonerOgObservasjoner(avdeling.Id);
                    SlettBeskyttelsesutstyrSesjonerOgObservasjoner(avdeling.Id);

                    SlettAvdeling(avdeling.Id);
                }
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
                    .ThenInclude(o=>o.Aktivitet)
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
                    .ThenInclude(o=>o.Beskyttelsesutstyrliste)
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
