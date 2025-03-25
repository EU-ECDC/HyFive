using System.Collections.Generic;
using HyFive.Domene.Observasjon.Hansker;

namespace HyFive.Domene.Sesjon
{
    public class HanskeSesjon : Sesjon
    {
        public ICollection<HanskeObservasjon> Observasjoner { get; set; }
    }
}