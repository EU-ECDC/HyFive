using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Autentisering
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class InnloggetBruker
    {
        public string Id { get; set; }
        public string IdentPseudonym { get; set; }
        public string Navn { get; set; }
        public bool ErKoordinator { get; set; }
        public bool ErFhiAdmin { get; set; }
        public bool ErObservator { get; set; }
        public List<int> InstitusjonsIder { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string HPRNummer { get; set; }
    }
}