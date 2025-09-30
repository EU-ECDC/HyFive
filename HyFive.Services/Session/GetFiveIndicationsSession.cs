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
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _userService = userService;
            }
            public async Task<FiveIndicationsSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.FiveIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Facility)
                    .Include(s => s.Observations).ThenInclude(o => o.Activity).ThenInclude(a => a.ActivityType)
                    .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (!_userService.HasEmailAndIsActive<ObserverUser>(request.Email).Compile()(session.Observer))
                    throw new Exception(
                        $"The session with ID {request.SessionId} is not associated with the logged-in user's email. {request.Email}");

                var fiveIndicationsSession = _mapper.Map<Domain.Session.FiveIndicationsSession, FiveIndicationsSession>(session);
                return fiveIndicationsSession;
            }
        }
    }
}
