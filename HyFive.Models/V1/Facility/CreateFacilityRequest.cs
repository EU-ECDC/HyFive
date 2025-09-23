using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    /// <summary>
    /// Interface used to create facilities from the admin interface
    /// </summary>
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateFacilityRequest
    {
        public string FacilityName { get; set; }
        public string Abbreviation { get; set; }
        public string HERId { get; set; }
        public int FacilityTypeId { get; set; }
        public string CoordinatorHPRNumber { get; set; }
        public string CoordinatorPseudonym { get; set; }
        public string CoordinatorFirstName { get; set; }
        public string CoordinatorLastName { get; set; }
        public string CoordinatorEmail { get; set; }    
        public int CityId { get; set; }
    }
}
