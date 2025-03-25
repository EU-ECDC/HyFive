using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Beskyttelsesutstyr
    {
        public int Id { get; set; }
        public bool BleBenyttet { get; set; }
        public bool ErIndikert { get; set; }
        public BeskyttelsesutstyrType Utstyrstype { get; set; }
        public List<FeilbrukType> Feilbruktyper { get; set; }
        public bool BleBenyttetRiktig { get; set; }
        public string Kommentar { get; set; }
    }
}