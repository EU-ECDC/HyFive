using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Session
{
    public class ReportForSessionTypeHasDataRequest
    {
        public int SessionType { get; set; }
        public List<int> InstitutionIds { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int RoleId { get; set; }
    }
}
