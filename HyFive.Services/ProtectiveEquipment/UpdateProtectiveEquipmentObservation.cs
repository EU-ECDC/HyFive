using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Services.ProtectiveEquipment.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.ProtectiveEquipment
{
    public class UpdateProtectiveEquipmentObservation
    {
        public class Command : IRequest<bool>
        {
            public ProtectiveEquipmentObservation Observation { get; set; }
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

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var observation = await _context.ProtectiveEquipmentObservation
                    .Include(o => o.ProtectiveEquipmentSession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.ProtectiveEquipmentList)
                    .ThenInclude(o => o.MisuseTypes)
                    .Include(o => o.ProtectiveEquipmentList)
                    .ThenInclude(o => o.EquipmentType)
                    .Include(o => o.SettingType)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);

                if (observation == null)
                {
                    throw new DomainException("ObservationNotFound", request.Observation.Id);
                }

                if (observation.ProtectiveEquipmentSession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToAdmin)
                {
                    throw new DomainException("ObservationAlreadyTransferred");
                }

                var observationFromRequest =
                    _mapper.Map<Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(request.Observation);

                ProtectiveEquipmentObservationValidator.ValidateObservation(observationFromRequest);
                
                
                observation.RegisteredTime = request.Observation.RegisteredTime;

                var equipmentTypes = await _context.ProtectiveEquipmentType.Include(bt => bt.MisuseTypes)
                    .ToListAsync(cancellationToken);

                var settingType = await _context.ProtectiveEquipmentSettingType.ToListAsync(cancellationToken);
                observation.SettingType = settingType.First(s => s.Id == observationFromRequest.SettingType.Id);
                foreach (var equipment in observation.ProtectiveEquipmentList)
                {
                    var equipmentFromRequest =
                        observationFromRequest.ProtectiveEquipmentList.First(u => u.Id == equipment.Id);
                        
                       equipment.IsRequired = equipmentFromRequest.IsRequired; 
                       equipment.EquipmentType = equipmentTypes.First(u => u.Id == equipmentFromRequest.EquipmentType.Id);
                       equipment.WasUsed = equipmentFromRequest.WasUsed;
                       equipment.WasUsedCorrectly = equipmentFromRequest.WasUsedCorrectly;
                       equipment.Comment = string.IsNullOrWhiteSpace(equipmentFromRequest.Comment) ? null : equipmentFromRequest.Comment;
                       var misuseTypeIDsFromRequest = equipmentFromRequest.MisuseTypes.Select(ft => ft.Id);
                       var misuseTypesFromRequest =
                           await _context.MisuseType.Where(f => misuseTypeIDsFromRequest.Contains(f.Id)).ToListAsync(cancellationToken);

                    equipment.MisuseTypes = misuseTypesFromRequest;
                       
                    _context.Entry(equipment).State = EntityState.Modified;
                }

                    var roleFromRequest = await _context.Role.FirstOrDefaultAsync(r => r.Id == observationFromRequest.Role.Id, cancellationToken);
                    observation.Role = roleFromRequest;
                    observation.Comment = observationFromRequest.Comment;

                _context.UpdateRange(observation.ProtectiveEquipmentList);
                _context.Update(observation);
                await _context.SaveChangesAsync(cancellationToken);
                

                return true;
            }
        }
    }
}