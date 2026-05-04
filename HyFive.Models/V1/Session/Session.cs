using HyFive.Models.V1.OrganisationUnit;
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
    /// <typeparam name="TObservation">The type of observation contained in the session.</typeparam>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public abstract class Session<TObservation> : ISessionWithObservations
    {
        /// <summary>
        /// Gets or sets the unique identifier for the session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the session was created.
        /// </summary>
        [TsProperty(Type = "Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the facility associated with the session.
        /// </summary>
        public int FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the department associated with the session.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the unit associated with the session.
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Gets or sets the facility organisation unit for the session.
        /// </summary>
        public Models.V1.OrganisationUnit.OrganisationUnit Facility { get; set; }

        /// <summary>
        /// Gets or sets the department organisation unit for the session.
        /// </summary>
        public Models.V1.OrganisationUnit.OrganisationUnit Department { get; set; }

        /// <summary>
        /// Gets or sets the unit organisation unit for the session.
        /// </summary>
        public Models.V1.OrganisationUnit.OrganisationUnit Unit { get; set; }

        /// <summary>
        /// Gets or sets the collection of observations captured in the session.
        /// May be null.
        /// </summary>
        [TsProperty(ForceNullable = true)]
        public List<TObservation> Observations { get; set; }

        /// <summary>
        /// Gets or sets an optional comment for the session.
        /// May be null.
        /// </summary>
        [TsProperty(ForceNullable = true)]
        public string Comment { get; set; }

        // Explicit interface implementation for ISessionWithObservations.Observations.
        // This is intentionally an explicit implementation and thus not part of the public API surface requiring XML docs.
        [TsIgnore]
        IEnumerable<object> ISessionWithObservations.Observations =>
                    Observations?.Cast<object>() ?? Enumerable.Empty<object>();
    }
}
