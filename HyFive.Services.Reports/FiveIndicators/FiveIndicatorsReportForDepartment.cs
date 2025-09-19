using System;
using System.Collections.Generic;

namespace HyFive.Services.Reports.FiveIndicators
{
    public class FiveIndicatorsReportForDepartment
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public FiveIndicatorsReport Department { get; set; } = new FiveIndicatorsReport();
        public FiveIndicatorsReport Facility { get; set; } = new FiveIndicatorsReport();
        public FiveIndicatorsReport ComparableDepartments { get; set; } = new FiveIndicatorsReport();
        public List<FiveIndicatorsReport> Clinics { get; set; } = new List<FiveIndicatorsReport>();

        public void SetDisplayTimestamps(DateTime startTime, DateTime endTime)
        {
            FromDate = startTime;
            ToDate = endTime;
            Department.FromDate = startTime;
            Department.ToDate = endTime;
            Facility.FromDate = startTime;
            Facility.ToDate = endTime;
            ComparableDepartments.FromDate = startTime;
            ComparableDepartments.ToDate = endTime;
            Clinics.ForEach(k =>
            {
                k.FromDate = startTime;
                k.ToDate = endTime;
            });
        }
    }
    


    public class FiveIndicatorsReport
    {
        public string Unit { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<RoleWithCombinationsReport> Roles { get; set; } = new List<RoleWithCombinationsReport>();
        public string[] DebugObservationsStringList { get; set; }
        public int NumberOfObservations { get; set; }
    }

    public class RoleWithCombinationsReport
    {
        public string Name { get; set; } // Nurse
        public int TotalNumberOfObservations { get; set; }
        public List<Combination> Combinations { get; set; } = new List<Combination>();
        public string AverageHandwashingTime { get; set; }
        public string AverageHandSanitizingTime { get; set; }
        public byte[] Chart { get; set; }
    }

    public class Combination
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public int NumberOfObservations { get; set; }
        public double PercentComplied {get;set; }
        public double PercentNotComplied { get; set; }
    }
}
