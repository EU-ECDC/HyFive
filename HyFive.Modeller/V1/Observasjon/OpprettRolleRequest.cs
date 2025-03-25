using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    /// <summary>
    /// Interface brukt for å opprette roller fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OpprettRolleRequest
    {
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
    }
}
