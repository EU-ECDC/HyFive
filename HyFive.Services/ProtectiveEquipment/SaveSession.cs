using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Common;
using HyFive.Services.Helpers;
using HyFive.Services.ProtectiveEquipment.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObserverUser = HyFive.Domain.User.User;
using ProtectiveEquipmentSession = HyFive.Models.V1.Session.ProtectiveEquipmentSession;

namespace HyFive.Services.ProtectiveEquipment
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public ProtectiveEquipmentSession Session { get; set; }
            public string Email { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Command, Guid>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IUserService _userService;

            public Handler(
                HandHygieneContext context,
                IMapper mapper,
                ILogger<Handler> logger,
                IUserService userService)
                : base(context, mapper)
            {
                _logger = logger;
                _userService = userService;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var init = await InitializeSessionAsync<ProtectiveEquipmentSession, Domain.Session.ProtectiveEquipmentSession>(
                    request.Session,
                    request.Email,
                    request.Session.FacilityId,
                    request.Session.UnitId,
                    _userService,
                    _logger,
                    cancellationToken);

                if (init == null)
                    return _mapper.Map<Domain.Session.ProtectiveEquipmentSession>(request.Session).Id;

                var (session, unit) = init.Value;

                var equipmentTypes = await _context.ProtectiveEquipmentType
                    .Include(x => x.MisuseTypes)
                    .ToListAsync(cancellationToken);

                var settingTypes = await _context.ProtectiveEquipmentSettingType.ToListAsync(cancellationToken);
                var ouRoles = GetDepartmentRoles(unit);

                foreach (var observation in session.Observations)
                {
                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.SettingType = settingTypes.First(x => x.Id == observation.SettingType.Id);
                    observation.Role = observation.Role == null
                        ? null
                        : ouRoles.FirstOrDefault(r => r.Id == observation.Role.Id);

                    foreach (var equipment in observation.ProtectiveEquipmentList)
                    {
                        equipment.EquipmentType = equipmentTypes.First(x => x.Id == equipment.EquipmentType.Id);

                        if (!equipment.WasUsedCorrectly && equipment.MisuseTypes.Any())
                        {
                            var misuseTypeIds = equipment.MisuseTypes.Select(x => x.Id);
                            equipment.MisuseTypes = equipment.EquipmentType.MisuseTypes
                                .Where(x => misuseTypeIds.Contains(x.Id))
                                .ToList();

                            equipment.Comment = string.IsNullOrWhiteSpace(equipment.Comment)
                                ? null
                                : equipment.Comment;
                        }
                    }

                    ProtectiveEquipmentObservationValidator.ValidateObservation(observation);
                }

                session.TransferStatus = await GetTransferredToCoordinatorStatusAsync(cancellationToken);

                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                return session.Id;
            }
        }
    }
}