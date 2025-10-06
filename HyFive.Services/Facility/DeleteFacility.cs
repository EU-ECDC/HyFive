using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class DeleteFacility
    {
        public class Command : IRequest<bool>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var facility = GetFacility(command.FacilityId);

                DeleteDepartmentWithRelatedData(facility);
                DeleteClinics(facility.Id);
                DeletePredefinedComments(facility.Id);
                DeleteUsers(facility.Id);
                DeleteFacility(facility);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private Domain.Place.Facility GetFacility(int facilityId)
            {
                var facility = _context.Facility
                                .Include(i=>i.Departments)
                                .FirstOrDefault(i=>i.Id == facilityId);

                if (facility == null)
                {
                    throw new Exception($"Did not find Facility With Id: {facilityId}");
                }

                return facility;
            }

            private void DeleteFacility(Domain.Place.Facility facility)
            {
                _context.Facility.Remove(facility);
            }

            private void DeleteUsers(int facilityId)
            {
                var usersForFacility = _context.User.Where(b => b.Facility.Id == facilityId);
                _context.User.RemoveRange(usersForFacility);
            }

            private void DeleteClinics(int facilityId)
            {
                var clinics = _context.Clinic.Where(k => k.Facility.Id == facilityId);
                _context.Clinic.RemoveRange(clinics);
            }

            private void DeletePredefinedComments(int facilityId)
            {
                var predefinedComments = _context.PredefinedComment.Where(p => p.FacilityId == facilityId);
                _context.PredefinedComment.RemoveRange(predefinedComments);
            }

            private void DeleteDepartmentWithRelatedData(Domain.Place.Facility facility)
            {
                foreach (var department in facility.Departments)
                {
                    DeleteFiveIndicationsSessionsAndObservations(department.Id);
                    DeleteHandJewelrySessionsAndObservations(department.Id);
                    DeleteGloveSessionsAndObservations(department.Id);
                    DeleteProtectiveEquipmentSessionsAndObservations(department.Id);

                    DeleteDepartment(department.Id);
                }
            }

            private void DeleteDepartment(int departmentId)
            {
                var department = _context.Department.Find(departmentId);
                _context.Department.Remove(department);
            }

            private void DeleteFiveIndicationsSessionsAndObservations(int departmentId)
            {
                var sessionsForDepartment = _context.Session.OfType<FiveIndicationsSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .ThenInclude(o=>o.Activity)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in sessionsForDepartment)
                {
                    var activities = session.Observations.Select(o => o.Activity).ToList();
                    _context.Activity.RemoveRange(activities);
                    _context.FiveIndicationsObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteHandJewelrySessionsAndObservations(int departmentId)
            {
                var sessionsForDepartment = _context.Session.OfType<HandJewelrySession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in sessionsForDepartment)
                {
                    _context.HandJewelryObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteGloveSessionsAndObservations(int departmentId)
            {
                var sessionsForDepartment = _context.Session.OfType<GloveSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in sessionsForDepartment)
                {
                    _context.GloveObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteProtectiveEquipmentSessionsAndObservations(int departmentId)
            {
                var SessionsForDepartment = _context.Session.OfType<ProtectiveEquipmentSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .ThenInclude(o=>o.ProtectiveEquipmentList)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in SessionsForDepartment)
                {
                    var protectiveEquipmentList = session.Observations.SelectMany(o => o.ProtectiveEquipmentList).ToList();
                    _context.RemoveRange(protectiveEquipmentList);
                    _context.ProtectiveEquipmentObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }
        }
    }
}
