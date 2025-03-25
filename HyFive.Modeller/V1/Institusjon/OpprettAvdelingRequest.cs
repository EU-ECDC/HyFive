using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OpprettAvdelingRequest
    {
        public string Navn { get; set; }
        public int InstitusjonId { get; set; }
        public int AvdelingTypeId { get; set; }
        public List<int> RolleIder { get; set; }
    }
}
