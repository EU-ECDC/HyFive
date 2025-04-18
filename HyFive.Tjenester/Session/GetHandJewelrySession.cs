using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Session
{
    public class GetHandJewelrySession
    {
        public class Query : IRequest<HandJewelrySession>
        {
            public string HPRNumber { get; set; }
            public string Pseudonym { get; set; }
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
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institution)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelry)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (!_userService.HasHprOrPseudonymAndIsActive<Observer>(request.HPRNumber, request.Pseudonym).Compile()(session.Observer))
                    throw new Exception(
                        $"The session with ID {{request.SessionId}} is not associated with a user with HPR number {request.HPRNumber}");

                var handmykkeSesjon = _mapper.Map<Domain.Session.HandJewelrySession, HandJewelrySession>(session);
                return handmykkeSesjon;
            }
        }
    }
}
