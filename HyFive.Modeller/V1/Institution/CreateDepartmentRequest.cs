using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institution
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateDepartmentRequest
    {
        public string Name { get; set; }
        public int InstitutionId { get; set; }
        public int DepartmentTypeId { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
