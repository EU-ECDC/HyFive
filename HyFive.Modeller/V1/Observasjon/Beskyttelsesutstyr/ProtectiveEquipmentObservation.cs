using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ProtectiveEquipmentObservation : Observasjon
    {
        public List<ProtectiveEquipment> ProtectiveEquipmentList { get; set; }
        public ProtectiveEquipmentSettingType SettingType { get; set; }
    }
}