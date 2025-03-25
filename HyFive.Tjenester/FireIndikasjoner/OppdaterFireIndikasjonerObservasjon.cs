using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Tjenester.FireIndikasjoner.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FireIndikasjonerObservasjon = HyFive.Modeller.V1.Observasjon.FireIndikasjonerObservasjon;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class OppdaterFireIndikasjonerObservasjon
    {
        public class Command : IRequest<bool>
        {
            public FireIndikasjonerObservasjon Observasjon { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(HandhygieneContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var observasjon = await _context.FireIndikasjonerObservasjon
                    .Include(o => o.FireIndikasjonerSesjon)
                    .ThenInclude(s => s.Overforingstatus)
                    .Include(o => o.Indikasjonstyper)
                    .Include(o => o.Aktivitet)
                    .Include(o => o.Rolle)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observasjon.Id), cancellationToken);
                
                if (observasjon == null)
                {
                    throw new Exception("O-FI-01: Kunne ikke finne observasjon med ID " + request.Observasjon.Id);
                }
                if (observasjon.FireIndikasjonerSesjon.Overforingstatus?.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi)
                {
                    throw new Exception("O-FI-02: Observasjonen er allerede overført til FHI, og kan ikke endres");
                }

                FireIndikasjonerObservasjonValidator.ValidateObservasjon(_mapper.Map<Domene.Observasjon.FireIndikasjonerObservasjon>(request.Observasjon));

                try
                {
                    var indikasjonstyperFraRequest = _context.Indikasjon.Where(i => request.Observasjon.Indikasjonstyper.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observasjon.Indikasjonstyper = indikasjonstyperFraRequest;

                    var aktivitetTypeFraRequest = _context.AktivitetType.FirstOrDefault(a => a.Kode == request.Observasjon.Aktivitet.AktivitetType.Kode);
                    observasjon.Aktivitet.AktivitetType = aktivitetTypeFraRequest;
                    observasjon.Aktivitet.BenyttetHanske = request.Observasjon.Aktivitet.BenyttetHanske;
                    observasjon.Aktivitet.SekunderBrukt = request.Observasjon.Aktivitet.SekunderBrukt;
                    observasjon.Aktivitet.TidtakingBleUtfort = request.Observasjon.Aktivitet.TidtakingBleUtfort;

                    observasjon.Registrerttidspunkt = request.Observasjon.Registrerttidspunkt;

                    var rolleFraRequest = _context.Rolle.FirstOrDefault(r => r.Id == request.Observasjon.Rolle.Id);
                    observasjon.Rolle = rolleFraRequest;

                    observasjon.Kommentar = request.Observasjon.Kommentar;

                    _context.Update(observasjon);

                    _context.SaveChanges();
                }
                catch (Exception e)
                {   
                    _logger.LogError(e,"O-FI-03: Feil under oppdatering av Fire Indikasjoner-observasjon");
                    throw;
                }

                return true;
            }
        }
    }
}
