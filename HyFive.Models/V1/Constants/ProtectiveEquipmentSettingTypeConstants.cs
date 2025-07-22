using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class ProtectiveEquipmentSettingTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string BasicIsolationRoutines =  "BASIC_ISOLATION_ROUTINES";

        [TsProperty(Constant = true)]
        public static string ContactTransmission = "CONTACT_TRANSMISSION";
        
        [TsProperty(Constant = true)]
        public static string DropletTransmission = "DROPLET_TRANSMISSION";

        [TsProperty(Constant = true)]
        public static string AirborneTransmission = "AIRBORNE_TRANSMISSION";
    }
}
