using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Institution
{
    public class DeleteInstitution
    {
        public class Command : IRequest<bool>
        {
            public int InstitutionId { get; set; }
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
                var institution = GetInstitution(command.InstitutionId);

                DeleteDepartmentWithRelatedData(institution);
                DeleteClinics(institution.Id);
                DeletePredefinedComments(institution.Id);
                DeleteUsers(institution.Id);
                DeleteInstitution(institution);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private Domain.Place.Institution GetInstitution(int institutionId)
            {
                var institution = _context.Institution
                                .Include(i=>i.Departments)
                                .FirstOrDefault(i=>i.Id == institutionId);

                if (institution == null)
                {
                    throw new Exception($"Could Not Find Institution With Id: {institutionId}");
                }

                return institution;
            }

            private void DeleteInstitution(Domain.Place.Institution institution)
            {
                _context.Institution.Remove(institution);
            }

            private void DeleteUsers(int institutionId)
            {
                var UsersForInstitution = _context.User.Where(b => b.Institution.Id == institutionId);
                _context.User.RemoveRange(UsersForInstitution);
            }

            private void DeleteClinics(int institutionId)
            {
                var clinics = _context.Clinic.Where(k => k.Institution.Id == institutionId);
                _context.Clinic.RemoveRange(clinics);
            }

            private void DeletePredefinedComments(int institusjonsId)
            {
                var preDefinerteKommentarer = _context.PredefinedComments.Where(p => p.InstitutionId == institusjonsId);
                _context.PredefinedComments.RemoveRange(preDefinerteKommentarer);
            }

            private void DeleteDepartmentWithRelatedData(Domain.Place.Institution institution)
            {
                foreach (var department in institution.Departments)
                {
                    DeleteFourIndicationsSessionsAndObservations(department.Id);
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

            private void DeleteFourIndicationsSessionsAndObservations(int departmentId)
            {
                var sessionsForDepartment = _context.Session.OfType<FourIndicationsSession>()
                    .Include(s => s.Department)
                    .Include(s => s.Observations)
                    .ThenInclude(o=>o.Activity)
                    .Where(s => s.Department.Id == departmentId).ToList();

                foreach (var session in sessionsForDepartment)
                {
                    var activities = session.Observations.Select(o => o.Activity).ToList();
                    _context.Activity.RemoveRange(activities);
                    _context.FourIndicationsObservation.RemoveRange(session.Observations);
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
