using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Reports.FiveIndicators
{
    public class Compliance
    {
        public class Query : IRequest<List<GrafDto>>
        {
            public List<int> FacilityIds { get; set; } = new();
            public string Interval { get; set; }
            public int FromMonth { get; set; }
            public int FromYear { get; set; }
            public int FromQuarter { get; set; }
            public int ToMonth { get; set; }
            public int ToYear { get; set; }
            public int ToQuarter { get; set; }
            public List<int> RoleIds { get; set; } = new();
            public List<int> DepartmentIds { get; set; } = new();
            public List<int> FacilityTypeIds { get; set; } = new();       
            public List<int> DepartmentTypeIds { get; set; } = new();
            public List<int> UnitIds { get; set; } = new();
            public int TranferredTo { get; set; }
            public AuthorizedRole RoleId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<GrafDto>>
        {
            private readonly HandHygieneContext _context;
            private const string IntervalYear = "year";
            private const string IntervalMonth = "month";
            private const string IntervalQuarter = "quarter";

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<List<GrafDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                DateTime fromDate;
                DateTime toDate;

                if (request.Interval == IntervalMonth)
                {
                    fromDate = new DateTime(request.FromYear, request.FromMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                    toDate = new DateTime(request.ToYear, request.ToMonth, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1); // exclusive
                }
                else if (request.Interval == IntervalQuarter)
                {
                    int fromMonth = ((request.FromQuarter - 1) * 3) + 1; // Q1 = 1, Q2 = 4, Q3 = 7, Q4 = 10
                    int toMonth = ((request.ToQuarter - 1) * 3) + 1;

                    fromDate = new DateTime(request.FromYear, fromMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                    toDate = new DateTime(request.ToYear, toMonth, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(3); // add full quarter (exclusive end)
                }
                else // IntervalYear
                {
                    fromDate = new DateTime(request.FromYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    toDate = new DateTime(request.ToYear + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc); // exclusive end
                }

                var unitIds = await ResolveUnitIdsForCompliance(request, cancellationToken);
                if (unitIds.Count == 0)
                    return new List<GrafDto>(); // or return empty graphs as you prefer

                var fromDateUtc = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

                var observationsInCurrentTimePeriodQuery = _context.FiveIndicationsObservation
                    .AsNoTracking()
                    .Include(f => f.Activity.ActivityType)
                    .Include(f => f.IndicationTypes)
                    .Include(f => f.Role)
                    .Where(f => f.RegisteredTime >= fromDateUtc && f.RegisteredTime < toDateUtc)
                    .Where(f => unitIds.Contains(f.FiveIndicationsSession.OrganisationUnitId));

                if (request.RoleIds?.Any() == true)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => request.RoleIds.Contains(x.Role.Id));
                }

                if (request.TranferredTo == 1 || request.RoleId == AuthorizedRole.Administrator)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => x.FiveIndicationsSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }
                if (request.TranferredTo == 2)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => x.FiveIndicationsSession.TransferStatus.Code != TransferStatusTypeConstants.TransferredToAdmin);
                }

                var observationsInCurrentTimePeriod = await observationsInCurrentTimePeriodQuery.ToListAsync(cancellationToken);
                var complianceGraphData = CreateComplianceGraphData(observationsInCurrentTimePeriod, request.Interval, fromDateUtc, toDateUtc);

                RemoveElementsWithNoRegistrationsAtTheBeginningOfTheSearchPeriod(complianceGraphData);
                RemoveElementsWithNoRecordsAtEndOfSearchPeriod(complianceGraphData);

                var graphPercentage = CreateGraph(complianceGraphData, "Compliance (%)", true);
                var graphCount = CreateGraph(complianceGraphData, "Indications (n)", false);

                return new List<GrafDto> { graphPercentage, graphCount };
            }

            private static GrafDto CreateGraph(List<ComplianceGraphData> complianceGraphData, string title, bool usePercentage)
            {
                List<GrafDataDto> grafData = MapComplianceToGraphData(complianceGraphData, usePercentage);

                var graf = new GrafDto
                {
                    Title = title,
                    GraphDataList = grafData
                };
                return graf;
            }

            private static List<GrafDataDto> MapComplianceToGraphData(List<ComplianceGraphData> complianceGraphDataList, bool usePercentage)
            {
                var graphDataDtoList = new List<GrafDataDto>();
                foreach (var complianceGraphData in complianceGraphDataList)
                {
                    var pointDtoList = new List<PointDto>();
                    foreach (var points in complianceGraphData.Data)
                    {
                        var pointDto = new PointDto()
                        {
                            Name = points.Name,
                            Y = usePercentage ? points.Percent : points.Quantity
                        };
                        pointDtoList.Add(pointDto);
                    }

                    graphDataDtoList.Add(new GrafDataDto()
                    {
                        Name = complianceGraphData.Name,
                        Data = pointDtoList
                    });
                }

                return graphDataDtoList;
            }

            private static List<ComplianceGraphData> CreateComplianceGraphData(List<FiveIndicationsObservation> observationsInCurrentPeriod, string interval, DateTime fromDate, DateTime toDate)
            {
                var complianceAllIndicators = CreateComplianceForAllIndications(interval, observationsInCurrentPeriod, fromDate, toDate);
                var complianceBeforePatient = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Before patient", IndicationTypeConstants.BeforePatient, fromDate, toDate);
                var asepticCompliance = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Aseptic", IndicationTypeConstants.AsepticProcedures, fromDate, toDate);
                var bodyFluidCompliance = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Body fluid", IndicationTypeConstants.BodyFluid, fromDate, toDate);
                var complianceAfterPatient = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "After patient", IndicationTypeConstants.AfterPatient, fromDate, toDate);
                var compliancePatientsSurroundings = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Patient's surroundings", IndicationTypeConstants.PatientsSurroundings, fromDate, toDate);

                return new List<ComplianceGraphData>
                        {
                            complianceAllIndicators,
                            complianceBeforePatient,
                            asepticCompliance,
                            bodyFluidCompliance,
                            complianceAfterPatient,
                            compliancePatientsSurroundings
                        };
            }

            private static ComplianceGraphData CreateComplianceForAllIndications(string interval, List<FiveIndicationsObservation> observationsInTheCurrentTimePeriod, DateTime fromDate, DateTime toDate)
            {
                var complianceForAllIndications = new ComplianceGraphData
                {
                    Name = "All indications"
                };

                var PeriodToDate = fromDate;
                var ListOfPoints = new List<CompliancePoint>();
                while (PeriodToDate < toDate)
                {
                    var PeriodFromDate = PeriodToDate;
                    PeriodToDate = CalculateNextPeriodUntilDate(interval, PeriodToDate);

                    var observationsPeriod = observationsInTheCurrentTimePeriod.Where(o => o.RegisteredTime >= PeriodFromDate && o.RegisteredTime < PeriodToDate);

                    var indications = observationsPeriod.Select(x => x.IndicationTypes);
                    decimal numberOfIndicators = indications.Sum(item => item.Count);

                    var compliedIndications = observationsPeriod.Where(o => o.Activity.ActivityType.Code == ActivityTypeConstants.Handwash ||
                                                                                                        o.Activity.ActivityType.Code == ActivityTypeConstants.Disinfection)
                                                                                            .Select(o => o.IndicationTypes);

                    decimal numberOfCompliedIndications = compliedIndications.Sum(item => item.Count);

                    string periodName = CalculatePeriodName(interval, PeriodFromDate);
                    var point = CreatePoint(numberOfIndicators, numberOfCompliedIndications, periodName);
                    ListOfPoints.Add(point);
                }

                complianceForAllIndications.Data = ListOfPoints;

                return complianceForAllIndications;
            }

            private static ComplianceGraphData CreateComplianceGraphDataForIndicator(string interval, List<FiveIndicationsObservation> observationsInCurrentPeriod, string title, string indicationType, DateTime fromDate, DateTime ToDate)
            {
                var compliance = new ComplianceGraphData
                {
                    Name = title
                };

                var observationsWithIndicationType = observationsInCurrentPeriod.Where(o => o.IndicationTypes.Any(i => i.Code == indicationType)).ToList();
                compliance.Data = CreateGraphDataForIndication(observationsWithIndicationType, interval, fromDate, ToDate);
                return compliance;
            }

            private static List<CompliancePoint> CreateGraphDataForIndication(List<FiveIndicationsObservation> observationsInRelevantTimePeriod, string interval, DateTime fromDate, DateTime ToDate)
            {
                var periodToDate = fromDate;
                var pointList = new List<CompliancePoint>();
                while (periodToDate < ToDate)
                {
                    var periodFromDate = periodToDate;
                    periodToDate = CalculateNextPeriodUntilDate(interval, periodToDate);

                    var observationsInPeriod = observationsInRelevantTimePeriod.Where(o => o.RegisteredTime >= periodFromDate && o.RegisteredTime < periodToDate);
                    var compliedObservationsInPeriod = observationsInPeriod.Where(o => o.Activity.ActivityType.Code == ActivityTypeConstants.Handwash || o.Activity.ActivityType.Code == ActivityTypeConstants.Disinfection);

                    decimal numberOfObservationsInPeriod = observationsInPeriod.Count();
                    decimal numberOfCompliedObservationsInPeriod = compliedObservationsInPeriod.Count();

                    string periodName = CalculatePeriodName(interval, periodFromDate);
                    var point = CreatePoint(numberOfObservationsInPeriod, numberOfCompliedObservationsInPeriod, periodName);

                    pointList.Add(point);
                }

                return pointList;
            }

            private static CompliancePoint CreatePoint(decimal numberOfObservationsInPeriod, decimal numberOfCompliantObservationsInPeriod, string periodName)
            {
                decimal compliancePercentage = 0;

                if (numberOfObservationsInPeriod > 0)
                {
                    compliancePercentage = (numberOfCompliantObservationsInPeriod / numberOfObservationsInPeriod) * 100;
                }

                CompliancePoint points = new()
                {
                    Name = periodName,
                    Quantity = numberOfObservationsInPeriod,
                    Percent = compliancePercentage
                };
                return points;
            }

            private static string CalculatePeriodName(string interval, DateTime periodFromDate)
            {
                if (interval == IntervalMonth)
                {
                    return $"{periodFromDate:MMMM yyyy}"; // e.g. "January 2024"
                }
                else if (interval == IntervalQuarter)
                {
                    int quarter = ((periodFromDate.Month - 1) / 3) + 1; // 1-based quarter
                    return $"Q{quarter} {periodFromDate.Year}";
                }
                else if (interval == IntervalYear)
                {
                    return $"{periodFromDate.Year}";
                }

                return "";
            }

            private static DateTime CalculateNextPeriodUntilDate(string interval, DateTime periodToDate)
            {
                if (interval == IntervalMonth)
                {
                    return periodToDate.AddMonths(1);
                }
                else if (interval == IntervalQuarter)
                {
                    return periodToDate.AddMonths(3);
                }
                else if (interval == IntervalYear)
                {
                    return periodToDate.AddYears(1);
                }
                else
                {
                    throw new ValidationException("InvalidIntervalValue", interval);
                }
            }

            private static void RemoveElementsWithNoRegistrationsAtTheBeginningOfTheSearchPeriod(List<ComplianceGraphData> complianceGraphData)
            {
                var allIndicatorsGraphData = complianceGraphData.First(x => x.Name == "All indications");

                var numberOfElementsToBeRemoved = 0;
                foreach (var data in allIndicatorsGraphData.Data)
                {
                    if (data.Quantity > 0)
                    {
                        break;
                    }
                    numberOfElementsToBeRemoved++;
                }

                foreach (var grafdata in complianceGraphData)
                {
                    grafdata.Data.RemoveRange(0, numberOfElementsToBeRemoved);
                }
            }

            private static void RemoveElementsWithNoRecordsAtEndOfSearchPeriod(List<ComplianceGraphData> complianceGraphData)
            {
                var allIndicatorsGraphData = complianceGraphData.First(x => x.Name == "All indications");

                var numberOfElementsToBeRemoved = 0;

                var numberOfElements = allIndicatorsGraphData.Data.Count;
                for (var i = numberOfElements; i > 0; i--)
                {
                    if (allIndicatorsGraphData.Data[i - 1].Quantity > 0)
                    {
                        break;
                    }

                    numberOfElementsToBeRemoved++;
                }

                foreach (var grafdata in complianceGraphData)
                {
                    int index = numberOfElements - numberOfElementsToBeRemoved;
                    grafdata.Data.RemoveRange(index, numberOfElementsToBeRemoved);
                }
            }

            private sealed record OuRow(int Id, int? ParentId, string Level, int? TypeId);

            private async Task<List<OuRow>> LoadOrgUnitsAsync(CancellationToken ct)
            {
                return await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Select(x => new OuRow(
                        x.Id,
                        x.ParentId,
                        x.LevelRef.Level,   // "Facility" / "Department" / "Unit"
                        x.TypeId
                    ))
                    .ToListAsync(ct);
            }

            private static Dictionary<int, List<int>> BuildChildrenByParent(List<OuRow> ous)
            {
                var dict = new Dictionary<int, List<int>>();
                foreach (var ou in ous)
                {
                    if (!ou.ParentId.HasValue) continue;
                    if (!dict.TryGetValue(ou.ParentId.Value, out var kids))
                        dict[ou.ParentId.Value] = kids = new List<int>();
                    kids.Add(ou.Id);
                }
                return dict;
            }

            private static List<int> GetDescendantsByLevel(
                int rootId,
                string targetLevel,
                List<OuRow> ous,
                Dictionary<int, List<int>> childrenByParent)
            {
                var levelById = ous.ToDictionary(x => x.Id, x => x.Level);

                var result = new List<int>();
                var stack = new Stack<int>();
                stack.Push(rootId);

                while (stack.Count > 0)
                {
                    var id = stack.Pop();

                    if (levelById.TryGetValue(id, out var level) && level == targetLevel)
                        result.Add(id);

                    if (childrenByParent.TryGetValue(id, out var kids))
                        foreach (var k in kids) stack.Push(k);
                }

                return result;
            }

            private async Task<HashSet<int>> ResolveUnitIdsForCompliance(Compliance.Query request, CancellationToken ct)
            {
                var ous = await LoadOrgUnitsAsync(ct);
                var childrenByParent = BuildChildrenByParent(ous);

                var facilityIds = FilterFacilityIdsByType(ous, request);
                var unitIds = GetExplicitUnitIds(ous, request);
                unitIds = MergeFacilityUnits(unitIds, facilityIds, ous, childrenByParent);
                unitIds = MergeDepartmentUnits(unitIds, request, ous, childrenByParent);

                return unitIds;
            }

            private static List<int> FilterFacilityIdsByType(
                List<OuRow> ous,
                Compliance.Query request)
            {
                var facilityIds = request.FacilityIds ?? new List<int>();

                if (request.FacilityTypeIds == null || request.FacilityTypeIds.Count == 0)
                    return facilityIds;

                var allowedFacilities = ous
                    .Where(o => o.Level == OrganisationUnitLevels.Facility
                             && o.TypeId.HasValue
                             && request.FacilityTypeIds.Contains(o.TypeId.Value))
                    .Select(o => o.Id)
                    .ToHashSet();

                return facilityIds.Where(allowedFacilities.Contains).ToList();
            }

            private static HashSet<int> GetExplicitUnitIds(
                List<OuRow> ous,
                Compliance.Query request)
            {
                if (request.UnitIds == null || request.UnitIds.Count == 0)
                    return new HashSet<int>();

                return ous
                    .Where(o => o.Level == OrganisationUnitLevels.Unit && request.UnitIds.Contains(o.Id))
                    .Select(o => o.Id)
                    .ToHashSet();
            }

            private static HashSet<int> MergeFacilityUnits(
                HashSet<int> currentUnitIds,
                List<int> facilityIds,
                List<OuRow> ous,
                Dictionary<int, List<int>> childrenByParent)
            {
                if (facilityIds == null || facilityIds.Count == 0)
                    return currentUnitIds;

                var facilityUnitIds = new HashSet<int>();

                foreach (var facilityId in facilityIds)
                {
                    foreach (var unitId in GetDescendantsByLevel(
                        facilityId,
                        OrganisationUnitLevels.Unit,
                        ous,
                        childrenByParent))
                    {
                        facilityUnitIds.Add(unitId);
                    }
                }

                return currentUnitIds.Count > 0
                    ? currentUnitIds.Intersect(facilityUnitIds).ToHashSet()
                    : facilityUnitIds;
            }

            private static HashSet<int> MergeDepartmentUnits(
                HashSet<int> currentUnitIds,
                Compliance.Query request,
                List<OuRow> ous,
                Dictionary<int, List<int>> childrenByParent)
            {
                if (request.DepartmentIds == null || request.DepartmentIds.Count == 0)
                    return currentUnitIds;

                var deptUnitIds = new HashSet<int>();

                foreach (var deptId in request.DepartmentIds)
                {
                    if (!IsMatchingDepartmentType(deptId, request, ous))
                        continue;

                    foreach (var unitId in GetDescendantsByLevel(
                        deptId,
                        OrganisationUnitLevels.Unit,
                        ous,
                        childrenByParent))
                    {
                        deptUnitIds.Add(unitId);
                    }
                }

                return currentUnitIds.Count > 0
                    ? currentUnitIds.Intersect(deptUnitIds).ToHashSet()
                    : deptUnitIds;
            }

            private static bool IsMatchingDepartmentType(
                int deptId,
                Compliance.Query request,
                List<OuRow> ous)
            {
                if (request.DepartmentTypeIds == null || request.DepartmentTypeIds.Count == 0)
                    return true;

                var deptRow = ous.FirstOrDefault(x => x.Id == deptId);
                if (deptRow is null)
                    return false;

                if (deptRow.Level != OrganisationUnitLevels.Department)
                    return false;

                return deptRow.TypeId.HasValue && request.DepartmentTypeIds.Contains(deptRow.TypeId.Value);
            }
        }

    }
}
