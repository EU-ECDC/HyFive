using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Rolle
    {
        public Rolle() { }
        public Rolle(string navn, int id)
        {
            Navn = navn;
            Id = id;
        }

        public int Id { get; set; }
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
    }
}
