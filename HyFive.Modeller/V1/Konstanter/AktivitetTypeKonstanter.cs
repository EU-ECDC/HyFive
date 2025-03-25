using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public static class AktivitetTypeKonstanter
    {
        [TsProperty(Constant = true)]
        public static string Desinfeksjon = "DESINFEKSJON";
        [TsProperty(Constant = true)]
        public static string Handvask = "HANDVASK";
        [TsProperty(Constant = true)]
        public static string IkkeUtfort = "IKKE_UTFORT";
        [TsProperty(Constant = true)]
        public static string IkkeRegistrert = "IKKE_REGISTRERT";
    }
}