using HyFive.Modeller.V1.Institusjon;
using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Bruker
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class KoordinatorForHelseforetak
    {
        public DateTime Opprettettidspunkt { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Epost { get; set; }
        public string IdentPseudonym { get; set; }
        public bool ErDeaktivert { get; set; }
        public string HPRNummer { get; set; }
        public List<InstitusjonRapport> Institusjoner { get; set; }
        [TsProperty(ForceNullable = true)]
        public string EndretIdentPseudonym { get; set; }
        [TsProperty(ForceNullable = true)]
        public string EndretHPRNummer { get; set; }

    }

}
