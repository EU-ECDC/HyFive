using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class ProtectiveEquipmentSettingTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string BasicIsolationRoutines =  "BASIC_ISOLATIONROUTINES";

        [TsProperty(Constant = true)]
        public static string ContactTransmission = "CONTACTTRANSMISSION";
        
        [TsProperty(Constant = true)]
        public static string DropletTransmission = "DROPLETTRANSMISSION";

        [TsProperty(Constant = true)]
        public static string AirborneTransmission = "AIRBORNETRANSMISSION";
    }
}
