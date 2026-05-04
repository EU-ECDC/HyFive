using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Report.FiveIndications
{
    /// <summary>
    /// Request model for generating Five Indications compliance reports.
    /// Contains filtering parameters such as facilities, intervals, roles, departments and units.
    /// </summary>
    public class FiveIndicationsComplianceRequest
    {
        public List<int> FacilityIds { get; set; }
        public List<int> FacilityTypeIds { get; set; }
        public string Interval { get; set; }
        public int FromMonth { get; set; }
        public int FromYear { get; set; }
        public int FromQuarter { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }
        public int ToQuarter { get; set; }
        public List<int> RoleIds { get; set; }
        public List<int> DepartmentIds { get; set; }
        public List<int> DepartmentTypeIds { get; set; }

        /// <summary>
        /// Gets or sets the list of unit identifiers used to filter the report.
        /// Each integer represents a Unit ID in the system.
        /// </summary>
        public List<int> UnitIds { get; set; }

        public int TransferredTo { get; set; }
        /// <summary>
        /// Gets or sets the single role identifier used to filter the report.
        /// Use <see cref="RoleIds"/> when filtering by multiple roles.
        /// </summary>
        public int Role { get; set; }
    }
}
