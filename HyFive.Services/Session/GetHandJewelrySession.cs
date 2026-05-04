using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Common;
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
    public class GetHandJewelrySession
    {
        public class Query : IRequest<HandJewelrySession>
        {
            public string Email { get; set; }
            public Guid SessionId { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Query, HandJewelrySession>
        {
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
                : base(context, mapper)
            {
                _userService = userService;
            }
            public async Task<HandJewelrySession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.HandJewelrySession
                .AsNoTracking()
                .Include(s => s.OrganisationUnit)
                    .ThenInclude(ou => ou.Parent)
                        .ThenInclude(p => p.Parent)
                .Include(s => s.OrganisationUnit)
                    .ThenInclude(ou => ou.OrganisationUnitRoles)
                        .ThenInclude(r => r.Role)
                .Include(s => s.Observer)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.HandJewelries)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.Role)
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (session == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                await ValidateObserverAccessAsync(session, request.Email, cancellationToken);

                var handJewelrySession = _mapper.Map<Domain.Session.HandJewelrySession, HandJewelrySession>(session);
                return handJewelrySession;
            }
        }
    }
}
