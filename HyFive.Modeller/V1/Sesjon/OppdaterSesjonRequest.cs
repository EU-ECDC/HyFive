using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Sesjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class OppdaterSesjonRequest
    {
        public Guid SesjonId { get; set; }
        public int InstitusjonId { get; set; }
        public string Kommentar { get; set; }
        public DateTime Starttidspunkt { get; set; }
    }
}