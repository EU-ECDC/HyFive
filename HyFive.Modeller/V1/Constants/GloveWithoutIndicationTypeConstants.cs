using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class GloveWithoutIndicationTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string CareWithoutBodyFluids = "CARE_WITHOUT_BODY_FLUIDS";

        [TsProperty(Constant = true)]
        public static string Food = "FOOD";

        [TsProperty(Constant = true)]
        public static string Other = "OTHER";
    }
}
