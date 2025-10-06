using System;
using System.Collections.Generic;
using HyFive.Domain.Observation;

namespace HyFive.Services.Reports.HandJewelry
{
    public class JewelryReportForJewelryTypeAndRole
    {
        public string Department { get; set; }
        public string Facility { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToTime { get; set; }
        public ReportForUnit ReportForDepartment { get; set; }
        public ReportForUnit ReportForFacility { get; set; }
    }

    public class ReportForUnit
    {
        public List<RoleCountForJewelryType> RoleJewelrySummaryList { get; set; }
        public List<ObservationsByRole> ListOfObservationsByRole { get; set; }
    }

    public class RoleCountForJewelryType
    {
        public HandJewelryType JewelryType { get; set; }
        public List<CountByRole> CountByRoleList { get; set; }
    }

    public class CountByRole
    {
        public int Count { get; set; }
        public string Role { get; set; }
    }

    public class ObservationsByRole
    {
        public int Count { get; set; }
        public string Role { get; set; }
    }
}
