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
            public List<int> OrganisationUnitIds { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
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
                reportDto.Units = GetReportsForUnits(request);
                reportDto.Facility = GetFacilityData(request);
                reportDto.ComparableDepartments = await GetComparableDepartmentData(request);
                reportDto.SetDisplayTimestamps(request.FromDate, request.ToDate);
                return reportDto;
            }

            private FiveIndicatorsReport GetDepartmentData(Query request)
            {
                var ous = _context.OrganisationUnit
                        .AsNoTracking()
                        .Where(ou => request.OrganisationUnitIds.Contains(ou.Id))
                        .ToList();
                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToDate.Date, DateTimeKind.Utc);

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
                        request.OrganisationUnitIds.Contains(s.OrganisationUnitId)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    departmentSessionsWithObservations = departmentSessionsWithObservations.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).ToList();
                }

                foreach (var session in departmentSessionsWithObservations)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= request.FromDate.Date &&
                            o.RegisteredTime.Date <= request.ToDate.Date)
                        .ToList();
                }

                var numberOfObservations = departmentSessionsWithObservations.SelectMany(o => o.Observations).Count();

                return new FiveIndicatorsReport()
                {
                    Name = string.Join(", ", ous.Select(d => d.Name)),
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    Roles = GetRoleWithCombinationsReportList(departmentSessionsWithObservations),
                    NumberOfObservations = numberOfObservations,
                    DebugObservationsStringList = departmentSessionsWithObservations.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private async Task<FiveIndicatorsReport> GetComparableDepartmentData(Query request)
            {
                var comparedOus = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(ou => ou.Type)
                    .Where(ou => request.OrganisationUnitIds.Contains(ou.Id))
                    .ToListAsync();

                var typeCodes = comparedOus
                    .Select(ou => ou.Type.Code)
                    .Distinct()
                    .ToList();

                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToDate.Date, DateTimeKind.Utc);

                var SessionsOfComparableDepartments = await _context.Session.OfType<FiveIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                        .ThenInclude(s => s.Type)
                    .Include(s => s.Observations)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.Activity)
                        .ThenInclude(a => a.ActivityType)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.IndicationTypes)
                    .Where(s =>
                        !request.OrganisationUnitIds.Contains(s.OrganisationUnitId) &&
                        s.OrganisationUnit != null &&
                        typeCodes.Contains(s.OrganisationUnit.Type.Code) 
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc)
                        && s.Observations.Any())
                    .ToListAsync();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    SessionsOfComparableDepartments = SessionsOfComparableDepartments.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).ToList();
                }

                foreach (var session in SessionsOfComparableDepartments)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= fromDateUtc &&
                            o.RegisteredTime.Date <= toDateUtc)
                        .ToList();
                }

                var observationsNumber = SessionsOfComparableDepartments.SelectMany(o => o.Observations).Count();

                return new FiveIndicatorsReport()
                {
                    Name = $"Comparable departments for {string.Join(", ", comparedOus.Select(x => x.Name))}",
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    Roles = GetRoleWithCombinationsReportList(SessionsOfComparableDepartments),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = SessionsOfComparableDepartments.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private FiveIndicatorsReport GetFacilityData(Query request)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToDate.Date, DateTimeKind.Utc);

                var facilityIds = _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(d => request.OrganisationUnitIds.Contains(d.Id))
                    .Select(ou => ou.ParentId)
                    .Where(pid => pid != null)
                    .Select(pid => pid.Value)
                    .Distinct()
                    .ToList();


                var facilityNames = _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(i => i.Type)
                    .Where(i => facilityIds.Contains(i.Id))
                    .Select(i => new { i.Name, FacilityTypeName = i.Type.Name })
                    .ToList();

                string facilityDisplayName = string.Join(" | ", facilityNames
                    .Select(i => $"{i.FacilityTypeName}: {i.Name}"));

                var facilitySessionsMinusRequestedDepartment = _context.Session.OfType<FiveIndicationsSession>()
                    .AsNoTracking()
                    .Include(s => s.TransferStatus)
                    .Include(s => s.OrganisationUnit)
                    .Include(s => s.Observations)
                        .ThenInclude(o => o.Role)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.Activity)
                        .ThenInclude(a => a.ActivityType)
                    .Include(o => o.Observations)
                        .ThenInclude(o => o.IndicationTypes)
                    .Where(s =>
                        facilityIds.Contains(s.OrganisationUnit.ParentId.Value)
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    facilitySessionsMinusRequestedDepartment = facilitySessionsMinusRequestedDepartment.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).ToList();
                }

                foreach (var session in facilitySessionsMinusRequestedDepartment)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= fromDateUtc &&
                            o.RegisteredTime.Date <= toDateUtc)
                        .ToList();
                }

                var observationsNumber = facilitySessionsMinusRequestedDepartment.SelectMany(o => o.Observations).Count();

                return new FiveIndicatorsReport()
                {
                    Name = facilityDisplayName,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    Roles = GetRoleWithCombinationsReportList(facilitySessionsMinusRequestedDepartment),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = facilitySessionsMinusRequestedDepartment.SelectMany(o => o.Observations)
                        .Select(o => DebugObservation(o))
                        .ToArray()
                };
            }

            private List<FiveIndicatorsReport> GetReportsForUnits(Query request)
            {
                var unitReports = new List<FiveIndicatorsReport>();

                var departmentUnitIds = _context.OrganisationUnit
                    .AsNoTracking()
                    .Where(d => request.OrganisationUnitIds.Contains(d.Id))
                    .Include(d => d.Children)
                    .SelectMany(d => d.Children.Select(c => c.Id))
                    .Distinct()
                    .ToList();

                foreach (var unitId in departmentUnitIds)
                {
                    var report = GetUnitReport(unitId, request);
                    unitReports.Add(report);
                }

                return unitReports;
            }

            private FiveIndicatorsReport GetUnitReport(int unitId, Query request)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Date, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(request.ToDate.Date, DateTimeKind.Utc);

                var AssociatedUnitSessions = _context.Session.OfType<FiveIndicationsSession>()
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
                        s.OrganisationUnitId == unitId
                        && s.Observations.Any(o => o.RegisteredTime.Date >= fromDateUtc)
                        && s.Observations.Any(o => o.RegisteredTime.Date <= toDateUtc))
                    .ToList();

                if (request.Role == AuthorizedRole.Administrator)
                {
                    AssociatedUnitSessions = AssociatedUnitSessions.Where(p => p.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).ToList();
                }

                foreach (var session in AssociatedUnitSessions)
                {
                    session.Observations = session.Observations.Where(o =>
                            o.RegisteredTime.Date >= fromDateUtc &&
                            o.RegisteredTime.Date <= toDateUtc)
                        .ToList();
                }

                var observationsNumber = AssociatedUnitSessions.SelectMany(o => o.Observations).Count();
                var unitName = _context.OrganisationUnit.FirstOrDefault(k => k.Id == unitId)?.Name ?? "Without a name";

                var report = new FiveIndicatorsReport()
                {
                    Name = $"Unit: {unitName}",
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    Roles = GetRoleWithCombinationsReportList(AssociatedUnitSessions),
                    NumberOfObservations = observationsNumber,
                    DebugObservationsStringList = AssociatedUnitSessions.SelectMany(o => o.Observations)
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
            ///  * 5: After patient’s surroundings
            ///  *
            ///  * Suggestion from the medical department by Mette Fagernes for grouping indications:
            ///  A (before patient) = 1, 1+2
            ///  B (before aseptic – inside the zone) = 2, 3+2
            ///  C (after patient) = 4, 3+4, 
            ///  D (transition between patients) = 4+1, 3+4+1, 3+1, 3+1+2, 4+2, 4+1+2, 3+4+1+2, 3+4+2,
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

                    dto.Combinations.Add(CreateCombination1(observations));
                    dto.Combinations.Add(CreateCombination2(observations));
                    dto.Combinations.Add(CreateCombination3(observations));
                    dto.Combinations.Add(CreateCombination4(observations));
                    dto.Combinations.Add(CreateCombination5(observations));
                    dto.Combinations.Add(CreateCombinationA(observations));
                    dto.Combinations.Add(CreateCombinationB(observations));
                    dto.Combinations.Add(CreateCombinationC(observations));
                    dto.Combinations.Add(CreateCombinationD(observations));

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
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombination1(List<FiveIndicationsObservation> observations)
            {
                var name = "1";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BeforePatient}}, // 1
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombination2(List<FiveIndicationsObservation> observations)
            {
                var name = "2";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AsepticProcedures}}, // 2
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombination3(List<FiveIndicationsObservation> observations)
            {
                var name = "3";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid}}, // 3
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombination4(List<FiveIndicationsObservation> observations)
            {
                var name = "4";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AfterPatient}}, // 4
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// * 5: After patient’s surroundings
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombination5(List<FiveIndicationsObservation> observations)
            {
                var name = "5";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.PatientsSurroundings}}, // 5
                };
                return CreateCombination(observations, name, combinations);
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
                var name = "B";
                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AsepticProcedures}}, // 2
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AsepticProcedures}} // 3 + 2
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// C (after patient) = 4, 3+4
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombinationC(List<FiveIndicationsObservation> observations)
            {
                var name = "C";

                var combinations = new[]
                {
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.AfterPatient}}, // 4
                    new IndicationCombination() {Code = new[] {IndicationTypeConstants.BodyFluid, IndicationTypeConstants.AfterPatient}} // 3+4
                };
                return CreateCombination(observations, name, combinations);
            }

            /// <summary>
            /// D (transition between patients) = 4+1, 3+4+1, 3+1, 3+1+2, 4+2, 4+1+2, 3+4+1+2, 3+4+2
            /// * 1: Before patient
            /// * 2: Aseptic
            /// * 3: Body fluid
            /// * 4: After patient
            /// </summary>
            /// <param name="observations"></param>
            /// <returns></returns>
            private Combination CreateCombinationD(List<FiveIndicationsObservation> observations)
            {
                var name = "D";
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
                var count = relevantObservations.Count;
                var complied = relevantObservations
                    .Count(o => o.Activity.ActivityType.Code != ActivityTypeConstants.NotPerformed);

                var compliancePercentage = CalculateCompliancePercentage(count, complied);
                var nonCompliancePercentage = CalculateNonCompliancePercentage(count, compliancePercentage);
                var combination = new Combination()
                {
                    Name = $"{combinationName}",
                    Role = observations[0].Role.Name,
                    NumberOfObservations = count,
                    PercentComplied = compliancePercentage,
                    PercentNotComplied = nonCompliancePercentage,
                };
                return combination;
            }

            private static double CalculateCompliancePercentage(int count, int complied)
            {
                if (count == 0 || complied == 0)
                    return 0.0f;
                return (complied / (double)count) * 100;
            }

            private static double CalculateNonCompliancePercentage(int count, double complied)
            {
                if (count > 0)
                {
                    return (100d - complied);
                }

                return 0.0d;
            }

            private static bool MeetsTheCombinationCriteria(IEnumerable<string> indicationTypes, IndicationCombination[] combinationsOfIndication)
            {
                return combinationsOfIndication.Any(combination =>
                    indicationTypes.OrderBy(i => i).SequenceEqual(combination.Code.OrderBy(k => k)));
            }
            #endregion

            private static string DebugObservation(FiveIndicationsObservation o)
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
