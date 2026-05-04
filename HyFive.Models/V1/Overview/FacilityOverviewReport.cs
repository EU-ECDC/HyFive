using HyFive.Models.Session;
using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Models.V1.Overview
{
    /// <summary>
    /// Represents an overview report for a facility, including aggregated counts and units.
    /// </summary>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class FacilityOverviewReport
    {
        /// <summary>
        /// Unique identifier of the facility.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The display name of the facility.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number of sessions associated with the facility.
        /// </summary>
        public int NumberOfSessions { get; set; } = 0;

        /// <summary>
        /// Number of observations associated with the facility.
        /// </summary>
        public int NumberOfObservations { get; set; } = 0;

        /// <summary>
        /// List of unit overview reports belonging to this facility.
        /// Each entry represents aggregated data for a single unit.
        /// </summary>
        public List<UnitOverviewReport> Units { get; set; } = new();
    }
}
