using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class BeskyttelsesutstyrsettingType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public List<BeskyttelsesutstyrType> Utstyrstyper { get; set; }
    }
}