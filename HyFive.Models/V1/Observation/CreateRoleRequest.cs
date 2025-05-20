using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    /// <summary>
    /// Interface brukt for å opprette roller fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
