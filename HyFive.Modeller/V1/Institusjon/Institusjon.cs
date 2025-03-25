using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Institusjon
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public string Forkortelse { get; set; }
        public string HERId { get; set; }
        public List<Avdeling> Avdelinger { get; set; }
        public InstitusjonType Institusjontype { get; set; }
        public Region Region { get; set; }
        public Kommune Kommune { get; set; }
        public bool HarObservasjoner { get; set; }
        public Helseforetak Helseforetak { get; set; }
    }
}
