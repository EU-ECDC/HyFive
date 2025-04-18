using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
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

namespace HyFive.Services.ProtectiveEquipment
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public ProtectiveEquipmentSession Session { get; set; }
            public string HPRNumber { get; set; }
            public string Pseudonym { get; set; }
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
                // Verify that the observer is an observer at the institution
                var observer = await GetObserver(request, cancellationToken);
                if (observer == null)
                    throw new Exception(
                        $"Could not find an observer with HPR number {request.HPRNumber} or pseudonym XXX at the institution with ID {request.Session.Department.InstitutionId}");

                var session = _mapper.Map<Domain.Session.ProtectiveEquipmentSession>(request.Session);
                session.CreatedDate = DateTime.Now;
                session.Department = await GetDepartment(request, cancellationToken);
                session.Observer = observer;

                // This is the way we want to handle errors if we try to save a session with a department that no longer exists
                if (session.Department == null)
                {
                    _logger.LogWarning($"Could not find department with ID: {request.Session.Department.Id}");
                    return session.Id;
                }

                var equipmentTypes = await _context.ProtectiveEquipmentType.Include(bt => bt.MisuseTypes)
                    .ToListAsync(cancellationToken);

                var settingTypes = await _context.ProtectiveEquipmentSettingType.ToListAsync(cancellationToken);
                foreach (var observation in session.Observations)
                {
                    observation.CreatedTime = DateTime.Now;
                    observation.SettingType = settingTypes.First(s => s.Id == observation.SettingType.Id);
                    observation.Role = session.Department.Role.FirstOrDefault(r => r.Id == observation.Role.Id);
                    foreach (var equipment in observation.ProtectiveEquipmentList)
                    {
                        equipment.EquipmentType = equipmentTypes.First(u => u.Id == equipment.EquipmentType.Id);
                        if (equipment.WasUsedCorrectly == false && equipment.MisuseTypes.Any())
                        {
                            var misuseTypeIds = equipment.MisuseTypes.Select(ft => ft.Id);
                            equipment.MisuseTypes = equipment.EquipmentType.MisuseTypes
                                .Where(f => misuseTypeIds.Contains(f.Id)).ToList();
                            equipment.Comment = string.IsNullOrWhiteSpace(equipment.Comment) ? null : equipment.Comment;
                        }
                    }

                    ProtectiveEquipmentObservationValidator.ValidateObservasjon(observation);
                }

                var transferStatuses = _context.TransmissionStatusType.ToList();
                session.TransmissionStatus = transferStatuses.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);

                
                _context.Add(session);
                _context.SaveChanges();
                return session.Id;
            }

            private async Task<Domain.Place.Department> GetDepartment(Command request, CancellationToken cancellationToken)
            {
                return await _context.Department.Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.Id == request.Session.Department.Id, cancellationToken);
            }


            private async Task<Observer> GetObserver(Command request, CancellationToken cancellationToken)
            {
                var institution = await _context.Institution
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.Id == request.Session.Department.InstitutionId);

                if (institution == null)
                    throw new Exception(
                        $"Could not find the specified institution with ID: {request.Session.Department.InstitutionId}");

                return institution
                    .Users
                    .OfType<Observer>()
                    .FirstOrDefault(_userService
                        .HasHprOrPseudonymAndIsActive<Observer>(request.HPRNumber,request.Pseudonym)
                        .Compile());
            }
        }
    }
}