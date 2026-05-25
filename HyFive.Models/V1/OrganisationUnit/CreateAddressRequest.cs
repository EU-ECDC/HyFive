using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.OrganisationUnit
{
    /// <summary>
    /// Request model used to create an address for an organisation unit.
    /// </summary>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreateAddressRequest
    {
            /// <summary>
            /// Identifier of the city the address belongs to.
            /// </summary>
            public int CityId { get; set; }

            /// <summary>
            /// Street name and number of the address.
            /// </summary>
        public string Street { get; set; }

            /// <summary>
            /// Postal or ZIP code for the address.
            /// </summary>
        public string PostalCode { get; set; }
    }
}
