using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Common;
using HyFive.Services.Glove.Helpers;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloveSession = HyFive.Models.V1.Session.GloveSession;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.Glove
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public string Email { get; set; }
            public GloveSession Session { get; set; }
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
                var init = await InitializeSessionAsync<GloveSession, Domain.Session.GloveSession>(
                    request.Session,
                    request.Email,
                    request.Session.FacilityId,
                    request.Session.UnitId,
                    _userService,
                    _logger,
                    cancellationToken);

                if (init == null)
                    return _mapper.Map<Domain.Session.GloveSession>(request.Session).Id;

                var (session, unit) = init.Value;

                var gloveWithIndicationTypes = await _context.GloveWithIndicationType.ToListAsync(cancellationToken);
                var gloveWithoutIndicationTypes = await _context.GloveWithoutIndicationType.ToListAsync(cancellationToken);
                var handHygieneAfterGloveUseTypes = await _context.HandHygieneAfterGloveUseType.ToListAsync(cancellationToken);
                var ouRoles = GetDepartmentRoles(unit);

                foreach (var observation in session.Observations)
                {
                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.Role = observation.Role == null
                        ? null
                        : ouRoles.FirstOrDefault(r => r.Id == observation.Role.Id);

                    observation.GloveWithIndicationTypes = gloveWithIndicationTypes
                        .Where(x => observation.GloveWithIndicationTypes.Select(y => y.Id).Contains(x.Id))
                        .ToList();

                    observation.GloveWithoutIndicationTypes = gloveWithoutIndicationTypes
                        .Where(x => observation.GloveWithoutIndicationTypes.Select(y => y.Id).Contains(x.Id))
                        .ToList();

                    observation.PostGloveHandHygieneType = observation.PostGloveHandHygieneType != null
                        ? handHygieneAfterGloveUseTypes.FirstOrDefault(x => x.Id == observation.PostGloveHandHygieneType.Id)
                        : null;

                    GloveObservationValidator.ValidateObservation(observation);
                }

                session.TransferStatus = await GetTransferredToCoordinatorStatusAsync(cancellationToken);

                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                return session.Id;
            }
        }
    }
}