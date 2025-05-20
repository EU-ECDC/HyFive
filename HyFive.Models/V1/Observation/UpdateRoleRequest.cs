using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    /// <summary>
    /// Interface used to update roles from the Admin interface
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class UpdateRoleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
