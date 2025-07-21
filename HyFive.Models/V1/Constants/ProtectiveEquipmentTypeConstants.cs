using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class ProtectiveEquipmentTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string Gloves = "GLOVES";

        [TsProperty(Constant = true)]
        public static string IsolationGown = "ISOLATION_GOWN";

        [TsProperty(Constant = true)]
        public static string FaceMask = "FACE_MASK";
        
        [TsProperty(Constant = true)]
        public static string EyeProtection = "EYE_PROTECTION";

        [TsProperty(Constant = true)]
        public static string RespiratoryProtection = "RESPIRATORY_PROTECTION";

        [TsProperty(Constant = true)]
        public static string Hood = "HOOD";

        [TsProperty(Constant = true)]
        public static string PlasticApron = "PLASTIC_APRON";

        [TsProperty(Constant = true)]
        public static string CareGown = "CARE_GOWN";
    }
}
