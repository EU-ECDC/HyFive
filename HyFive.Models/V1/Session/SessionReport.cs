using System;
using HyFive.Models.V1.Session;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.Session
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SessionReport
    {
        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public DateTime StartDate { get; set; }
        public SessionType Type { get; set; }
        public string FacilityName { get; set; }
        [TsProperty(ForceNullable = true)]
        public bool? IsSelected { get; set; }
    }
}
