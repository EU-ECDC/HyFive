using HyFive.Models.Sesjon;
using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Models.V1.Overview
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
