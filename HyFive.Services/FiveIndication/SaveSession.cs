using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Common;
using HyFive.Services.FiveIndication.Helpers;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FiveIndicationsSession = HyFive.Models.V1.Session.FiveIndicationsSession;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.FiveIndication
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public FiveIndicationsSession Session { get; set; }
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
                var init = await InitializeSessionAsync<FiveIndicationsSession, Domain.Session.FiveIndicationsSession>(
                    request.Session,
                    request.Email,
                    request.Session.FacilityId,
                    request.Session.UnitId,
                    _userService,
                    _logger,
                    cancellationToken);

                if (init == null)
                    return _mapper.Map<Domain.Session.FiveIndicationsSession>(request.Session).Id;

                var (session, unit) = init.Value;

                var indicationTypes = await _context.IndicationTypes.ToListAsync(cancellationToken);
                var activityTypes = await _context.ActivityType.ToListAsync(cancellationToken);
                var ouRoles = GetDepartmentRoles(unit);

                foreach (var observation in session.Observations)
                {
                    var matchedRole = observation.Role == null
                        ? null
                        : ouRoles.FirstOrDefault(r => r.Id == observation.Role.Id);

                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.Role = matchedRole;

                    if (observation.Activity != null)
                    {
                        observation.Activity.ActivityType = activityTypes
                            .FirstOrDefault(a => a.Id == observation.Activity.ActivityType.Id);
                    }

                    observation.IndicationTypes = indicationTypes
                        .Where(i => observation.IndicationTypes.Select(x => x.Id).Contains(i.Id))
                        .ToList();

                    FiveIndicatorsObservationValidator.ValidateObservation(observation);
                }

                session.TransferStatus = await GetTransferredToCoordinatorStatusAsync(cancellationToken);

                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                return session.Id;
            }
        }
    }
}