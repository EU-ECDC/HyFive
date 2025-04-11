using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Department
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public int DepartmentTypeId { get; set; }
        public string Name { get; set; }
        public List<Modeller.V1.Observasjon.Role> Role { get; set; }
        public DepartmentType DepartmentType { get; set; }
    }
}
