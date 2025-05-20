using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.ProtectiveEquipment
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipmentSettingType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ProtectiveEquipmentType> EquipmentTypes { get; set; }
    }
}