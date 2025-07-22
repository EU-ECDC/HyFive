using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Observation;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;
using FiveIndicationsSession = HyFive.Domain.Session.FiveIndicationsSession;

namespace HyFive.Services.Reports.FiveIndicators
{
    public class GetFiveIndicatorsReportForDepartment
    {
        public class Query : IRequest<FiveIndicatorsReportForDepartment>
        {
            public List<int> DepartmentIds { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToTime { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, FiveIndicatorsReportForDepartment>
        {
            private readonly HandHygieneContext _context;


            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<FiveIndicatorsReportForDepartment> Handle(Query request, CancellationToken cancellationToken)
            {
                var reportDto = new FiveIndicatorsReportForDepartment();

                reportDto.Department = GetDepartmentData(request);
                reportDto.Clinics = GetReportsForClinics(request);
                reportDto.Institution = GetInstitutionData(request);
                reportDto.ComparableDepartments = await GetComparableDepartmentData(request);
                reportDto.SetDisplayTimestamps(request.FromDate, request.ToTime);
                return reportDto;
            }

            private FiveIndicatorsReport GetDepartmentData(Query request)
            {
                var departments = _context.Department.AsNoTracking().Where(a => request.DepartmentIds.Contains(a.Id)).ToList();

                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToTime.Date, DateTimeKind.Utc);

                var departmentSessionsWithObservations = _context.Session.OfType<FiveIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.Observations)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.Activity)
                        .ThenInclude(a => a.ActivityType)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.IndicationTypes)
                    .Where(s =>
                        request.DepartmentIds.Contains(s.Department.Id)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    departmentSessionsWithObservations = departmentSessionsWithObservations.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var session in departmentSessionsWithObservations)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= request.FromDate.Date &&
                            o.RegisteredTime.Date <= request.ToTime.Date)
                        .ToList();
                }

                var numberOfObservations = departmentSessionsWithObservations.SelectMany(o => o.Observations).Count();

                return new FiveIndicatorsReport()
                {
                    Name = string.Join(", ", departments.Select(d => d.Name)),
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(departmentSessionsWithObservations),
                    NumberOfObservations = numberOfObservations,
                    DebugObservationsStringList = departmentSessionsWithObservations.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private async Task<FiveIndicatorsReport> GetComparableDepartmentData(Query request)
            {
                var comparedDepartments = _context.Department.AsNoTracking().Include(a => a.DepartmentType).Where(d => request.DepartmentIds.Contains(d.Id)).ToList();

                var departmentTypeCodes = comparedDepartments
                    .Select(d => d.DepartmentType.Code)
                    .Distinct()
                    .ToList();

                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToTime.Date, DateTimeKind.Utc);

                var SessionsOfComparableDepartments = await _context.Session.OfType<FiveIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.Department)
                        .ThenInclude(s => s.DepartmentType)
                    .Include(s => s.Observations)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.Activity)
                        .ThenInclude(a => a.ActivityType)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.IndicationTypes)
                    .Where(s =>
                        !request.DepartmentIds.Contains(s.Department.Id)
                        && departmentTypeCodes.Contains(s.Department.DepartmentType.Code)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc)
                        && s.Observations.Any())
                    .ToListAsync();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    SessionsOfComparableDepartments = SessionsOfComparableDepartments.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var session in SessionsOfComparableDepartments)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= fromDateUtc &&
                            o.RegisteredTime.Date <= toDateUtc)
                        .ToList();
                }

                var observationsNumber = SessionsOfComparableDepartments.SelectMany(o => o.Observations).Count();

                var departmentNames = string.Join(", ", comparedDepartments.Select(d => d.Name));


                return new FiveIndicatorsReport()
                {
                    Name = $"Comparable departments for {comparedDepartments}",
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(SessionsOfComparableDepartments),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = SessionsOfComparableDepartments.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private FiveIndicatorsReport GetInstitutionData(Query request)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToTime.Date, DateTimeKind.Utc);

                var institutionIds = _context.Department
                    .AsNoTracking()
                    .Where(d => request.DepartmentIds.Contains(d.Id))
                    .Select(d => d.InstitutionId)
                    .Distinct()
                    .ToList();


                var institutionNames = _context.Institution
                    .AsNoTracking()
                    .Include(i => i.InstitutionType)
                    .Where(i => institutionIds.Contains(i.Id))
                    .Select(i => new { i.Name, InstitutionTypeName = i.InstitutionType.Name })
                    .ToList();

                string institutionDisplayName = string.Join(" | ", institutionNames
                    .Select(i => $"{i.InstitutionTypeName}: {i.Name}"));

                var institutionSessionsMinusRequestedDepartment = _context.Session.OfType<FiveIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.Department)
                        .ThenInclude(s => s.DepartmentType)
                    .Include(s => s.Observations)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.Activity)
                        .ThenInclude(a => a.ActivityType)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.IndicationTypes)
                    .Where(s =>
                        institutionIds.Contains(s.Department.InstitutionId)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    institutionSessionsMinusRequestedDepartment = institutionSessionsMinusRequestedDepartment.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var session in institutionSessionsMinusRequestedDepartment)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= fromDateUtc &&
                            o.RegisteredTime.Date <= toDateUtc)
                        .ToList();
                }

                var observationsNumber = institutionSessionsMinusRequestedDepartment.SelectMany(o => o.Observations).Count();

                return new FiveIndicatorsReport()
                {
                    Name = institutionDisplayName,
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(institutionSessionsMinusRequestedDepartment),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = institutionSessionsMinusRequestedDepartment.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private List<FiveIndicatorsReport> GetReportsForClinics(Query request)
            {
                var clinicReports = new List<FiveIndicatorsReport>();

                var departmentClinicIds = _context.Department
                    .AsNoTracking()
                    .Where(d => request.DepartmentIds.Contains(d.Id))
                    .Include(d => d.Clinics)
                    .SelectMany(d => d.Clinics.Select(c => c.Id))
                    .Distinct()
                    .ToList();

                foreach (var clinicId in departmentClinicIds)
                {
                    var report = GetClinicReport(clinicId, request);
                    clinicReports.Add(report);
                }

                return clinicReports;
            }

            private FiveIndicatorsReport GetClinicReport(int clinicId, Query request)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToTime.Date, DateTimeKind.Utc);

                var AssociatedClinicSessions = _context.Session.OfType<FiveIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.Department)
                        .ThenInclude(a => a.DepartmentType)
                    .Include(s => s.Department)
                        .ThenInclude(a => a.Clinics)
                    .Include(s => s.Observations)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.Activity)
                        .ThenInclude(a => a.ActivityType)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.IndicationTypes)
                    .Where(s =>
                        s.Department.Clinics.Any(k => k.Id == clinicId)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    AssociatedClinicSessions = AssociatedClinicSessions.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var session in AssociatedClinicSessions)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= fromDateUtc &&
                            o.RegisteredTime.Date <= toDateUtc)
                        .ToList();
                }

                var observationsNumber = AssociatedClinicSessions.SelectMany(o => o.Observations).Count();
                var clinicName = _context.Clinic.FirstOrDefault(k => k.Id == clinicId)?.Name ?? "Without a name";

                var report = new FiveIndicatorsReport()
                {
                    Name = $"Clinic: {clinicName}",
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(AssociatedClinicSessions),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = AssociatedClinicSessions.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };

                return report;
            }

            #region Combinations
            /// <summary>
            ///  * 1: Before patient
            ///  * 2: Aseptic
            ///  * 3: Bodily fluids
            ///  * 4: After patient
            ///  *
            ///  * Suggestion from the medical department by Mette Fagernes for grouping indications:
            ///  A (before patient) = 1, 1+2
            ///  B (before aseptic – inside the zone) = 2, 3+2
            ///  C (after bodily fluids – primarily inside the zone) = 3, 
            ///  D (after patient) = 4, 3+4, 
            ///  E (transition between patients) = 4+1, 3+4+1, 3+1, 3+1+2, 4+2, 4+1+2, 3+4+1+2, 3+4+2,
            /// 
            /// </summary>
            /// <param name="sessions"></param>
            /// <returns></returns>
            private List<RoleWithCombinationsReport> GetRoleWithCombinationsReportList(List<FiveIndicationsSession> sessions)
            {
                var dtoList = new List<RoleWithCombinationsReport>();
                var groupedByRoles = sessions.SelectMany(s => s.Observations).GroupBy(o => new { o.Role.Name });

                foreach (var observationsRole in groupedByRoles)
                {
                    var handwashingTimes = new List<int>();
                    var disinfectionTimes = new List<int>();

                    var dto = new RoleWithCombinationsReport();
                    dto.Name = observationsRole.Key.Name;
                    dto.TotalNumberOfObservations = observationsRole.Count();
                    var observations = observationsRole.ToList();
                    handwashingTimes.AddRange(
                        observations
                            .Where(o => o.Activity.TimingWasPerformed && o.Activity.ActivityType.Code == ActivityTypeConstants.Handwash)
                            .Select(o => o.Activity.SecondsUsed));
                    disinfectionTimes.AddRange(
                        observations
                            .Where(o => o.Activity.TimingWasPerformed && o.Activity.ActivityType.Code == ActivityTypeConstants.Disinfection)
                            .Select(o => o.Activity.SecondsUsed));

                    dto.Combinations.Add(CreateCombinationA(observations));
                    dto.Combinations.Add(CreateCombinationB(observations));
                    dto.Combinations.Add(CreateCombinationC(observations));
                    dto.Combinations.Add(LagKombinasjonD(observations));
                    dto.Combinations.Add(LagKombinasjonE(observations));

                    dtoList.Add(dto);

                    if (handwashingTimes.Any())
                    {
                        dto.AverageHandwashingTime = handwashingTimes.Average().ToString();
                    }
                    if (disinfectionTimes.Any())
                    {
                        dto.AverageHandSanitizingTime = disinfectionTimes.Average().ToString();
                    }
                }
                return dtoList;
            }

            /// <summary>
            /// A (before patient) = 1, 1+2.
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombinationA(List<FiveIndicationsObservation> observations)
            {
                //var name = "A (For patient)";
                var name = "A";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BeforePatient}}, // 1
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 1+2
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// B (before aseptic – inside the zone) = 2, 3+2
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluids
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombinationB(List<FiveIndicationsObservation> observations)
            {
                //var name = "B (before aseptic – inside the zone)";
                var name = "B";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AsepticProcedures}}, // 2
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AsepticProcedures}} // 3 + 2
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// C (after body fluid – primarily inside the zone) = 3,
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombinationC(List<FiveIndicationsObservation> observations)
            {
                //var name = "C (after body fluid – primarily inside the zone)";
                var name = "C";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid}} // 3
                };
                return CreateCombination(observations, name, combinations);
            }


            /// <summary>
            /// D (after patient) = 4, 3+4
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination LagKombinasjonD(List<FiveIndicationsObservation> observations)
            {
                //var name = "D (etter pasient)";
                var name = "D";

                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AfterPatient}}, // 4
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient}} // 3+4
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// E (transition between patients) = 4+1, 3+4+1, 3+1, 3+1+2, 4+2, 4+1+2, 3+4+1+2, 3+4+2
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination LagKombinasjonE(List<FiveIndicationsObservation> observations)
            {
                //var name = "E (overgang mellom pasienter)";
                var name = "E";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient}}, // 4 + 1
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient}}, // 3 + 4 + 1
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.BeforePatient}}, // 3 + 1
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 3 + 1 + 2
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AfterPatient, IndicationTypeConstants.AsepticProcedures}}, // 4 + 2
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 4 + 1 + 2
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient, IndicationTypeConstants.BeforePatient, IndicationTypeConstants.AsepticProcedures}}, // 3 + 4 + 1 + 2
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient, IndicationTypeConstants.AsepticProcedures}}, // 3 + 4 + 2
                };
                return CreateCombination(observations, name, combinations);
            }


            private Combination CreateCombination(List<FiveIndicationsObservation> observations, string combinationName, params IndicationCombination[] combinationsOfIndication)
            {
                var relevantObservations = observations
                    .Where(o => MeetsTheCombinationCriteria(o.IndicationTypes.Select(i => i.Code), combinationsOfIndication)).ToList();
                var count = relevantObservations.Count();
                var complied = relevantObservations
                    .Count(o => o.Activity.ActivityType.Code != ActivityTypeConstants.NotExecuted);

                var compliancePercentage = CalculateCompliancePercentage(count, complied);
                var nonCompliancePercentage = CalculateNonCompliancePercentage(count, compliancePercentage);
                var combination = new Combination()
                {
                    Name = $"{combinationName}",
                    Role = observations.First().Role.Name,
                    NumberOfObservations = count,
                    PercentComplied = compliancePercentage,
                    PercentNotComplied = nonCompliancePercentage,
                };
                return combination;
            }

            private double CalculateCompliancePercentage(int count, int complied)
            {
                if (count == 0 || complied == 0)
                    return 0.0f;
                return (complied / (double)count) * 100;
            }

            private double CalculateNonCompliancePercentage(int count, double complied)
            {
                if (count > 0)
                {
                    return (100d - complied);
                }

                return 0.0d;
            }

            private bool MeetsTheCombinationCriteria(IEnumerable<string> indicationTypes, IndicationCombination[] combinationsOfIndication)
            {
                foreach (var combination in combinationsOfIndication)
                {
                    if (indicationTypes.OrderBy(i => i).SequenceEqual(combination.Code.OrderBy(k => k)))
                    {
                        return true;
                    }
                }
                return false;
            }
            #endregion

            private string DebugObservation(FiveIndicationsObservation o)
            {
                return
                    $"{o.Id}, rolle: {o.Role.Name} (id:{o.Role.Id} {o.Activity.ActivityType.Code} {string.Join(',', o.IndicationTypes.Select(i => i.Code))}";
            }

            private struct IndicationCombination
            {
                public string[] Code { get; set; }
            }
        }


    }
}
