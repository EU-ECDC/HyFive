using System.Collections.Generic;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;

namespace HyFive.Domene.Sesjon
{
    public class BeskyttelsesutstyrSesjon : Sesjon
    {
        public ICollection<BeskyttelsesutstyrObservasjon> Observasjoner { get; set; }
    }
}
