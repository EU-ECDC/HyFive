using System.Collections.Generic;
using HyFive.Domene.Sesjon;

namespace HyFive.Domene.Observasjon.Beskyttelsesutstyr
{
    public class BeskyttelsesutstyrObservasjon : Observasjon
    {
        public List<Beskyttelsesutstyr> Beskyttelsesutstyrliste { get; set; }
        public BeskyttelsesutstyrsettingType Settingtype { get; set; }
        public BeskyttelsesutstyrSesjon BeskyttelsesutstyrSesjon { get; set; }
    }
}