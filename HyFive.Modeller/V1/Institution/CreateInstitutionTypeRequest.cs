using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institution
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateInstitutionTypeRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
