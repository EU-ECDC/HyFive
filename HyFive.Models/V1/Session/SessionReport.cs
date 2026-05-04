using System;
using HyFive.Models.V1.Session;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.Session
{
    /// <summary>
    /// Represents a session report containing identifiers and metadata about a session.
    /// </summary>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SessionReport
    {
        /// <summary>
        /// Gets or sets the unique identifier for the session.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the unit associated with the session.
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Gets or sets the name of the unit associated with the session.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the department associated with the session.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the department associated with the session.
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the facility associated with the session, if any.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the name of the facility associated with the session.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the session.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the session.
        /// </summary>
        public SessionType Type { get; set; }

        /// <summary>
        /// Gets or sets the displayed name of the session.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the session is selected.
        /// Nullable to allow explicit "unset" state for the TypeScript mapping.
        /// </summary>
        [TsProperty(ForceNullable = true)]
        public bool? IsSelected { get; set; }
    }
}
