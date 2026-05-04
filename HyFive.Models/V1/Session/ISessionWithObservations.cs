using HyFive.Models.V1.OrganisationUnit;
using System;
using System.Collections.Generic;

namespace HyFive.Models.V1.Session
{
    /// <summary>
    /// Represents a session that contains a collection of observations. 
    /// Used by shared controller helpers for saving sessions.
    /// </summary>
    public interface ISessionWithObservations
    {
        /// <summary>
        /// Gets the unique identifier for this session.
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// Gets the department associated with this session.
        /// </summary>
        int DepartmentId { get; }
        /// <summary>
        /// Gets the Facility associated with this session.
        /// </summary>
        int FacilityId { get; }
        /// <summary>
        /// Gets the unit associated with this session.
        /// </summary>
        int UnitId { get; }
        /// <summary>
        /// Gets the observations recorded in this session.
        /// Returned as a non-generic collection for domain-agnostic processing.
        /// </summary>
        IEnumerable<object> Observations { get; }
    }
}
