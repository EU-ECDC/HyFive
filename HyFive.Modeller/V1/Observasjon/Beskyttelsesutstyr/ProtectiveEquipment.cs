using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipment
    {
        public int Id { get; set; }
        public bool WasUsed { get; set; }
        public bool IsRequired { get; set; }
        public ProtectiveEquipmentType EquipmentType { get; set; }
        public List<MisuseType> MisuseTypes { get; set; }
        public bool WasUsedCorrectly { get; set; }
        public string Comment { get; set; }
    }
}