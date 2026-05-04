using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Authentication
{
    public class OrganisationAccessInfo
    {
        public int OrganisationUnitId { get; set; }
        public string PermissionLevel { get; set; } = null!;  // ADMIN / COORDINATOR / OBSERVER
        public int LevelId { get; set; }
        public string Level { get; set; } = null!;            // Unit / Department / Unit
        public string Name { get; set; } = null!;
    }
}
