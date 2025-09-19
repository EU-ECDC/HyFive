using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domain.User;
using Microsoft.EntityFrameworkCore;
using HandJewelrySession = HyFive.Models.V1.Session.HandJewelrySession;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.HandJewelry
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public HandJewelrySession Session { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
                _userService = userService;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                // Verify that observer is an observer at the facility
                var observer = await GetObserver(request, cancellationToken);
                if (observer == null)
                    throw new Exception($"Did not find an observer with Email {request.Email} at facility with ID {request.Session.Department.FacilityId}");

                var handJewelryTypes = _context.HandJewelryType.ToList();
                var session = _mapper.Map<Domain.Session.HandJewelrySession>(request.Session);
                session.CreatedDate = DateTime.UtcNow;
                session.StartDate = DateTime.UtcNow;
                session.Department = await GetDepartment(request, cancellationToken);

                // This is the way we want to handle the error if we try to save a session with a department that no longer exists
                if (session.Department == null)
                {
                    _logger.LogWarning($"Did not find department with ID: {request.Session.Department.Id}");
                    return session.Id;
                }
                    

                session.Observer = observer;
                foreach (var observation in session.Observations)
                {
                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.Role = session.Department.Roles.FirstOrDefault(r => r.Id == observation.Role.Id);
                    observation.HandJewelries = handJewelryTypes.Where(ht => observation.HandJewelries.Select(oh => oh.Id).Contains(ht.Id)).ToList();
                    observation.Comment = string.IsNullOrEmpty(observation.Comment) ? null : observation.Comment;
                }

                var transferStatuses = _context.TransferStatusType.ToList();
                session.TransferStatus = transferStatuses.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);

                _context.Add(session);
                _context.SaveChanges();
                return session.Id;
            }

            private async Task<Domain.Place.Department> GetDepartment(Command request, CancellationToken cancellationToken)
            {
                return await _context.Department.Include(a => a.Roles).FirstOrDefaultAsync(a => a.Id == request.Session.Department.Id, cancellationToken);
            }

            private async Task<Observer> GetObserver(Command request, CancellationToken cancellationToken)
            {
                var facility = await _context.Facility
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.Id == request.Session.Department.FacilityId);

                if (facility == null)
                    throw new Exception($"Did not find the specified facility with ID: {request.Session.Department.FacilityId}");

                return facility
                    .Users
                    .OfType<Observer>()
                    .FirstOrDefault(_userService.HasEmailAndIsActive<Observer>(request.Email).Compile());
            }
        }
    }
}
