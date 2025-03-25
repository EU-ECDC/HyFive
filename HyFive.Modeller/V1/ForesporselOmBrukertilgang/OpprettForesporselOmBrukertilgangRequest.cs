using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.ForesporselOmBrukertilgang
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OpprettForesporselOmBrukertilgangRequest
    {
        [TsProperty(ForceNullable = true)]
        public int InstitusjonId { get; set; }
        public string BrukerFornavn { get; set; }
        public string BrukerEtternavn { get; set; }
        public string IdentPseudonym { get; set; }
        public string HPRNummer { get; set; }
    }
}
