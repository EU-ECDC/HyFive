using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Bruker
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class Bruker
    {
        public int Id { get; set; }
        public int InstitusjonId { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Epost { get; set; }
        public string IdentPseudonym { get; set; }
        public bool ErDeaktivert { get; set; }
        public string HPRNummer { get; set; }
       
    }
}
