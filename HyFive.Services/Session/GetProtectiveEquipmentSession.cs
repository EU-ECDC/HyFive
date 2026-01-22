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
    public class GetProtectiveEquipmentSession
    {
        public class Query : IRequest<ProtectiveEquipmentSession>
        {
            public Guid SessionId { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, ProtectiveEquipmentSession>
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

            public async Task<ProtectiveEquipmentSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.ProtectiveEquipmentSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Facility)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(o => o.EquipmentType)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(o => o.EquipmentType).ThenInclude(u => u.MisuseTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(o => o.MisuseTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId);
                
                if (!_userService.HasEmailAndIsActive<ObserverUser>(request.Email).Compile()(session.Observer))
                    throw new DomainException("SessionNotLinkedToUser", request.SessionId, request.Email);

                var protectiveEquipmentSession = _mapper.Map<ProtectiveEquipmentSession>(session);
                return protectiveEquipmentSession;
            }
        }
    }
}
