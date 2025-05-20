using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class HandHygieneAfterGloveUseTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string Yes = "YES";

        [TsProperty(Constant = true)]
        public static string No = "NO";

        [TsProperty(Constant = true)]
        public static string NotIndicated = "NOT_INDICATED";
    }
}
