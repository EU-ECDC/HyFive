using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class Unit
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public List<Department> Departments { get; set; }
    }
}
