using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Overview
{
    /// <summary>
    /// Represents an overview report for a unit containing counts and department information.
    /// </summary>
    public class UnitOverviewReport
    {
        /// <summary>
        /// Gets or sets the unique identifier of the unit.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the unit.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of sessions associated with the unit.
        /// </summary>
        public int NumberOfSessions { get; set; }

        /// <summary>
        /// Gets or sets the number of observations recorded for the unit.
        /// </summary>
        public int NumberOfObservations { get; set; }

        /// <summary>
        /// Gets or sets the name of the department the unit belongs to.
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the department the unit belongs to.
        /// </summary>
        public int DepartmentId { get; set; }
    }
}
