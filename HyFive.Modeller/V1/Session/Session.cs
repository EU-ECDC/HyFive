using System;
using System.Collections.Generic;
using HyFive.Models.V1.Institution;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{

    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public abstract class Session<TObservasjon>
    {
        public string Id { get; set; }
        public Department Avdeling { get; set; }
        [TsProperty(Type = "Date")]
        public DateTime Starttidspunkt { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<TObservasjon> Observasjoner { get; set; }

        [TsProperty(ForceNullable = true)]
        public string Institusjonsnavn { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public int InstitusjonId { get; set; }

        [TsProperty(ForceNullable = true)]
        public string Kommentar { get; set; }
    }
}
