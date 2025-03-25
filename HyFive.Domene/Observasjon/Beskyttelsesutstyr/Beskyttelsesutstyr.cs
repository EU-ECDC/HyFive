using System;
using System.Collections.Generic;

namespace HyFive.Domene.Observasjon.Beskyttelsesutstyr
{
    public class Beskyttelsesutstyr
    {
        public int Id { get; set; }
        public bool BleBenyttet { get; set; }
        public bool ErIndikert { get; set; }
        public BeskyttelsesutstyrType Utstyrstype { get; set; }
        public ICollection<FeilbrukType> Feilbruktyper { get; set; }
        public bool BleBenyttetRiktig { get; set; }
        public string Kommentar { get; set; }
        public BeskyttelsesutstyrObservasjon BeskyttelsesutstyrObservasjon { get; set; }
    }
}