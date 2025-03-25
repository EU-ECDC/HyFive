using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OpprettInstitusjonstypeRequest
    {
        public string Kode { get; set; }
        public string Navn { get; set; }
    }
}
