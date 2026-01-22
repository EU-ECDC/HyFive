using HyFive.Models.V1.Facility;
using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyFive.Models.V1.Session
{
    /// <summary>
    /// Represents the base class for all types of observation sessions.
    /// Contains shared properties such as department, observations, and metadata.
    /// </summary>
    /// <typeparam name="TObservation">
    /// The type of observation contained in the session.
    /// </typeparam>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public abstract class Session<TObservation> : ISessionWithObservations
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

        // Explicit interface implementation
        [TsIgnore]
        IEnumerable<object> ISessionWithObservations.Observations =>
                    Observations?.Cast<object>() ?? Enumerable.Empty<object>();
    }
}
