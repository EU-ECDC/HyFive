using System;

namespace HyFive.Models.V1.Report.HandJewelry
{
    public class HandJewelryObservationReport
    {
		public Guid SessionId { get; set; }
        public Guid ObservationId { get; set; }
        public string SessionCreationTime { get; set; }
        public string ObservationRegistrationTime { get; set; }
        public string Observer { get; set; }
        public string InstitutionAbbreviation { get; set; }
        public string Institution { get; set; }
        public string InstitutionType { get; set; }
        public string InstitutionTypeCode { get; set; }
        public string Department { get; set; }
        public string DepartmentType { get; set; }
        public string RoleName { get; set; }
        public string TransferStatus { get; set; }
        public string ObservationComment { get; set; }
        public string SessionComment { get; set; }
        public string HandJewelryTypes { get; set; }
        public string HandJewelryTypeCodes { get; set; }
        public string HealthOrganization { get; set; }
        public string RegionalHealthOrganization { get; set; }
        public string MunicipalityNumber { get; set; }
        public string Municipality { get; set; }
    }
}
