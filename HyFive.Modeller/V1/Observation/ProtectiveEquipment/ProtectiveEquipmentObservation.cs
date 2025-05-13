using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.ProtectiveEquipment
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipmentObservation : Observation
    {
        public List<ProtectiveEquipment> ProtectiveEquipmentList { get; set; }
        public ProtectiveEquipmentSettingType SettingType { get; set; }
    }
}