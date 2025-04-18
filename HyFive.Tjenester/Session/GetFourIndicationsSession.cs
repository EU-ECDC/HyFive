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
    public class GetFourIndicationsSession
    {
        public class Query : IRequest<FourIndicationsSession>
        {
            public string HPRNumber { get; set; }
            public string Pseudonym { get; set; }
            public Guid SessionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FourIndicationsSession>
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
            public async Task<FourIndicationsSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.FourIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institution)
                    .Include(s => s.Observations).ThenInclude(o => o.Activity).ThenInclude(a => a.ActivityType)
                    .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (!_userService.HasHprOrPseudonymAndIsActive<Observer>(request.HPRNumber, request.Pseudonym).Compile()(session.Observer))
                    throw new Exception(
                        $"The session with ID {request.SessionId} is not associated with the logged-in user's pseudonym or HPR number. {request.HPRNumber}");

                var fourIndicationsSession = _mapper.Map<Domain.Session.FourIndicationsSession, FourIndicationsSession>(session);
                return fourIndicationsSession;
            }
        }
    }
}
