using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.ForesporselOmBrukertilgang
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class ForesporselOmBrukertilgang
    {
        public int Id { get; set; }
        [TsProperty(ForceNullable = true)]
        public int InstitusjonId { get; set; }
        public string BrukerFornavn { get; set; }
        public string BrukerEtternavn { get; set; }
        public string IdentPseudonym { get; set; }
        public string HPRNummer { get; set; }
        public int Status { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public DateTime Godkjenttidspunkt { get; set; }
    }
}
