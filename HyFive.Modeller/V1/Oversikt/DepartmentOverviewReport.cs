using System.Collections.Generic;
using HyFive.Modeller.V1.Observasjon;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Oversikt
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
