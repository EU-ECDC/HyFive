using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    /// <summary>
    /// Interface brukt for å opprette institusjoner fra Admin-grensesnittet
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateInstitutionRequest
    {
        public string InstitutionName { get; set; }
        public string Abbreviation { get; set; }
        public string HERId { get; set; }
        public int InstitutionTypeId { get; set; }
        public string CoordinatorHPRNumber { get; set; }
        public string CoordinatorPseudonym { get; set; }
        public string CoordinatorFirstName { get; set; }
        public string CoordinatorLastName { get; set; }
        public string CoordinatorEmail { get; set; }    
        public int RegionId { get; set; }
        public int MunicipalityId { get; set; }
        public int InstitutionId { get; set; }
    }
}
