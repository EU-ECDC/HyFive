using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class GloveWithIndicationTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string BodyFluids = "BODY_FLUIDS";

        [TsProperty(Constant = true)]
        public static string Transmission = "TRANSMISSION";

        [TsProperty(Constant = true)]
        public static string Other = "OTHER";
    }
}
