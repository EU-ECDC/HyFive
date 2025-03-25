using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Avdeling
    {
        public int Id { get; set; }
        public int InstitusjonId { get; set; }
        public int AvdelingTypeId { get; set; }
        public string Navn { get; set; }
        public List<Modeller.V1.Observasjon.Rolle> Roller { get; set; }
        public AvdelingType AvdelingType { get; set; }
    }
}
