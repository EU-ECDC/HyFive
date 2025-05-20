using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class UpdateSessionRequest
    {
        public Guid SessionId { get; set; }
        public int InstitutionId { get; set; }
        public string Comment { get; set; }
        public DateTime StartTime { get; set; }
    }
}