using System.Collections.Generic;

namespace HyFive.Domene.Observasjon
{
    public class HandsmykkeType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public int Rekkefolge { get; set; }
        public bool ErAktiv { get; set; }

        public ICollection<HandsmykkeObservasjon> Observasjoner { get; set; }
    }
}