using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Domain.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                Domain.User.User user = null;
                var type = command.UserType;

                if (type == typeof(Coordinator))
                {
                    user = await _context.User.OfType<Coordinator>()
                        .FirstOrDefaultAsync(i => i.Id == command.UserId, cancellationToken: cancellationToken);
                }
                else if (type == typeof(Observer))
                {
                    user = await _context.User.OfType<Observer>()
                        .FirstOrDefaultAsync(i => i.Id == command.UserId, cancellationToken: cancellationToken);

                    var hasUserSessions = await _context.Session.AnyAsync(s => s.Observer.Id == user.Id, cancellationToken);

                    if (hasUserSessions)
                    {
                        DeleteSessionsAndObservations(user.Id);
                    }
                }

                _context.User.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
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
                    .Include(s => s.Observer)
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.ProtectiveEquipmentList)
                    .Where(s => s.Observer.Id == userId);

                foreach (var session in sessions)
                {
                    var protectiveEquipmentList = session.Observations.SelectMany(o => o.ProtectiveEquipmentList);
                    _context.RemoveRange(protectiveEquipmentList);
                    _context.ProtectiveEquipmentObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteGloveSessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<GloveSession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observations)
                    .Where(s => s.Observer.Id == userId);

                foreach (var session in sessions)
                {
                    _context.GloveObservation.RemoveRange(session.Observations);

                    _context.Session.Remove(session);
                }
            }

            private void DeleteHandJewelrySessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<HandJewelrySession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observations)
                    .Where(s => s.Observer.Id == userId).ToList();

                foreach (var session in sessions)
                {
                    _context.HandJewelryObservation.RemoveRange(session.Observations);

                    _context.Session.Remove(session);
                }
            }

            private void DeleteFourIndicatorsSessionsAndObservations(int userId)
            {
                var sessions = _context.Session.OfType<FiveIndicationsSession>()
                    .Include(s => s.Observer)
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.Activity)
                    .Where(s => s.Observer.Id == userId).ToList();

                foreach (var session in sessions)
                {
                    var activities =
                        session.Observations.Select(o => o.Activity).ToList();

                    _context.Activity.RemoveRange(activities);
                    _context.FiveIndicationsObservation.RemoveRange(session.Observations);

                    _context.Session.Remove(session);
                }
            }
        }
    }
}
