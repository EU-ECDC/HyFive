using System.Collections.Generic;

namespace HyFive.Domain.Observation.ProtectiveEquipment
{
    public class ProtectiveEquipmentType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
      
        public List<MisuseType> MisuseTypes { get; set; }
        public ICollection<ProtectiveEquipmentSettingTypeProtectiveEquipmentType> ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes { get; set; }
    }
}