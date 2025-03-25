using System.Collections.Generic;

namespace HyFive.Domene.Observasjon.Beskyttelsesutstyr
{
    public class BeskyttelsesutstyrType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
      
        public List<FeilbrukType> Feilbruktyper { get; set; }
        public ICollection<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType> BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper { get; set; }
    }
}