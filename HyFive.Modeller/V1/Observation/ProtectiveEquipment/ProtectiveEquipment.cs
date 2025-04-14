using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Models.V1.Observation.ProtectiveEquipment
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipment
    {
        public int Id { get; set; }
        public bool WasUsed { get; set; }
        public bool IsRequired { get; set; }
        public ProtectiveEquipmentType EquipmentType { get; set; }
        public List<IncorrectType> IncorrectTypes { get; set; }
        public bool WasUsedCorrectly { get; set; }
        public string Comment { get; set; }
    }
}