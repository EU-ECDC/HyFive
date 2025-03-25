using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using HyFive.Tjenester.Beskyttelsesutstyr.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class OppdaterBeskyttelsesutstyrObservasjon
    {
        public class Command : IRequest<bool>
        {
            public BeskyttelsesutstyrObservasjon Observasjon { get; set; }
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
                var observasjon = await _context.BeskyttelsesutstyrObservasjon
                    .Include(o => o.BeskyttelsesutstyrSesjon)
                    .ThenInclude(s => s.Overforingstatus)
                    .Include(o => o.Beskyttelsesutstyrliste)
                    .ThenInclude(o => o.Feilbruktyper)
                    .Include(o => o.Beskyttelsesutstyrliste)
                    .ThenInclude(o => o.Utstyrstype)
                    .Include(o => o.Settingtype)
                    .Include(o => o.Rolle)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observasjon.Id), cancellationToken);

                if (observasjon == null)
                {
                    throw new Exception("O-BU-01: Kunne ikke finne observasjon med ID " + request.Observasjon.Id);
                }

                if (observasjon.BeskyttelsesutstyrSesjon.Overforingstatus?.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi)
                {
                    throw new Exception("O-BU-02: Observasjonen er allerede overført til FHI, og kan ikke endres");
                }

                var observasjonFraRequest =
                    _mapper.Map<Domene.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrObservasjon>(request.Observasjon);

                BeskyttelsesutstyrObservasjonValidator.ValidateObservasjon(observasjonFraRequest);
                
                try
                {
                    observasjon.Registrerttidspunkt = request.Observasjon.Registrerttidspunkt;

                    var utstyrstyper = await _context.BeskyttelsesutstyrType.Include(bt => bt.Feilbruktyper)
                        .ToListAsync(cancellationToken);

                    var settingtyper = await _context.BeskyttelsesutstyrsettingType.ToListAsync(cancellationToken);
                    observasjon.Settingtype = settingtyper.First(s => s.Id == observasjonFraRequest.Settingtype.Id);
                    foreach (var utstyr in observasjon.Beskyttelsesutstyrliste)
                    {
                        var utstyrFraRequest =
                            observasjonFraRequest.Beskyttelsesutstyrliste.First(u => u.Id == utstyr.Id);
                        
                       utstyr.ErIndikert = utstyrFraRequest.ErIndikert; 
                       utstyr.Utstyrstype = utstyrstyper.First(u => u.Id == utstyrFraRequest.Utstyrstype.Id);
                       utstyr.BleBenyttet = utstyrFraRequest.BleBenyttet;
                       utstyr.BleBenyttetRiktig = utstyrFraRequest.BleBenyttetRiktig;
                       utstyr.Kommentar = string.IsNullOrWhiteSpace(utstyrFraRequest.Kommentar) ? null : utstyrFraRequest.Kommentar;
                       var feilbruktypeIderFraRequest = utstyrFraRequest.Feilbruktyper.Select(ft => ft.Id);
                       var feilbruktyperFraRequest =
                           _context.FeilbrukType.Where(f => feilbruktypeIderFraRequest.Contains(f.Id)).ToList();

                       utstyr.Feilbruktyper = feilbruktyperFraRequest;
                       
                       _context.Entry(utstyr).State = EntityState.Modified;
                    }

                    var rolleFraRequest = _context.Rolle.FirstOrDefault(r => r.Id == observasjonFraRequest.Rolle.Id);
                    observasjon.Rolle = rolleFraRequest;
                    observasjon.Kommentar = observasjonFraRequest.Kommentar;

                    _context.UpdateRange(observasjon.Beskyttelsesutstyrliste);
                    _context.Update(observasjon);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {   
                    _logger.LogError(e,"O-BU-03: Feil under oppdatering av Beskyttelsesutstyr-observasjon");
                    throw;
                }

                return true;
            }
        }
    }
}