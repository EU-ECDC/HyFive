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
            public List<int> FacilityTypeIds { get; set; } = new();       // Optional: add if needed
            public List<int> DepartmentTypeIds { get; set; } = new();
            public int TranferredTo { get; set; }
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

                var fromDateUtc = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
                var toDateUtc = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);
                var observationsInCurrentTimePeriodQuery = _context.FiveIndicationsObservation.Include(f => f.Activity.ActivityType)
                                                                                      .Include(f => f.IndicationTypes)
                                                                                      .Include(f => f.Role)
                                                                                      .AsNoTracking()
                                                                                      .Where(f => f.RegisteredTime >= fromDateUtc &&
                                                                                                  f.RegisteredTime < toDateUtc &&
                                                                                                  request.FacilityIds.Contains(f.FiveIndicationsSession.Department.Facility.Id));

                if (request.FacilityTypeIds?.Any() == true)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => request.FacilityTypeIds.Contains(x.FiveIndicationsSession.Department.Facility.FacilityType.Id));
                }

                if (request.RoleIds?.Any() == true)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => request.RoleIds.Contains(x.Role.Id));
                }

                if (request.DepartmentIds?.Any() == true)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => request.DepartmentIds.Contains(x.FiveIndicationsSession.Department.Id));
                }

                if (request.DepartmentTypeIds?.Any() == true)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => request.DepartmentTypeIds.Contains(x.FiveIndicationsSession.Department.DepartmentType.Id));
                }

                if (request.TranferredTo == 1)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => x.FiveIndicationsSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }
                else if (request.TranferredTo == 2)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => x.FiveIndicationsSession.TransferStatus.Code != TransferStatusTypeConstants.TransferredToAdmin);
                }

                var observationsInCurrentTimePeriod = await observationsInCurrentTimePeriodQuery.ToListAsync(cancellationToken);
                var complianceGraphData = CreateComplianceGraphData(observationsInCurrentTimePeriod, request.Interval, fromDateUtc, toDateUtc);

                RemoveElementsWithNoRegistrationsAtTheBeginningOfTheSearchPeriod(complianceGraphData);
                RemoveElementsWithNoRecordsAtEndOfSearchPeriod(complianceGraphData);

                var graphPercentage = CreateGraph(complianceGraphData, "Compliance (%)", true);
                var graphCount = CreateGraph(complianceGraphData, "Compliance (N)", false);

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
        }
    }
}
