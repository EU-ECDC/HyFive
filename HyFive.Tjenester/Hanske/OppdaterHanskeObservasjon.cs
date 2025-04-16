using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Services.Glove.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.Glove
{
    public class OppdaterHanskeObservasjon
    {
        public class Command : IRequest<bool>
        {
            public GloveObservation Observasjon { get; set; }
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
                var observasjon = await _context.GloveObservation
                    .Include(o => o.GloveSession)
                    .ThenInclude(s => s.TransmissionStatus)
                    .Include(o => o.HandhygieneEtterHanskebrukType)
                    .Include(o => o.IndicatedGloveTypes)
                    .Include(o => o.GeneralPurposeGloveTypes)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observasjon.Id), cancellationToken);

                if (observasjon == null)
                {
                    throw new Exception("O-H-01: Kunne ikke finne observasjon med ID " + request.Observasjon.Id);
                }

                if (observasjon.HanskeSesjon.TransmissionStatus?.Code == TransferStatusTypeConstants.TransferredToFhi)
                {
                    throw new Exception("O-H-02: Observasjonen er allerede overført til FHI, og kan ikke endres");
                }

                var observasjonFraRequest =
                    _mapper.Map<Domene.Observation.Gloves.GloveObservation>(request.Observasjon);
                HanskeObservasjonValidator.ValidateObservasjon(observasjonFraRequest);

                var hanskeMedIndikasjonTyper = _context.IndicatedGloveType.ToList();
                var hanskeUtenIndikasjonTyper = _context.GeneralPurposeGloveType.ToList();
                var handhygieneEtterHanskebrukTyper = _context.PostGloveHandHygiene.ToList();
                
                try
                {
                    observasjon.RegistrationTime = request.Observasjon.RegistrationTime;

                    observasjon.GloveUsed = observasjonFraRequest.GloveUsed;
                    observasjon.IndicatedGloveTypes = hanskeMedIndikasjonTyper
                        .Where(hmi => observasjonFraRequest.IndicatedGloveTypes.Select(ohmi => ohmi.Id).Contains(hmi.Id))
                        .ToList();
                    observasjon.GeneralPurposeGloveTypes = hanskeUtenIndikasjonTyper
                        .Where(hui => observasjonFraRequest.GeneralPurposeGloveTypes.Select(ohui => ohui.Id).Contains(hui.Id))
                        .ToList();
                    observasjon.HandhygieneEtterHanskebrukType = observasjonFraRequest.HandhygieneEtterHanskebrukType != null
                        ? handhygieneEtterHanskebrukTyper.FirstOrDefault(he => he.Id == observasjonFraRequest.HandhygieneEtterHanskebrukType.Id)
                        : null;
                    
                    var rolleFraRequest = _context.Role.FirstOrDefault(r => r.Id == request.Observasjon.Role.Id);
                    observasjon.Role = rolleFraRequest;
                    observasjon.Comment = request.Observasjon.Comment;

                    _context.Update(observasjon);

                    _context.SaveChanges();
                }
                catch (Exception e)
                {   
                    _logger.LogError(e,"O-H-03: Feil under oppdatering av Hanske-observasjon");
                    throw;
                }

                return true;
            }
        }
    }
}
