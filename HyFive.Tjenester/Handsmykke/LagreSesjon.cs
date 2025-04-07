using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Bruker;
using Microsoft.EntityFrameworkCore;
using HandsmykkeSesjon = HyFive.Modeller.V1.Sesjon.HandsmykkeSesjon;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Tjenester.Autentisering.Bruker;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.Handsmykke
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public HandsmykkeSesjon Sesjon { get; set; }
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
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
                // Verifisere at observatør er observatør på institusjon
                var observator = await HentObservator(request, cancellationToken);
                if (observator == null)
                    throw new Exception($"Fant ikke en observatør med HPR-nummer {request.HPRNummer} på institusjon med ID {request.Sesjon.Avdeling.InstitusjonId}");

                var handsmykketyper = _context.HandsmykkeType.ToList();
                var sesjon = _mapper.Map<Domene.Sesjon.HandsmykkeSesjon>(request.Sesjon);
                sesjon.Opprettettidspunkt = DateTime.UtcNow;
                sesjon.Avdeling = await HentAvdeling(request, cancellationToken);

                // Dette er måten vi ønsker å håndtere feil hvis vi prøver å lagre en sesjon med en avdeling som lenger ikke eksisterer
                if (sesjon.Avdeling == null)
                {
                    _logger.LogWarning($"Fant ikke avdeling med id: {request.Sesjon.Avdeling.Id}");
                    return sesjon.Id;
                }
                    

                sesjon.Observator = observator;
                foreach (var observasjon in sesjon.Observasjoner)
                {
                    observasjon.Opprettettidspunkt = DateTime.UtcNow;
                    observasjon.Rolle = sesjon.Avdeling.Roller.FirstOrDefault(r => r.Id == observasjon.Rolle.Id);
                    observasjon.Handsmykker = handsmykketyper.Where(ht => observasjon.Handsmykker.Select(oh => oh.Id).Contains(ht.Id)).ToList();
                    observasjon.Kommentar = string.IsNullOrEmpty(observasjon.Kommentar) ? null : observasjon.Kommentar;
                }

                var overforingsstatuser = _context.OverforingstatusType.ToList();
                sesjon.Overforingstatus = overforingsstatuser.First(o => o.Kode == OverforingstatusTypeKonstanter.OverfortTilKoordinator);

                _context.Add(sesjon);
                _context.SaveChanges();
                return sesjon.Id;
            }

            private async Task<Domene.Sted.Avdeling> HentAvdeling(Command request, CancellationToken cancellationToken)
            {
                return await _context.Avdeling.Include(a => a.Roller).FirstOrDefaultAsync(a => a.Id == request.Sesjon.Avdeling.Id, cancellationToken);
            }

            private async Task<Observator> HentObservator(Command request, CancellationToken cancellationToken)
            {
                var institusjon = await _context.Institusjon
                    .Include(i => i.Brukere)
                    .FirstOrDefaultAsync(i => i.Id == request.Sesjon.Avdeling.InstitusjonId);

                if (institusjon == null)
                    throw new Exception($"Fant ikke oppgitt institusjon med id: {request.Sesjon.Avdeling.InstitusjonId}");

                return institusjon
                    .Brukere
                    .OfType<Observator>()
                    .FirstOrDefault(_brukerService.HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer, request.Pseudonym).Compile());
            }
        }
    }
}
