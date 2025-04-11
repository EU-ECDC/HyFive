using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipmentOverviewReport
    {
        public string EquipmentName { get; set; }
        public List<string> MisuseTypes { get; set; }
        public bool WasUsed { get; set; }
        public bool WasUsedCorrectly { get; set; }
        public bool IsRequired { get; set; }
        public string Comment { get; set; }
    }
}
