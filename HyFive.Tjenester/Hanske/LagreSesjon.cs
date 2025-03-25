using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Hanske.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HanskeSesjon = HyFive.Modeller.V1.Sesjon.HanskeSesjon;

namespace HyFive.Tjenester.Hanske
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
            public HanskeSesjon Sesjon { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IBrukerService _brukerService;

            public Handler(HandhygieneContext context, IMapper mapper, ILogger<Handler> logger, IBrukerService brukerService)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
                _brukerService = brukerService;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var observator = await HentObservator(request, cancellationToken);
                if (observator == null)
                    throw new Exception(
                        $"Fant ikke en observatør med HPR-nummer { request.HPRNummer } // pseudonym {request.Pseudonym} på institusjon med ID {request.Sesjon.Avdeling.InstitusjonId}");

                var hanskeMedIndikasjonTyper = _context.HanskeMedIndikasjonType.ToList();
                var hanskeUtenIndikasjonTyper = _context.HanskeUtenIndikasjonType.ToList();
                var handhygieneEtterHanskebrukTyper = _context.HandhygieneEtterHanskebrukType.ToList();

                var sesjon = _mapper.Map<Domene.Sesjon.HanskeSesjon>(request.Sesjon);
                sesjon.Opprettettidspunkt = DateTime.Now;
                sesjon.Avdeling = await HentAvdeling(request, cancellationToken);
                sesjon.Observator = observator;

                // Dette er måten vi ønsker å håndtere feil hvis vi prøver å lagre en sesjon med en avdeling som lenger ikke eksisterer
                if(sesjon.Avdeling == null)
                {
                    _logger.LogWarning($"Fant ikke avdeling med id: {request.Sesjon.Avdeling.Id}");
                    return sesjon.Id;
                }

                foreach (var observajon in sesjon.Observasjoner)
                {
                    observajon.Opprettettidspunkt = DateTime.Now;
                    observajon.Rolle = sesjon.Avdeling.Roller.FirstOrDefault(r => r.Id == observajon.Rolle.Id);
                    observajon.HanskeMedIndikasjonTyper = hanskeMedIndikasjonTyper
                                                            .Where(hmi => observajon.HanskeMedIndikasjonTyper.Select(ohmi => ohmi.Id).Contains(hmi.Id))
                                                            .ToList();
                    observajon.HanskeUtenIndikasjonTyper = hanskeUtenIndikasjonTyper
                                                            .Where(hui => observajon.HanskeUtenIndikasjonTyper.Select(ohui => ohui.Id).Contains(hui.Id))
                                                            .ToList();
                    observajon.HandhygieneEtterHanskebrukType = observajon.HandhygieneEtterHanskebrukType != null
                                                                ? handhygieneEtterHanskebrukTyper.FirstOrDefault(he => he.Id == observajon.HandhygieneEtterHanskebrukType.Id)
                                                                : null;
                    HanskeObservasjonValidator.ValidateObservasjon(observajon);
                }

                var overforingsstatuser = _context.OverforingstatusType.ToList();
                sesjon.Overforingstatus = overforingsstatuser.First(o => o.Kode == OverforingstatusTypeKonstanter.OverfortTilKoordinator);

                _context.Add(sesjon);
                _context.SaveChanges();

                return sesjon.Id;
            }

            private async Task<Domene.Sted.Avdeling> HentAvdeling(Command request, CancellationToken cancellationToken)
            {
                return await _context.Avdeling.Include(a => a.Roller)
                    .FirstOrDefaultAsync(a => a.Id == request.Sesjon.Avdeling.Id, cancellationToken);
            }

            private async Task<Observator> HentObservator(Command request, CancellationToken cancellationToken)
            {
                var institusjon = await _context.Institusjon
                    .Include(i => i.Brukere)
                    .FirstOrDefaultAsync(i => i.Id == request.Sesjon.Avdeling.InstitusjonId);

                if (institusjon == null)
                    throw new Exception(
                        $"Fant ikke oppgitt institusjon med id: {request.Sesjon.Avdeling.InstitusjonId}");

                return institusjon.Brukere.OfType<Observator>().Where(
                    _brukerService
                        .HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer,
                            request.Pseudonym).Compile()).FirstOrDefault();
            }
        }
    }
}