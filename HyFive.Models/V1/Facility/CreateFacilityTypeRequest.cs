using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateFacilityTypeRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
