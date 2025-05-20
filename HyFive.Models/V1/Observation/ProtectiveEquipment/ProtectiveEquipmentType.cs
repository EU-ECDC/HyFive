using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.ProtectiveEquipment
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipmentType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsRequired { get; set; }
        public List<MisuseType> MisuseTypes { get; set; }
    }
}