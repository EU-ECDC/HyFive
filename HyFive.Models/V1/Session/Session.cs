using System;
using System.Collections.Generic;
using HyFive.Models.V1.Institution;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{

    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public abstract class Session<TObservation>
    {
        public string Id { get; set; }
        public Department Department { get; set; }
        [TsProperty(Type = "Date")]
        public DateTime CreatedDate { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<TObservation> Observations { get; set; }

        [TsProperty(ForceNullable = true)]
        public string InstitutionsName { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public int InstitutionId { get; set; }

        [TsProperty(ForceNullable = true)]
        public string Comment { get; set; }
    }
}
