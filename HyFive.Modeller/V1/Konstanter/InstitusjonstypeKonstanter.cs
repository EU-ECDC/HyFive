using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public static class InstitusjonstypeKonstanter
    {
        [TsProperty(Constant = true)]
        public static string Sykehus = "SYKEHUS";
        [TsProperty(Constant = true)]
        public static string Sykehjem = "SYKEHJEM";
    }
}