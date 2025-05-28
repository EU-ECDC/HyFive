using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class Clinic
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public List<Department> Departments { get; set; }
    }
}
