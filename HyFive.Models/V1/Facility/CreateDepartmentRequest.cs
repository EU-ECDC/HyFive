using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    /// <summary>
    /// Interface used to create facilities from the Admin interface
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateDepartmentRequest
    {
        public string Name { get; set; }
        public int FacilityId { get; set; }
        public int DepartmentTypeId { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
