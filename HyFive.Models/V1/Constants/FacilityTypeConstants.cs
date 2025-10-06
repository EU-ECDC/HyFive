using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public static class FacilityTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string PrimaryCare = "PRIMARY_CARE";

        [TsProperty(Constant = true)]
        public static string SecondaryCare = "SECONDARY_CARE";

        [TsProperty(Constant = true)]
        public static string TertiaryCare = "TERTIARY_CARE";

        [TsProperty(Constant = true)]
        public static string SpecialisedCare = "SPECIALISED_CARE";

        [TsProperty(Constant = true)]
        public static string LongTermCare = "LONG_TERM_CARE";
    }
}