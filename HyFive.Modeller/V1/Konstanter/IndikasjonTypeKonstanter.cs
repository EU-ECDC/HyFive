using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public static class IndikasjonTypeKonstanter
    {
        [TsProperty(Constant = true)]
        public static string FoerPasient = "FOER_PASIENT";
        [TsProperty(Constant = true)]
        public static string AseptiskeProsedyrer = "ASEPTISKE_PROSEDYRER";
        [TsProperty(Constant = true)]
        public static string Kroppsveske = "KROPPSVESKE";
        [TsProperty(Constant = true)]
        public static string EtterPasient = "ETTER_PASIENT";
    }
}