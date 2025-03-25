using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class Klinikk
    {
        public int Id { get; set; }
        public int InstitusjonId { get; set; }
        public string Navn { get; set; }
        public List<Avdeling> Avdelinger { get; set; }
    }
}
