using System;

namespace HyFive.Models.V1.Report.Beskyttelsesutstyr
{
    public class PPEObservationReport
    {
		public Guid SessionId { get; set; }
        public Guid ObservationId { get; set; }
        public string CreateSessionTime { get; set; }
        public string ObservationRegisteredTime { get; set; }
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
        public bool WasUsed { get; set; }
        public bool WasUsedCorrectly { get; set; }
        public bool IsIndicated { get; set; }
        public string ProtectiveEquipmentCode { get; set; }
        public string ProtectiveEquipment { get; set; }
        public string ProtectiveEquipmentSettingCode { get; set; }
        public string ProtectiveEquipmentSetting { get; set; }
        public string Misuse { get; set; }
        public string HealthTrust { get; set; }
        public string RegionalHealthTrust { get; set; }
        public string MunicipalityNumber { get; set; }
        public string Municipality { get; set; }
    }
}
