using HyFive.Modeller.Sesjon;
using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class InstitusjonOversiktRapport
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public int AntallSesjoner { get; set; } = 0;
        public int AntallObservasjoner { get; set; } = 0;
        public List<AvdelingOversiktRapport> Avdelinger { get; set; }
    }
}
