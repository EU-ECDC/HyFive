using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using ObserverUser = HyFive.Domain.User.User;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Session
{
    public class GetGloveSession
    {
        public class Query : IRequest<GloveSession>
        {
            public string Email { get; set; }
            public Guid SessionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, GloveSession>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _userService = userService;
            }

            public async Task<GloveSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.GloveSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(r => r.Facility)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.GloveWithIndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.GloveWithoutIndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.PostGloveHandHygieneType)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (!_userService.HasEmailAndIsActive<ObserverUser>(request.Email).Compile()(session.Observer))
                    throw new Exception(
                        $"The session with ID {request.SessionId} is not linked to the user with email {request.Email}");

                var gloveSession = _mapper.Map<GloveSession>(session);

                return gloveSession;
            }
        }
    }
}