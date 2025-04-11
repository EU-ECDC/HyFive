using HyFive.Modeller.Sesjon;
using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class InstitutionOverviewReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfSessions { get; set; } = 0;
        public int NumberOfObservations { get; set; } = 0;
        public List<DepartmentOverviewReport> Departments { get; set; }
    }
}
