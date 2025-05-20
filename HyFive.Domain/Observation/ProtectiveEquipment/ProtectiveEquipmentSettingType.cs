using System.Collections.Generic;

namespace HyFive.Domain.Observation.ProtectiveEquipment
{
    public class ProtectiveEquipmentSettingType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ICollection<ProtectiveEquipmentSettingTypeProtectiveEquipmentType> ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes { get; set; }
    }
}