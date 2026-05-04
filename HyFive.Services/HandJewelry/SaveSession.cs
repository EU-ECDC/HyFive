using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Common;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandJewelrySession = HyFive.Models.V1.Session.HandJewelrySession;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.HandJewelry
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public HandJewelrySession Session { get; set; }
            public string Email { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Command, Guid>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger, IUserService userService) : base(context, mapper)
            {
                _logger = logger;
                _userService = userService;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var init = await InitializeSessionAsync<HandJewelrySession, Domain.Session.HandJewelrySession>(
            request.Session,
            request.Email,
            request.Session.FacilityId,
            request.Session.UnitId,
            _userService,
            _logger,
            cancellationToken);

                if (init == null)
                    return _mapper.Map<Domain.Session.HandJewelrySession>(request.Session).Id;

                var (session, unit) = init.Value;
                var ouRoles = GetDepartmentRoles(unit);
                var handJewelryTypes = await _context.HandJewelryType.ToListAsync(cancellationToken);

                foreach (var observation in session.Observations)
                {
                    var matchedRole = observation.Role == null
                        ? null
                        : ouRoles.FirstOrDefault(r => r.Id == observation.Role.Id);

                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.Role = matchedRole;
                    observation.HandJewelries = handJewelryTypes
                        .Where(ht => observation.HandJewelries.Select(oh => oh.Id).Contains(ht.Id))
                        .ToList();
                    observation.Comment = string.IsNullOrEmpty(observation.Comment) ? null : observation.Comment;
                }

                session.TransferStatus = await GetTransferredToCoordinatorStatusAsync(cancellationToken);

                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);
                return session.Id;
            }
        }
    }
}
