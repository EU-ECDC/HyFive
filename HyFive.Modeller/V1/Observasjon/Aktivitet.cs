using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Aktivitet
    {
        public AktivitetType AktivitetType { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public int SekunderBrukt { get; set; }

        public bool TidtakingBleUtfort { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public bool? BenyttetHanske { get; set; }
    }
}
