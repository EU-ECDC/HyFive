using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Hansker
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HanskeMedIndikasjonType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }

        public bool ErValgt { get; set; }
    }
}