using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public class HanskeMedIndikasjonTypeKonstanter
    {
        [TsProperty(Constant = true)] 
        public static string Kroppvaesker = "KROPPVAESKER";
        [TsProperty(Constant = true)] 
        public static string Smitte = "SMITTE";
        [TsProperty(Constant = true)] 
        public static string Annet = "Annet";
    }
}
