using System;
using HyFive.Models.V1.Session;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.Sesjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SesjonRapport
    {
        public string Id { get; set; }
        public string Avdelingsnavn { get; set; }
        public DateTime Starttidspunkt { get; set; }
        public SesjonType Type { get; set; }
        public string Institusjonsnavn { get; set; }
        [TsProperty(ForceNullable = true)]
        public bool? ErValgt { get; set; }
    }
}
