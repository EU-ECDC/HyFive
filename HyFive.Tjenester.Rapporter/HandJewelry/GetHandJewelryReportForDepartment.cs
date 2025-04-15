using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Observation;
using HyFive.Domain.Session;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Reports.HandJewelry
{
    public class GetHandJewelryReportForDepartment
    {
        public class Query : IRequest<JewelryReportForJewelryTypeAndRole>
        {
            public int DepartmentId { get; set; }
            public int InstitutionId { get; set; }
            public DateTime FromDateTime { get; set; }
            public DateTime ToDateTime { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, JewelryReportForJewelryTypeAndRole>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<JewelryReportForJewelryTypeAndRole> Handle(Query request, CancellationToken cancellationToken)
            {
                var departmentReport = CreateDepartmentReport(request);
                var InstitutionReport = CreateInstitutionReport(request);

                var department = _context.Department.AsNoTracking().First(a => a.Id == request.DepartmentId);
                var institution = _context.Institution.AsNoTracking().First(a => a.Id == request.InstitutionId);
                var report = new JewelryReportForJewelryTypeAndRole
                {
                    Department = department.Name,
                    Institution = institution.Name,
                    FromTime = request.FromDateTime,
                    ToTime = request.ToDateTime,
                    ReportForDepartment = departmentReport,
                    ReportForInstitution = InstitutionReport
                };

                return report;
            }

            private ReportForUnit CreateDepartmentReport(Query request)
            {
                var sessions = _context.Session.OfType<HandJewelrySession>()
                    .AsNoTracking()
                    .Include(p => p.TransmissionStatus)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelry)
                    .Where(s =>
                        s.Department.Id == request.DepartmentId
                        && s.Observations.Any(o => o.RegistrationTime.Date >= request.FromDateTime.Date)
                        && s.Observations.Any(o => o.RegistrationTime.Date <= request.ToDateTime.Date))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    sessions = sessions.Where(p => p.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                var reportForUnit = CreateUnitReport(sessions);

                return reportForUnit;
            }

            private ReportForUnit CreateInstitutionReport(Query request)
            {
                var sessions = _context.Session.OfType<HandJewelrySession>()
                   .AsNoTracking()
                   .Include(p => p.TransmissionStatus)
                   .Include(s => s.Observations).ThenInclude(o => o.Role)
                   .Include(s => s.Observations).ThenInclude(o => o.HandJewelry)
                   .Where(s =>
                       s.Department.InstitutionId == request.InstitutionId
                       && s.Observations.Any(o => o.RegistrationTime.Date >= request.FromDateTime.Date)
                       && s.Observations.Any(o => o.RegistrationTime.Date <= request.ToDateTime.Date))
                   .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    sessions = sessions.Where(p => p.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                var unitReport = CreateUnitReport(sessions);

                return unitReport;
            }

            private ReportForUnit CreateUnitReport(IEnumerable<HandJewelrySession> sessions)
            {
                var observations = sessions.SelectMany(p => p.Observations).ToList();

                var jewelryTypeAndRoleList = new List<JewelryTypeAndRole>();
                foreach (var observation in observations)
                {
                    foreach (var jewelryType in observation.HandJewelry)
                    {
                        jewelryTypeAndRoleList.Add(new JewelryTypeAndRole
                        {
                            Role = observation.Role.Name,
                            handJewelryType = jewelryType,
                        });
                    }
                }

                var jewelryTypes = _context.HandJewelryType.AsNoTracking().ToList();

                var jewelryTypeAndCountForRoleList = new List<RoleCountForJewelryType>();
                foreach (var jewelryTypeName in jewelryTypeAndRoleList.Select(p => p.handJewelryType.Name).Distinct())
                {
                    var roleListCount = jewelryTypeAndRoleList
                        .Where(p => p.handJewelryType.Name == jewelryTypeName)
                        .GroupBy(q => q.Role)
                        .Select(r => new CountByRole { Count = r.Count(), Role = r.Key })
                        .ToList();

                    var jewelryType = jewelryTypes.First(p => p.Name == jewelryTypeName);
                    jewelryTypeAndCountForRoleList.Add(new RoleCountForJewelryType
                    {
                        JewelryType = jewelryType,
                        CountByRoleList = roleListCount
                    });
                }

                var observationsForRoleList = observations
                    .GroupBy(p => p.Role.Name)
                    .Select(q => new ObservationsByRole { Count = q.Count(), Role = q.Key })
                    .ToList();

                var unitReport = new ReportForUnit
                {
                    RoleJewelrySummaryList = jewelryTypeAndCountForRoleList,
                    ListOfObservationsByRole = observationsForRoleList
                };

                return unitReport;
            }

            private class JewelryTypeAndRole
            {
                public HandJewelryType handJewelryType { get; set; }
                public string Role { get; init; }
            }
        }
    }
}
