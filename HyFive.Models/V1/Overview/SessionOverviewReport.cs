using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using HyFive.Models.V1.OrganisationUnit;

namespace HyFive.Models.V1.Overview
{
    /// <summary>
    /// Represents a session-level overview report containing metadata and observation summaries.
    /// </summary>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SessionOverviewReport
    {
        /// <summary>
        /// Gets or sets the unique identifier of the session report.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the session.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the displayed name of the session.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the observer who performed the session.
        /// </summary>
        public string ObserverName { get; set; }

        /// <summary>
        /// Gets or sets the name of the organisational unit where the session took place.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the department associated with this session.
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the department associated with this session.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the optional facility identifier associated with this session.
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the session.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the session report.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets any comment associated with the session.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the transfer status of the session report.
        /// </summary>
        public TransferStatusType TransferStatus { get; set; }

        /// <summary>
        /// Gets or sets the list of observation overview reports included in this session.
        /// </summary>
        public List<ObservationOverviewReport> Observations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this session is selected in the UI.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
