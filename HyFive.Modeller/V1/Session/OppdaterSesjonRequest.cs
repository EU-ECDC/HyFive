using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class OppdaterSesjonRequest
    {
        public Guid SesjonId { get; set; }
        public int InstitutionId { get; set; }
        public string Kommentar { get; set; }
        public DateTime Starttidspunkt { get; set; }
    }
}