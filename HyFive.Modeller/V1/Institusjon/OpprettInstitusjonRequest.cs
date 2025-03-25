using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OpprettInstitusjonRequest
    {
        public string Institusjonsnavn { get; set; }
        public string Forkortelse { get; set; }
        public string HERId { get; set; }
        public int InstitusjonTypeId { get; set; }
        public string KoordinatorHPRnummer { get; set; }
        public string KoordinatorPseudonym { get; set; }
        public string KoordinatorFornavn { get; set; }
        public string KoordinatorEtternavn { get; set; }
        public string KoordinatorEpost { get; set; }    
        public int RegionId { get; set; }
        public int KommuneId { get; set; }
        public int HelseforetakId { get; set; }
    }
}
