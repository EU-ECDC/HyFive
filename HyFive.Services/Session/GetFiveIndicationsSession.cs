using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.Session
{
    public class GetFiveIndicationsSession
    {
        public class Query : IRequest<FiveIndicationsSession>
        {
            public string Email { get; set; }
            public Guid SessionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FiveIndicationsSession>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<FiveIndicationsSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.FiveIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.OrganisationUnit)
                    .ThenInclude(ou => ou.Parent)
                        .ThenInclude(p => p.Parent)
                    .ThenInclude(ou => ou.OrganisationUnitRoles)
                    .ThenInclude(r => r.Role)
                    .Include(s => s.Observer)
                    .Include(s => s.Observations).ThenInclude(o => o.Activity).ThenInclude(a => a.ActivityType)
                    .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (session == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                if (!string.Equals(session.Observer.Email, request.Email, StringComparison.OrdinalIgnoreCase)
                    || session.Observer.IsDeactivated)
                {
                    throw new DomainException("SessionNotOwnedByUser");
                }

                // 3) Ensure the observer has permission for the OU

                var facilityId = await PermissionHelper.GetFacilityIdForUnitAsync(
                    _context,
                    session.OrganisationUnitId,
                    cancellationToken);

                var hasAccessToOu = await PermissionHelper.HasObserverPermissionForFacilityAsync(
                    _context,
                    session.ObserverId.Value,
                    facilityId.Value,
                    cancellationToken);

                if (!hasAccessToOu)
                    throw new DomainException("FacilityAccessDenied");

                var fiveIndicationsSession = _mapper.Map<Domain.Session.FiveIndicationsSession, FiveIndicationsSession>(session);
                return fiveIndicationsSession;
            }
        }
    }
}
