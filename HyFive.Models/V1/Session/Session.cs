using System;
using System.Collections.Generic;
using HyFive.Models.V1.Facility;
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
        public string FacilityName { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public int FacilityId { get; set; }

        [TsProperty(ForceNullable = true)]
        public string Comment { get; set; }
    }
}
