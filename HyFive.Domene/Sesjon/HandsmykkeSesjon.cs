
using System.Collections.Generic;
using HyFive.Domene.Observasjon;

namespace HyFive.Domene.Sesjon
{
    public class HandsmykkeSesjon : Sesjon
    {
        public ICollection<HandsmykkeObservasjon> Observasjoner { get; set; }
    }
}
