using HyFive.Models.V1.Facility;
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
        string Id { get; }
        /// <summary>
        /// Gets the department associated with this session.
        /// </summary>
        Department Department { get; }
        /// <summary>
        /// Gets the observations recorded in this session.
        /// Returned as a non-generic collection for domain-agnostic processing.
        /// </summary>
        IEnumerable<object> Observations { get; }
    }
}
