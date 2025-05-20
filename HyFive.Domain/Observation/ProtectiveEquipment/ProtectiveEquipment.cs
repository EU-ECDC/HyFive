using System;
using System.Collections.Generic;

namespace HyFive.Domain.Observation.ProtectiveEquipment
{
    public class ProtectiveEquipment
    {
        public int Id { get; set; }
        public bool WasUsed { get; set; }
        public bool IsRequired { get; set; }
        public ProtectiveEquipmentType EquipmentType { get; set; }
        public ICollection<MisuseType> MisuseTypes { get; set; }
        public bool WasUsedCorrectly { get; set; }
        public string Comment { get; set; }
        public ProtectiveEquipmentObservation ProtectiveEquipmentObservation { get; set; }
    }
}