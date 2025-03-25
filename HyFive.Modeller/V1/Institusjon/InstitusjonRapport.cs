using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class InstitusjonRapport
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public string Forkortelse { get; set; }
        public string HERId { get; set; }
        public InstitusjonType Institusjontype { get; set; }
        public Region Region { get; set; }
        public Kommune Kommune { get; set; }
        public Helseforetak Helseforetak { get; set; }
    }
}
