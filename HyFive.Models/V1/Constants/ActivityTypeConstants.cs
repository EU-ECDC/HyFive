using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public static class ActivityTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string Disinfection = "DISINFECTION";
        [TsProperty(Constant = true)]
        public static string Handwash = "HANDWASH";
        [TsProperty(Constant = true)]
        public static string NotPerformed = "NOT_PERFORMED";
        [TsProperty(Constant = true)]
        public static string NotRegistered = "NOT_REGISTERED";
    }
}