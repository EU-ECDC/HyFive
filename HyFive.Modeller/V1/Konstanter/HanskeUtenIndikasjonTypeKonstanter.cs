using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public class HanskeUtenIndikasjonTypeKonstanter
    {
        [TsProperty(Constant = true)] 
        public static string StellUtenKroppvaesker = "STELL_UTEN_KROPPVAESKER";
        [TsProperty(Constant = true)] 
        public static string Mat = "MAT";
        [TsProperty(Constant = true)] 
        public static string Annet = "Annet";
    }
}
