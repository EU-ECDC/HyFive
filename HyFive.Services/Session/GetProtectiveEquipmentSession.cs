using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
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
    public class GetProtectiveEquipmentSession
    {
        public class Query : IRequest<ProtectiveEquipmentSession>
        {
            public Guid SessionId { get; set; }
            public string Email { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Query, ProtectiveEquipmentSession>
        {
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
                : base(context, mapper)
            {
                _userService = userService;
            }

            public async Task<ProtectiveEquipmentSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.ProtectiveEquipmentSession
                .AsNoTracking()
                .Include(s => s.OrganisationUnit)
                .Include(s => s.Observer)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.ProtectiveEquipmentList)
                        .ThenInclude(pe => pe.EquipmentType)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.ProtectiveEquipmentList)
                        .ThenInclude(pe => pe.MisuseTypes)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.SettingType)
                .Include(s => s.Observations)
                    .ThenInclude(o => o.Role)
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (session == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                await ValidateObserverAccessAsync(session, request.Email, cancellationToken);

                var protectiveEquipmentSession = _mapper.Map<ProtectiveEquipmentSession>(session);
                return protectiveEquipmentSession;
            }
        }
    }
}
