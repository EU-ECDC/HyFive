using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Observation
    {
        public string Id { get; set; }
        public Role Role { get; set; }

        [TsProperty(Type = "Date", ForceNullable = true)]
        public DateTime RegistrationTime { get; set; }
        public string SessionId { get; set; }

        [TsProperty(ForceNullable = true)]
        public string Comment { get; set; }
    }
}