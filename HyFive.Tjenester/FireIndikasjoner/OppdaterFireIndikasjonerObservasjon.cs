using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
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
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var observasjon = await _context.FourIndicationsObservation
                    .Include(o => o.FourIndicationsSession)
                    .ThenInclude(s => s.TransmissionStatus)
                    .Include(o => o.IndicationTypes)
                    .Include(o => o.Activity)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observasjon.Id), cancellationToken);
                
                if (observasjon == null)
                {
                    throw new Exception("O-FI-01: Kunne ikke finne observasjon med ID " + request.Observasjon.Id);
                }
                if (observasjon.FourIndicationsSession.TransmissionStatus?.Code == OverforingstatusTypeKonstanter.OverfortTilFhi)
                {
                    throw new Exception("O-FI-02: Observasjonen er allerede overført til FHI, og kan ikke endres");
                }

                FireIndikasjonerObservasjonValidator.ValidateObservasjon(_mapper.Map<Domene.Observation.FourIndicationsObservation>(request.Observasjon));

                try
                {
                    var indikasjonstyperFraRequest = _context.IndicationTypes.Where(i => request.Observasjon.Indikasjonstyper.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observasjon.IndicationTypes = indikasjonstyperFraRequest;

                    var aktivitetTypeFraRequest = _context.ActivityType.FirstOrDefault(a => a.Code == request.Observasjon.Aktivitet.ActivityType.Code);
                    observasjon.Activity.ActivityType = aktivitetTypeFraRequest;
                    observasjon.Activity.GloveUsed = request.Observasjon.Aktivitet.GloveUsed;
                    observasjon.Activity.TimeSpent = request.Observasjon.Aktivitet.TimeSpent;
                    observasjon.Activity.TimeRecordingWasDone = request.Observasjon.Aktivitet.TimeRecordingWasDone;

                    observasjon.RegistrationTime = request.Observasjon.Registrerttidspunkt;

                    var rolleFraRequest = _context.Role.FirstOrDefault(r => r.Id == request.Observasjon.Rolle.Id);
                    observasjon.Role = rolleFraRequest;

                    observasjon.Comment = request.Observasjon.Kommentar;

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
