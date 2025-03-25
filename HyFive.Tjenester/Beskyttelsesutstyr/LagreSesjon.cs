using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeskyttelsesutstyrSesjon = HyFive.Modeller.V1.Sesjon.BeskyttelsesutstyrSesjon;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Beskyttelsesutstyr.Helpers;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public BeskyttelsesutstyrSesjon Sesjon { get; set; }
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
                    throw new Exception(
                        $"Fant ikke en observatør med HPR-nummer {request.HPRNummer} eller pseudonym XXX på institusjon med ID {request.Sesjon.Avdeling.InstitusjonId}");

                var sesjon = _mapper.Map<Domene.Sesjon.BeskyttelsesutstyrSesjon>(request.Sesjon);
                sesjon.Opprettettidspunkt = DateTime.Now;
                sesjon.Avdeling = await HentAvdeling(request, cancellationToken);
                sesjon.Observator = observator;

                // Dette er måten vi ønsker å håndtere feil hvis vi prøver å lagre en sesjon med en avdeling som lenger ikke eksisterer
                if (sesjon.Avdeling == null)
                {
                    _logger.LogWarning($"Fant ikke avdeling med id: {request.Sesjon.Avdeling.Id}");
                    return sesjon.Id;
                }

                var utstyrstyper = await _context.BeskyttelsesutstyrType.Include(bt => bt.Feilbruktyper)
                    .ToListAsync(cancellationToken);

                var settingtyper = await _context.BeskyttelsesutstyrsettingType.ToListAsync(cancellationToken);
                foreach (var observasjon in sesjon.Observasjoner)
                {
                    observasjon.Opprettettidspunkt = DateTime.Now;
                    observasjon.Settingtype = settingtyper.First(s => s.Id == observasjon.Settingtype.Id);
                    observasjon.Rolle = sesjon.Avdeling.Roller.FirstOrDefault(r => r.Id == observasjon.Rolle.Id);
                    foreach (var utstyr in observasjon.Beskyttelsesutstyrliste)
                    {
                        utstyr.Utstyrstype = utstyrstyper.First(u => u.Id == utstyr.Utstyrstype.Id);
                        if (utstyr.BleBenyttetRiktig == false && utstyr.Feilbruktyper.Any())
                        {
                            var feilbruktypeIder = utstyr.Feilbruktyper.Select(ft => ft.Id);
                            utstyr.Feilbruktyper = utstyr.Utstyrstype.Feilbruktyper
                                .Where(f => feilbruktypeIder.Contains(f.Id)).ToList();
                            utstyr.Kommentar = string.IsNullOrWhiteSpace(utstyr.Kommentar) ? null : utstyr.Kommentar;
                        }
                    }

                    BeskyttelsesutstyrObservasjonValidator.ValidateObservasjon(observasjon);
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

                return institusjon
                    .Brukere
                    .OfType<Observator>()
                    .FirstOrDefault(_brukerService
                        .HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer,request.Pseudonym)
                        .Compile());
            }
        }
    }
}