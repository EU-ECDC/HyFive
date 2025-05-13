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
using FourIndicationsSession = HyFive.Domain.Session.FourIndicationsSession;

namespace HyFive.Services.Reports.FourIndicators
{
    public class GetFourIndicatorsReportForDepartment
    {
        public class Query : IRequest<FourIndicatorsReportForDepartment>
        {
            public int DepartmentId { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToTime { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, FourIndicatorsReportForDepartment>
        {
            private readonly HandHygieneContext _context;


            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<FourIndicatorsReportForDepartment> Handle(Query request, CancellationToken cancellationToken)
            {
                var reportDto = new FourIndicatorsReportForDepartment();

                reportDto.Department = GetDepartmentData(request);
                reportDto.Clinics = GetReportsForClinics(request);
                reportDto.Institution = GetInstitutionData(request);
                reportDto.ComparableDepartments = await GetComparableDepartmentData(request);
                reportDto.SetDisplayTimestamps(request.FromDate, request.ToTime);
                return reportDto;
            }

            private FourIndicatorsReport GetDepartmentData(Query request)
            {
                var department = _context.Department.AsNoTracking().First(a => a.Id == request.DepartmentId);

                var departmentSessionsWithObservations = _context.Session.OfType<FourIndicationsSession>()
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
                        s.Department.Id == request.DepartmentId
                        && s.Observations.Any(o => o.RegisteredTime.Date >= request.FromDate.Date)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= request.ToTime.Date))
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

                return new FourIndicatorsReport()
                {
                    Name = department.Name,
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(departmentSessionsWithObservations),
                    NumberOfObservations = numberOfObservations,
                    DebugObservationsStringList = departmentSessionsWithObservations.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private async Task<FourIndicatorsReport> GetComparableDepartmentData(Query request)
            {
                var comparedDepartment = _context.Department.AsNoTracking().Include(a => a.DepartmentType).First(a => a.Id == request.DepartmentId);

                var SessionsOfComparableDepartments = await _context.Session.OfType<FourIndicationsSession>()
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
                        s.Department.Id != request.DepartmentId
                        && s.Department.DepartmentType.Code == comparedDepartment.DepartmentType.Code
                        && s.Observations.Any(o => o.RegisteredTime.Date >= request.FromDate.Date)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= request.ToTime.Date)
                        && s.Observations.Any())
                    .ToListAsync();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    SessionsOfComparableDepartments = SessionsOfComparableDepartments.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var session in SessionsOfComparableDepartments)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= request.FromDate.Date &&
                            o.RegisteredTime.Date <= request.ToTime.Date)
                        .ToList();
                }

                var observationsNumber = SessionsOfComparableDepartments.SelectMany(o => o.Observations).Count();

                return new FourIndicatorsReport()
                {
                    Name = $"Comparable departments for {comparedDepartment.Name}",
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(SessionsOfComparableDepartments),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = SessionsOfComparableDepartments.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private FourIndicatorsReport GetInstitutionData(Query request)
            {
                var institutionId = _context.Department
                    .AsNoTracking()
                    .Select(a => new { DepartmentId = a.Id, a.InstitutionId })
                    .First(a => a.DepartmentId == request.DepartmentId).InstitutionId;


                var institution = _context.Institution
                    .AsNoTracking()
                    .Include(i => i.InstitutionType)
                    .Select(i => new { i.Name, i.Id, InstitutionType = i.InstitutionType.Name })
                    .First(i => i.Id == institutionId);

                var institutionSessionsMinusRequestedDepartment = _context.Session.OfType<FourIndicationsSession>()
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
                        s.Department.InstitutionId == institutionId
                        && s.Observations.Any(o => o.RegisteredTime.Date >= request.FromDate.Date)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= request.ToTime.Date))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    institutionSessionsMinusRequestedDepartment = institutionSessionsMinusRequestedDepartment.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var sesjon in institutionSessionsMinusRequestedDepartment)
                {
                    sesjon.Observations = sesjon.Observations.Where(o =>
                            o.RegisteredTime.Date >= request.FromDate.Date &&
                            o.RegisteredTime.Date <= request.ToTime.Date)
                        .ToList();
                }

                var observationsNumber = institutionSessionsMinusRequestedDepartment.SelectMany(o => o.Observations).Count();

                return new FourIndicatorsReport()
                {
                    Name = $"{institution.InstitutionType}: {institution.Name} ",
                    FromDate = request.FromDate,
                    ToDate = request.ToTime,
                    Roles = GetRoleWithCombinationsReportList(institutionSessionsMinusRequestedDepartment),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = institutionSessionsMinusRequestedDepartment.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private List<FourIndicatorsReport> GetReportsForClinics(Query request)
            {
                var clinicReports = new List<FourIndicatorsReport>();

                var departmentClinicIds = _context.Department
                    .AsNoTracking()
                    .Include(a => a.Clinics)
                    .First(a => a.Id == request.DepartmentId).Clinics.Select(s => s.Id);

                foreach (var clinicId in departmentClinicIds)
                {
                    var report = GetClinicReport(clinicId, request);
                    clinicReports.Add(report);
                }

                return clinicReports;
            }

            private FourIndicatorsReport GetClinicReport(int clinicId, Query request)
            {
                var AssociatedClinicSessions = _context.Session.OfType<FourIndicationsSession>()
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
                        && s.Observations.Any(o => o.RegisteredTime.Date >= request.FromDate.Date)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= request.ToTime.Date))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    AssociatedClinicSessions = AssociatedClinicSessions.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi).ToList();
                }

                foreach (var sesjon in AssociatedClinicSessions)
                {
                    sesjon.Observations = sesjon.Observations.Where(o =>
                            o.RegisteredTime.Date >= request.FromDate.Date &&
                            o.RegisteredTime.Date <= request.ToTime.Date)
                        .ToList();
                }

                var observationsNumber = AssociatedClinicSessions.SelectMany(o => o.Observations).Count();
                var clinicName = _context.Clinic.FirstOrDefault(k => k.Id == clinicId)?.Name ?? "Without a name";

                var report = new FourIndicatorsReport()
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
            private List<RoleWithCombinationsReport> GetRoleWithCombinationsReportList(List<FourIndicationsSession> sessions)
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
                            .Where(o => o.Activity.TimeRecordingWasDone && o.Activity.ActivityType.Code == ActivityTypeConstants.Handwash)
                            .Select(o => o.Activity.TimeSpent));
                    disinfectionTimes.AddRange(
                        observations
                            .Where(o => o.Activity.TimeRecordingWasDone && o.Activity.ActivityType.Code == ActivityTypeConstants.Disinfection)
                            .Select(o => o.Activity.TimeSpent));

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
            private Combination CreateCombinationA(List<FourIndicationsObservation> observations)
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
            private Combination CreateCombinationB(List<FourIndicationsObservation> observations)
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
            private Combination CreateCombinationC(List<FourIndicationsObservation> observations)
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
            private Combination LagKombinasjonD(List<FourIndicationsObservation> observations)
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
            private Combination LagKombinasjonE(List<FourIndicationsObservation> observations)
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


            private Combination CreateCombination(List<FourIndicationsObservation> observations, string combinationName, params IndicationCombination[] combinationsOfIndication)
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

            private string DebugObservation(FourIndicationsObservation o)
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
