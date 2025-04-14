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
        public string InstitutionAbbreviation { get; set; }
        public string Institution { get; set; }
        public string InstitutionType { get; set; }
        public string InstitutionTypeCode { get; set; }
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
        public string HealthcareTrust { get; set; }
        public string RegionalHealthcareTrust { get; set; }
        public string MunicipalityNumber { get; set; }
        public string Municipality { get; set; }
    }
}
