using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public static class InstitutionTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string Hospital = "HOSPITAL";

        [TsProperty(Constant = true)]
        public static string NursingHome = "NURSING_HOME";
    }
}