using System.Collections.Generic;

namespace HyFive.Domain.Observation.ProtectiveEquipment
{
    public class MisuseType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProtectiveEquipmentType ProtectiveEquipmentType { get; set; }
        public ICollection<ProtectiveEquipment> ProtectiveEquipment { get; set; }
    }
}