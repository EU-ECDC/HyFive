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
    public class GetProtectiveEquipmentSession
    {
        public class Query : IRequest<ProtectiveEquipmentSession>
        {
            public Guid SessionId { get; set; }
            public string HPRNumber { get; set; }
            public string Pseudonym { get; set; }
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
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institution)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(o => o.EquipmentType)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(o => o.EquipmentType).ThenInclude(u => u.MisuseTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(o => o.MisuseTypes)
                    .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId);
                
                if (!_userService.HasHprOrPseudonymAndIsActive<Observer>(request.HPRNumber, request.Pseudonym).Compile()(session.Observer))
                    throw new Exception(
                        $"he session with ID {request.SessionId} is not associated with the user with HPR number {request.HPRNumber} / Pseudonym XXX");
                
                var protectiveEquipmentSession = _mapper.Map<ProtectiveEquipmentSession>(session);
                return protectiveEquipmentSession;
            }
        }
    }
}
