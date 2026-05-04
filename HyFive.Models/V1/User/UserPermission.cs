using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class UserPermission
    {
        public int Id { get; set; }

        public string PermissionLevel { get; set; }

        public int UserId { get; set; }

        public int OrganisationUnitId { get; set; }

        // Optional: only if you want to return details in the same response
        [TsProperty(ForceNullable = true)]
        public Models.V1.OrganisationUnit.OrganisationUnit OrganisationUnit { get; set; }
    }
}
