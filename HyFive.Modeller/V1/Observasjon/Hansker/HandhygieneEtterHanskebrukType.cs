using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Hansker
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HandhygieneEtterHanskebrukType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
    }
}