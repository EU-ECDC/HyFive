using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
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

        public class Handler : IRequestHandler<Query, HandJewelrySession>
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
            public async Task<HandJewelrySession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.HandJewelrySession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Facility)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (!_userService.HasEmailAndIsActive<ObserverUser>(request.Email).Compile()(session.Observer))
                    throw new DomainException("SessionNotLinkedToUser", request.SessionId, request.Email);

                var handmykkeSesjon = _mapper.Map<Domain.Session.HandJewelrySession, HandJewelrySession>(session);
                return handmykkeSesjon;
            }
        }
    }
}
