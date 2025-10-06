using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Report.ProtectiveEquipment
{
    public class ProtectiveEquipmentReport
    {
        public List<int> DepartmentIds { get; set; }
        public List<int> FacilityIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Role { get; set; }
    }
}
