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
            public List<int> DepartmentIds { get; set; }
            public List<int> InstitutionIds { get; set; }
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

                var departments = _context.Department.AsNoTracking().Where(d => request.DepartmentIds.Contains(d.Id)).ToList();
                var institutions = _context.Institution.AsNoTracking().Where(i => request.InstitutionIds.Contains(i.Id)).ToList();

                var departmentNames = string.Join(", ", departments.Select(d => d.Name));
                var institutionNames = string.Join(", ", institutions.Select(i => i.Name));

                var report = new JewelryReportForJewelryTypeAndRole
                {
                    Department = departmentNames,
                    Institution = institutionNames,
                    FromDate = request.FromDateTime,
                    ToTime = request.ToDateTime,
                    ReportForDepartment = departmentReport,
                    ReportForInstitution = InstitutionReport
                };

                return report;
            }

            private ReportForUnit CreateDepartmentReport(Query request)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDateTime.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToDateTime.Date, DateTimeKind.Utc);

                var sessions = _context.Session.OfType<HandJewelrySession>()
                    .AsNoTracking()
                    .Include(p => p.TransferStatus)
                    .Include(s => s.Observations).ThenInclude(o => o.Role)
                    .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                    .Where(s =>
                        request.DepartmentIds.Contains(s.Department.Id)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    sessions = sessions.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).ToList();
                }

                var reportForUnit = CreateUnitReport(sessions);

                return reportForUnit;
            }

            private ReportForUnit CreateInstitutionReport(Query request)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDateTime.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToDateTime.Date, DateTimeKind.Utc);

                var sessions = _context.Session.OfType<HandJewelrySession>()
                   .AsNoTracking()
                   .Include(p => p.TransferStatus)
                   .Include(s => s.Observations).ThenInclude(o => o.Role)
                   .Include(s => s.Observations).ThenInclude(o => o.HandJewelries)
                   .Where(s =>
                       request.InstitutionIds.Contains(s.Department.InstitutionId)
                       && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                       && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                   .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    sessions = sessions.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).ToList();
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
                    foreach (var jewelryType in observation.HandJewelries)
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
