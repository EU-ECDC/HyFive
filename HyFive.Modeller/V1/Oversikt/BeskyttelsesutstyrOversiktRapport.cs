using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class BeskyttelsesutstyrOversiktRapport
    {
        public string Utstyrsnavn { get; set; }
        public List<string> Feilbruktyper { get; set; }
        public bool BleBenyttet { get; set; }
        public bool BleBenyttetRiktig { get; set; }
        public bool ErIndikert { get; set; }
        public string Kommentar { get; set; }
    }
}
