using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    /// <summary>
    /// Request model used to update an organisation unit's address.
    /// </summary>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class UpdateAddressRequest
    {
        /// <summary>
        /// Identifier of the city associated with the address.
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Street name and number of the address.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Postal code for the address.
        /// </summary>
        public string PostalCode { get; set; }

    }
}
