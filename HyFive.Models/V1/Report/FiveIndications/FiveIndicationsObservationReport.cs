using System;

namespace HyFive.Models.V1.Report.FiveIndications
{
    public class FiveIndicationsObservationReport
    {
        public Guid SessionId { get; set; }
        public Guid ObservationId { get; set; }
        public string SessionCreatedTime { get; set; }
        public string ObservationRegisteredTime { get; set; }
        public string Observer { get; set; }
        public string FacilityAbbreviation { get; set; }
        public string Facility { get; set; }
        public string FacilityType { get; set; }
        public string FacilityTypeCode { get; set; }
        public string Department { get; set; }
        public string DepartmentType { get; set; }
        public string RoleName { get; set; }
        public string Activity { get; set; }
        public string Indications { get; set; }
        public int SecondsUsed { get; set; }
        public bool TimingWasPerformed { get; set; }
        public bool? GlovesUsed { get; set; }
        public string TransferStatus { get; set; }
        public string ObservationComment { get; set; }
        public string SessionComment { get; set; }
        public string HealthcareOrganization { get; set; }
        public string RegionalHealthcareOrganization { get; set; }
        public string MunicipalityNumber { get; set; }
        public string Municipality { get; set; }
    }
}
