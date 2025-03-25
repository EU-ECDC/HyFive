using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class BeskyttelsesutstyrType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public bool ErDefaultIndikert { get; set; }
        public bool ErIndikert { get; set; }
        public List<FeilbrukType> Feilbruktyper { get; set; }
    }
}