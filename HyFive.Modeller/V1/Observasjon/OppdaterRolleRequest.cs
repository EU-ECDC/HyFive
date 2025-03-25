using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    /// <summary>
    /// Interface brukt for å oppdatere roller fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OppdaterRolleRequest
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
    }
}
