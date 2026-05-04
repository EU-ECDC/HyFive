using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreateAddressRequest
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}
