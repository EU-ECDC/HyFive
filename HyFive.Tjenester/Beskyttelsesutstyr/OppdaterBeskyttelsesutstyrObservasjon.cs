using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Services.Beskyttelsesutstyr.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.Beskyttelsesutstyr
{
    public class OppdaterBeskyttelsesutstyrObservasjon
    {
        public class Command : IRequest<bool>
        {
            public ProtectiveEquipmentObservation Observasjon { get; set; }
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
                var observasjon = await _context.ProtectiveEquipmentObservation
                    .Include(o => o.ProtectiveEquipmentSession)
                    .ThenInclude(s => s.TransmissionStatus)
                    .Include(o => o.ProtectiveEquipmentList)
                    .ThenInclude(o => o.MisuseTypes)
                    .Include(o => o.ProtectiveEquipmentList)
                    .ThenInclude(o => o.EquipmentType)
                    .Include(o => o.SettingType)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observasjon.Id), cancellationToken);

                if (observasjon == null)
                {
                    throw new Exception("O-BU-01: Kunne ikke finne observasjon med ID " + request.Observasjon.Id);
                }

                if (observasjon.ProtectiveEquipmentSession.TransmissionStatus?.Code == TransferStatusTypeConstants.TransferredToFhi)
                {
                    throw new Exception("O-BU-02: Observasjonen er allerede overført til FHI, og kan ikke endres");
                }

                var observasjonFraRequest =
                    _mapper.Map<Domene.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(request.Observasjon);

                BeskyttelsesutstyrObservasjonValidator.ValidateObservasjon(observasjonFraRequest);
                
                try
                {
                    observasjon.RegistrationTime = request.Observasjon.RegistrationTime;

                    var utstyrstyper = await _context.ProtectiveEquipmentType.Include(bt => bt.MisuseTypes)
                        .ToListAsync(cancellationToken);

                    var settingtyper = await _context.ProtectiveEquipmentSettingType.ToListAsync(cancellationToken);
                    observasjon.SettingType = settingtyper.First(s => s.Id == observasjonFraRequest.SettingType.Id);
                    foreach (var utstyr in observasjon.ProtectiveEquipmentList)
                    {
                        var utstyrFraRequest =
                            observasjonFraRequest.ProtectiveEquipmentList.First(u => u.Id == utstyr.Id);
                        
                       utstyr.IsRequired = utstyrFraRequest.IsRequired; 
                       utstyr.EquipmentType = utstyrstyper.First(u => u.Id == utstyrFraRequest.EquipmentType.Id);
                       utstyr.WasUsed = utstyrFraRequest.WasUsed;
                       utstyr.WasUsedCorrectly = utstyrFraRequest.WasUsedCorrectly;
                       utstyr.Comment = string.IsNullOrWhiteSpace(utstyrFraRequest.Comment) ? null : utstyrFraRequest.Comment;
                       var feilbruktypeIderFraRequest = utstyrFraRequest.MisuseTypes.Select(ft => ft.Id);
                       var feilbruktyperFraRequest =
                           _context.MisuseType.Where(f => feilbruktypeIderFraRequest.Contains(f.Id)).ToList();

                       utstyr.MisuseTypes = feilbruktyperFraRequest;
                       
                       _context.Entry(utstyr).State = EntityState.Modified;
                    }

                    var rolleFraRequest = _context.Role.FirstOrDefault(r => r.Id == observasjonFraRequest.Role.Id);
                    observasjon.Role = rolleFraRequest;
                    observasjon.Comment = observasjonFraRequest.Comment;

                    _context.UpdateRange(observasjon.ProtectiveEquipmentList);
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