using System;

namespace HyFive.Models.V1.Report.Glove
{
    public class GloveObservationReport
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
        public bool GlovesUsed { get; set; }
        public string TransferStatus { get; set; }
        public string HandHygieneAfterGloveUseCode { get; set; }
        public string GlovesWithoutIndicationCode { get; set; }
        public string GlovesWithIndicationCode { get; set; }
        public string ObservationComment { get; set; }
        public string SessionComment { get; set; }
        public string City { get; set; }
    }
}
