using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FiveIndicationsSession = HyFive.Models.V1.Session.FiveIndicationsSession;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.FiveIndication.Helpers;
using Microsoft.Extensions.Logging;
using HyFive.Domain.Observation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ObserverUser = HyFive.Domain.User.User;
using HyFive.Services.Helpers;

namespace HyFive.Services.FiveIndication
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public FiveIndicationsSession Session { get; set; }
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
            // NOSONAR: constructor duplicated across session handlers for consistency
            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                // Verify that the observer is an observer at the facility
                var observer = await SessionHelper.GetObserverAsync(_context, _userService, request.Email, request.Session.Department.FacilityId, cancellationToken);
                if (observer == null)
                    throw new ArgumentException(
                        $"Did not find an observer with email {request.Email} at facility with ID {request.Session.Department.FacilityId}");

                var indicationTypes = await _context.IndicationTypes.ToListAsync(cancellationToken);
                var activityTypes = await _context.ActivityType.ToListAsync(cancellationToken);

                var session = _mapper.Map<Domain.Session.FiveIndicationsSession>(request.Session);
                session.CreatedDate = DateTime.UtcNow;
                session.StartDate = DateTime.UtcNow;
                session.Department = await SessionHelper.GetDepartmentAsync(_context, request.Session.Department.Id, cancellationToken);

                // This is how we want to handle errors if we try to save a session with a department that no longer exists
                if (session.Department == null)
                {
                    _logger.LogWarning("Did not find department with ID {DepartmentId}", request.Session.Department.Id);
                    return session.Id;
                }

                session.Observer = observer;
                foreach (var observation in session.Observations)
                {
                    FiveIndicatorsObservationValidator.ValidateObservation(observation);
                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.Role = session.Department.Roles.FirstOrDefault(r => r.Id == observation.Role.Id);
                    observation.IndicationTypes = indicationTypes
                        .Where(i => observation.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observation.Activity.ActivityType = observation.Activity.ActivityType != null
                        ? activityTypes.FirstOrDefault(a => a.Id == observation.Activity.ActivityType.Id)
                        : null;
                }

                var transferStatuses = await _context.TransferStatusType.ToListAsync(cancellationToken);
                session.TransferStatus = transferStatuses.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);

                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);
                return session.Id;
            }
        }
    }
}