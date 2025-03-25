using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Sesjon;
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
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
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

            private Domene.Sted.Institusjon HentInstitusjon(int institusjonId)
            {
                var institusjon = _context.Institusjon
                                .Include(i=>i.Avdelinger)
                                .FirstOrDefault(i=>i.Id == institusjonId);

                if (institusjon == null)
                {
                    throw new Exception($"Kunne ikke finne institusjon med id {institusjonId}");
                }

                return institusjon;
            }

            private void SlettInstitusjon(Domene.Sted.Institusjon institusjon)
            {
                _context.Institusjon.Remove(institusjon);
            }

            private void SlettBrukere(int institusjonId)
            {
                var brukereForInstitusjon = _context.Bruker.Where(b => b.Institusjon.Id == institusjonId);
                _context.Bruker.RemoveRange(brukereForInstitusjon);
            }

            private void SlettKlinikker(int institusjonsId)
            {
                var klinikker = _context.Klinikk.Where(k => k.Institusjon.Id == institusjonsId);
                _context.Klinikk.RemoveRange(klinikker);
            }

            private void SlettPredefinerteKommentarer(int institusjonsId)
            {
                var preDefinerteKommentarer = _context.PredefinertKommentar.Where(p => p.InstitusjonId == institusjonsId);
                _context.PredefinertKommentar.RemoveRange(preDefinerteKommentarer);
            }

            private void SlettAvdelingMedTilhørendeData(Domene.Sted.Institusjon institusjon)
            {
                foreach (var avdeling in institusjon.Avdelinger)
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
                var avdeling = _context.Avdeling.Find(avdelingId);
                _context.Avdeling.Remove(avdeling);
            }

            private void SlettFireIndikasjonerSesjonerOgObservasjoner(int avdelingId)
            {
                var sesjonerForAvdeling = _context.Sesjon.OfType<FireIndikasjonerSesjon>()
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observasjoner)
                    .ThenInclude(o=>o.Aktivitet)
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
                    .ThenInclude(o=>o.Beskyttelsesutstyrliste)
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
