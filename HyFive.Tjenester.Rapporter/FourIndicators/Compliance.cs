using HyFive.DataAccess;
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

namespace HyFive.Services.Reports.FourIndicators
{
    public class Compliance
    {
        public class Query : IRequest<List<GrafDto>>
        {
            public int InstitutionId { get; set; }
            public string Interval { get; set; }
            public int FromMonth { get; set; }
            public int FromYear { get; set; }
            public int ToMonth { get; set; }
            public int ToYear { get; set; }
            public int? RoleId { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<GrafDto>>
        {
            private readonly HandHygieneContext _context;
            private const string IntervalWeek = "week";
            private const string IntervalMonth = "month";
            private const string IntervalYear = "year";

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<List<GrafDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var fromDate = new DateTime(request.FromYear, 1, 1);
                var ToDate = new DateTime(request.ToYear, 1, 1);
                if (request.Interval == IntervalWeek)
                {
                    fromDate = fromDate.AddMonths(request.FromMonth - 1);
                    ToDate = ToDate.AddMonths(request.ToMonth);
                }
                else
                {
                    ToDate = ToDate.AddYears(1);
                }

                var observationsInCurrentTimePeriodQuery = _context.FourIndicationsObservation.Include(f => f.Activity.ActivityType)
                                                                                      .Include(f => f.IndicationTypes)
                                                                                      .Include(f => f.Role)
                                                                                      .AsNoTracking()
                                                                                      .Where(f => f.RegisteredTime >= fromDate &&
                                                                                                  f.RegisteredTime < ToDate &&
                                                                                                  f.FourIndicationsSession.Department.Institution.Id == request.InstitutionId);

                if (request.RoleId != null)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => x.Role.Id == request.RoleId);
                }

                if (request.DepartmentId != null)
                {
                    observationsInCurrentTimePeriodQuery = observationsInCurrentTimePeriodQuery.Where(x => x.FourIndicationsSession.Department.Id == request.DepartmentId);
                }

                var observationsInCurrentTimePeriod = observationsInCurrentTimePeriodQuery.ToList();
                var complianceGraphData = CreateComplianceGraphData(observationsInCurrentTimePeriod, request.Interval, fromDate, ToDate);

                RemoveElementsWithNoRegistrationsAtTheBeginningOfTheSearchPeriod(complianceGraphData);
                RemoveElementsWithNoRecordsAtEndOfSearchPeriod(complianceGraphData);

                var graphPercentage = CreateGraph(complianceGraphData, "Compliance (%)", true);
                var graphCount = CreateGraph(complianceGraphData, "Compliance number", false);

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

            private static List<ComplianceGraphData> CreateComplianceGraphData(List<FourIndicationsObservation> observationsInCurrentPeriod, string interval, DateTime fromDate, DateTime toDate)
            {
                var complianceAllIndicators = CreateComplianceForAllIndications(interval, observationsInCurrentPeriod, fromDate, toDate);
                var complianceBeforePatient = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Before patient", IndicationTypeConstants.BeforePatient, fromDate, toDate);
                var asepticCompliance = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Aseptic", IndicationTypeConstants.AsepticProcedures, fromDate, toDate);
                var bodyFluidCompliance = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "Body fluid", IndicationTypeConstants.BodyFluid, fromDate, toDate);
                var complianceAfterPatient = CreateComplianceGraphDataForIndicator(interval, observationsInCurrentPeriod, "After patient", IndicationTypeConstants.AfterPatient, fromDate, toDate);

                return new List<ComplianceGraphData>
                        {
                            complianceAllIndicators,
                            complianceBeforePatient,
                            asepticCompliance,
                            bodyFluidCompliance,
                            complianceAfterPatient
                        };
            }

            private static ComplianceGraphData CreateComplianceForAllIndications(string interval, List<FourIndicationsObservation> observationsInTheCurrentTimePeriod, DateTime fromDate, DateTime toDate)
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

                    var observasjonerIPeriode = observationsInTheCurrentTimePeriod.Where(o => o.RegisteredTime >= PeriodFromDate && o.RegisteredTime < PeriodToDate);

                    var indikasjoner = observasjonerIPeriode.Select(x => x.IndicationTypes);
                    decimal antallIndikasjoner = indikasjoner.Sum(item => item.Count);

                    var etterlevdeIndikasjoner = observasjonerIPeriode.Where(o => o.Activity.ActivityType.Code == ActivityTypeConstants.Handwash ||
                                                                                                        o.Activity.ActivityType.Code == ActivityTypeConstants.Disinfection)
                                                                                            .Select(o => o.IndicationTypes);

                    decimal antallEtterlevdeIndikasjoner = etterlevdeIndikasjoner.Sum(item => item.Count);

                    string periodenavn = CalculatePeriodName(interval, PeriodFromDate, PeriodToDate);
                    var punkt = CreatePoint(antallIndikasjoner, antallEtterlevdeIndikasjoner, periodenavn);
                    ListOfPoints.Add(punkt);
                }

                complianceForAllIndications.Data = ListOfPoints;

                return complianceForAllIndications;
            }

            private static ComplianceGraphData CreateComplianceGraphDataForIndicator(string interval, List<FourIndicationsObservation> observationsInCurrentPeriod, string title, string indicationType, DateTime fromDate, DateTime ToDate)
            {
                var compliance = new ComplianceGraphData
                {
                    Name = title
                };

                var observationsWithIndicationType = observationsInCurrentPeriod.Where(o => o.IndicationTypes.Any(i => i.Code == indicationType)).ToList();
                compliance.Data = CreateGraphDataForIndication(observationsWithIndicationType, interval, fromDate, ToDate);
                return compliance;
            }

            private static List<CompliancePoint> CreateGraphDataForIndication(List<FourIndicationsObservation> observationsInRelevantTimePeriod, string interval, DateTime fromDate, DateTime ToDate)
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

                    string periodName = CalculatePeriodName(interval, periodFromDate, periodToDate);
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

            private static string CalculatePeriodName(string interval, DateTime periodFromDate, DateTime periodToDate)
            {
                if (interval == IntervalWeek)
                {
                    return $"{periodFromDate:d MMM} - {periodToDate:d MMM yy}";
                }
                else if (interval == IntervalMonth)
                {
                    return $"{periodFromDate:MMMM yy}";
                }
                else if (interval == IntervalYear)
                {
                    return $"{periodFromDate:MMM yy} - {periodToDate.AddMonths(-1):MMM yy}";
                }

                return "";
            }

            private static DateTime CalculateNextPeriodUntilDate(string interval, DateTime periodToDate)
            {
                if (interval == IntervalWeek)
                {
                    return periodToDate.AddDays(7);
                }
                else if (interval == IntervalMonth)
                {
                    return periodToDate.AddMonths(1);
                }
                else if (interval == IntervalYear)
                {
                    return periodToDate.AddMonths(4);
                }
                else
                {
                    throw new Exception("The interval must be week, month, or quarter.");
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

            private void RemoveElementsWithNoRecordsAtEndOfSearchPeriod(List<ComplianceGraphData> complianceGraphData)
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
