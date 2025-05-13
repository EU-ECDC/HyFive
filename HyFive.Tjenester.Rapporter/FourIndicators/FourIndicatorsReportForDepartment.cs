using System;
using System.Collections.Generic;

namespace HyFive.Services.Reports.FourIndicators
{
    public class FourIndicatorsReportForDepartment
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public FourIndicatorsReport Department { get; set; } = new FourIndicatorsReport();
        public FourIndicatorsReport Institution { get; set; } = new FourIndicatorsReport();
        public FourIndicatorsReport ComparableDepartments { get; set; } = new FourIndicatorsReport();
        public List<FourIndicatorsReport> Clinics { get; set; } = new List<FourIndicatorsReport>();

        public void SetDisplayTimestamps(DateTime startTime, DateTime endTime)
        {
            FromDate = startTime;
            ToDate = endTime;
            Department.FromDate = startTime;
            Department.ToDate = endTime;
            Institution.FromDate = startTime;
            Institution.ToDate = endTime;
            ComparableDepartments.FromDate = startTime;
            ComparableDepartments.ToDate = endTime;
            Clinics.ForEach(k =>
            {
                k.FromDate = startTime;
                k.ToDate = endTime;
            });
        }
    }
    


    public class FourIndicatorsReport
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
