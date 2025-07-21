using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Department
{
    public class DeleteDepartment
    {
        public class Command : IRequest<bool>
        {
            public int DepartmentId { get; set; }
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
                var institution = GetDepartment(command.DepartmentId);
                

                DeleteDepartmentWithAssociatedData(institution);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private Domain.Place.Department GetDepartment(int departmentId)
            {
                var department = _context.Department
                                .FirstOrDefault(i => i.Id == departmentId);

                if (department == null)
                {
                    throw new Exception($"Did not find institution with ID {departmentId}");
                }

                return department;
            }
        
            private void DeleteDepartmentWithAssociatedData(Domain.Place.Department department)
            {
                    DeleteFiveIndicationsSessionsAndObservations(department.Id);
                    DeleteHandJewelrySessionsAndObservations(department.Id);
                    DeleteGloveSessionsAndObservations(department.Id);
                    DeleteProtectiveEquipmentSessionsAndObservations(department.Id);

                    DeleteDepartment(department.Id);
            }

            private void DeleteDepartment(int departmentId)
            {
                var department = _context.Department.Find(departmentId);
                _context.Department.Remove(department);
            }

            private void DeleteFiveIndicationsSessionsAndObservations(int departmentId)
            {
                var departmentSessions = _context.Session.OfType<FiveIndicationsSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.Activity)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in departmentSessions)
                {
                    var activities = session.Observations.Select(o => o.Activity).ToList();
                    _context.Activity.RemoveRange(activities);
                    _context.FiveIndicationsObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteHandJewelrySessionsAndObservations(int departmentId)
            {
                var departmentSessions = _context.Session.OfType<HandJewelrySession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in departmentSessions)
                {
                    _context.HandJewelryObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteGloveSessionsAndObservations(int departmentId)
            {
                var departmentSessions = _context.Session.OfType<GloveSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in departmentSessions)
                {
                    _context.GloveObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteProtectiveEquipmentSessionsAndObservations(int departmentId)
            {
                var departmentSessions = _context.Session.OfType<ProtectiveEquipmentSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.ProtectiveEquipmentList)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var sesjon in departmentSessions)
                {
                    var protectiveEquipmentList = sesjon.Observations.SelectMany(o => o.ProtectiveEquipmentList).ToList();
                    _context.RemoveRange(protectiveEquipmentList);
                    _context.ProtectiveEquipmentObservation.RemoveRange(sesjon.Observations);
                    _context.Session.Remove(sesjon);
                }
            }
        }
    }
}