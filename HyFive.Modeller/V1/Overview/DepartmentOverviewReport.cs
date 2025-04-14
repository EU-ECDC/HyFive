using System.Collections.Generic;
using HyFive.Models.V1.Observation;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Overview
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class DepartmentOverviewReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfSessions { get; set; }
        public int NumberOfObservations { get; set; }
    }
}
