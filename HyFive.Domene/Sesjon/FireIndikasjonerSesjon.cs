
using System.Collections.Generic;
using HyFive.Domene.Observasjon;

namespace HyFive.Domene.Sesjon
{
    public class FireIndikasjonerSesjon : Sesjon
    {
        public ICollection<FireIndikasjonerObservasjon> Observasjoner { get; set; }
    }
}
