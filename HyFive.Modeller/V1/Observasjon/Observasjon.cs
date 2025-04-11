using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Observasjon
    {
        public string Id { get; set; }
        public Role Rolle { get; set; }

        [TsProperty(Type = "Date", ForceNullable = true)]
        public DateTime Registrerttidspunkt { get; set; }
        public string SesjonId { get; set; }

        [TsProperty(ForceNullable = true)]
        public string Kommentar { get; set; }
    }
}