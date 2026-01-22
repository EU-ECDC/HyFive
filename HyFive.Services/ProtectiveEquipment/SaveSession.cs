using AutoMapper;
using HyFive.DataAccess;
using ObserverUser = HyFive.Domain.User.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProtectiveEquipmentSession = HyFive.Models.V1.Session.ProtectiveEquipmentSession;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.ProtectiveEquipment.Helpers;
using Microsoft.Extensions.Logging;
using HyFive.Domain.Exceptions;
using HyFive.Services.Helpers;

namespace HyFive.Services.ProtectiveEquipment
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public ProtectiveEquipmentSession Session { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
                _userService = userService;
            }
            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                // Verify that the observer is an observer at the facility
                var observer = await SessionHelper.GetObserverAsync(_context, _userService, request.Email, request.Session.Department.FacilityId, cancellationToken);
                if (observer == null)
                    throw new DomainException("ObserverNotFoundAtFacility", request.Email, request.Session.Department.FacilityId);

                var session = _mapper.Map<Domain.Session.ProtectiveEquipmentSession>(request.Session);
                session.CreatedDate = DateTime.UtcNow;
                session.StartDate = DateTime.UtcNow;
                session.Department = await SessionHelper.GetDepartmentAsync(_context, request.Session.Department.Id, cancellationToken);
                session.Observer = observer;

                // This is the way we want to handle errors if we try to save a session with a department that no longer exists
                if (session.Department == null)
                {
                    _logger.LogWarning("Did not find department with ID: {DepartmentId}", request.Session.Department.Id);
                    return session.Id;
                }

                var equipmentTypes = await _context.ProtectiveEquipmentType.Include(bt => bt.MisuseTypes)
                    .ToListAsync(cancellationToken);

                var settingTypes = await _context.ProtectiveEquipmentSettingType.ToListAsync(cancellationToken);
                foreach (var observation in session.Observations)
                {
                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.SettingType = settingTypes.First(s => s.Id == observation.SettingType.Id);
                    observation.Role = session.Department.Roles.FirstOrDefault(r => r.Id == observation.Role.Id);
                    foreach (var equipment in observation.ProtectiveEquipmentList)
                    {
                        equipment.EquipmentType = equipmentTypes.First(u => u.Id == equipment.EquipmentType.Id);
                        if (!equipment.WasUsedCorrectly && equipment.MisuseTypes.Any())
                        {
                            var misuseTypeIds = equipment.MisuseTypes.Select(ft => ft.Id);
                            equipment.MisuseTypes = equipment.EquipmentType.MisuseTypes
                                .Where(f => misuseTypeIds.Contains(f.Id)).ToList();
                            equipment.Comment = string.IsNullOrWhiteSpace(equipment.Comment) ? null : equipment.Comment;
                        }
                    }

                    ProtectiveEquipmentObservationValidator.ValidateObservation(observation);
                }

                var transferStatuses = await _context.TransferStatusType.ToListAsync(cancellationToken);
                session.TransferStatus = transferStatuses.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator); // NOSONAR
                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken); 
                return session.Id;
            }
        }
    }
}