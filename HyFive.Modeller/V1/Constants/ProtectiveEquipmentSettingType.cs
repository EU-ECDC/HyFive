using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class ProtectiveEquipmentSettingType
    {
        [TsProperty(Constant = true)]
        public static string BasicInfectionControlRoutines =  "BASIC_INFECTIONcONTROLROUTINES";

        [TsProperty(Constant = true)]
        public static string ContactTransmission = "CONTACTTRANSMISSION";
        
        [TsProperty(Constant = true)]
        public static string DropletTransmission = "DROPLETTRANSMISSION";

        [TsProperty(Constant = true)]
        public static string AirborneTransmission = "AIRBORNETRANSMISSION";
    }
}
