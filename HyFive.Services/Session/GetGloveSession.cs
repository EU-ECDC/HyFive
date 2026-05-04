using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using HyFive.Services.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.Session
{
    public class GetGloveSession
    {
        public class Query : IRequest<GloveSession>
        {
            public string Email { get; set; }
            public Guid SessionId { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Query, GloveSession>
        {
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
                : base(context, mapper)
            {
                _userService = userService;
            }

            public async Task<GloveSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.GloveSession
                .AsNoTracking()
                .Include(s => s.OrganisationUnit)
                .ThenInclude(ou => ou.Parent)
                        .ThenInclude(p => p.Parent)
                .Include(s => s.Observer)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.Role)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.GloveWithIndicationTypes)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.GloveWithoutIndicationTypes)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.PostGloveHandHygieneType)
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (session == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                await ValidateObserverAccessAsync(session, request.Email, cancellationToken);

                return _mapper.Map<GloveSession>(session);
            }
        }
    }
}