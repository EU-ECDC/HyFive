using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    /// <summary>
    /// Represents a postal address associated with an organisation unit.
    /// </summary>
    /// <remarks>
    /// An <see cref="Address"/> references a <see cref="City"/> via <see cref="CityId"/> and contains street and postal code information.
    /// </remarks>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Address
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="City"/> associated with this address.
        /// </summary>
        /// <remarks>
        /// This value is the foreign key linking the address to a <see cref="City"/> entity.
        /// </remarks>
        public int CityId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="City"/> navigation property for this address.
        /// </summary>
        /// <remarks>
        /// This may be null when the related city is not loaded or not set.
        /// </remarks>
        /// <seealso cref="City"/>
        public City City { get; set; }

        /// <summary>
        /// Gets or sets the street portion of the address (street name and number).
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the postal code (ZIP code) for the address.
        /// </summary>
        public string PostalCode { get; set; }
    }
}
