using System.Collections.Generic;

namespace HyFive.Domain.Observation.ProtectiveEquipment
{
    public class ProtectiveEquipmentSettingTypeProtectiveEquipmentType
    {
        public int ProtectiveEquipmentTypeId { get; set; }
        public ProtectiveEquipmentType ProtectiveEquipmentType { get; set; }
        
        public int ProtectiveEquipmentSettingTypeId { get; set; }
        public ProtectiveEquipmentSettingType ProtectiveEquipmentSettingType { get; set; }
        
        public bool IsDefault { get; set; }
    }
}