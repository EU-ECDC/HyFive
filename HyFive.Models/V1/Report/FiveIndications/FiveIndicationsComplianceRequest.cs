using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Report.FiveIndications
{
    public class FiveIndicationsComplianceRequest
    {
        public List<int> InstitutionIds { get; set; }
        public List<int> InstitutionTypeIds { get; set; }
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
    }
}
