using System.Collections.Generic;
using HyFive.Domain.Session;

namespace HyFive.Domain.Observation.ProtectiveEquipment
{
    public class ProtectiveEquipmentObservation : Observation
    {
        public List<ProtectiveEquipment> ProtectiveEquipmentList { get; set; }
        public ProtectiveEquipmentSettingType SettingType { get; set; }
        public ProtectiveEquipmentSession ProtectiveEquipmentSession { get; set; }
    }
}