using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class IndikasjonType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public string Nummer { get; set; }
    }
}
