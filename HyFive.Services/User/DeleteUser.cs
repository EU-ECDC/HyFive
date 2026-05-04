using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Session;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.User
{
    public class DeleteUser
    {
        public class Command : IRequest<bool>
        {
            public Type UserType { get; set; }
            public int UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                Domain.User.User user = await LoadUserByType(command.UserType, command.UserId, cancellationToken);
                var type = command.UserType;

                if (user == null)
                    throw new DomainException("UserNotFound", command.UserId);

                //delete sessions/observations first (Session.ObserverId is Restrict)
                if (command.UserType == typeof(Observer))
                {
                    var hasUserSessions = await _context.Session
                        .AsNoTracking()
                        .AnyAsync(s => s.ObserverId == user.Id, cancellationToken);

                    if (hasUserSessions)
                    {
                        DeleteSessionsAndObservations(user.Id);
                    }
                }

                //delete UserPermissions first (UserPermission.UserId is Restrict)
                var permissions = _context.UserPermission.Where(p => p.UserId == user.Id);
                _context.UserPermission.RemoveRange(permissions);

                _context.User.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }

            private async Task<Domain.User.User> LoadUserByType(Type userType, int userId, CancellationToken ct)
            {
                if (userType == typeof(Coordinator))
                {
                    return await _context.User
                    .FirstOrDefaultAsync(u =>
                        u.Id == userId &&
                        u.UserPermissions.Any(p => p.PermissionLevel == PermissionLevelConstants.Coordinator),
                        ct);
                }

                if (userType == typeof(Observer))
                {
                    return await _context.User
                     .FirstOrDefaultAsync(u =>
                         u.Id == userId &&
                         u.UserPermissions.Any(p => p.PermissionLevel == PermissionLevelConstants.Observer),
                         ct);
                }

                throw new DomainException("UserTypeNotSupported", userType.Name);
            }

            private void DeleteSessionsAndObservations(int userId)
            {
                DeleteFourIndicatorsSessionsAndObservations(userId);
                DeleteHandJewelrySessionsAndObservations(userId);
                DeleteGloveSessionsAndObservations(userId);
                DeleteProtectiveEquipmentSessionsAndObservations(userId);
            }

            private void DeleteProtectiveEquipmentSessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<ProtectiveEquipmentSession>()
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.ProtectiveEquipmentList)
                    .Where(s => s.ObserverId == userId);

                foreach (var session in sessions)
                {
                    var protectiveEquipmentList = session.Observations.SelectMany(o => o.ProtectiveEquipmentList).ToList();
                    
                    if (protectiveEquipmentList.Any())
                        _context.RemoveRange(protectiveEquipmentList);

                    _context.ProtectiveEquipmentObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteGloveSessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<GloveSession>()
                    .Include(s => s.Observations)
                    .Where(s => s.ObserverId == userId);

                foreach (var session in sessions)
                {
                    _context.GloveObservation.RemoveRange(session.Observations);

                    _context.Session.Remove(session);
                }
            }

            private void DeleteHandJewelrySessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<HandJewelrySession>()
                    .Include(s => s.Observations)
                    .Where(s => s.ObserverId == userId).ToList();

                foreach (var session in sessions)
                {
                    _context.HandJewelryObservation.RemoveRange(session.Observations);

                    _context.Session.Remove(session);
                }
            }

            private void DeleteFourIndicatorsSessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<FiveIndicationsSession>()
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.Activity)
                    .Where(s => s.ObserverId == userId).ToList();

                foreach (var session in sessions)
                {
                    var activities = session.Observations.Select(o => o.Activity).ToList();

                    _context.Activity.RemoveRange(activities);
                    _context.FiveIndicationsObservation.RemoveRange(session.Observations);

                    _context.Session.Remove(session);
                }
            }
        }
    }
}
