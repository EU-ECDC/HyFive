using HyFive.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Place
{
    public class Address : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the city associated with this address.
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// Gets or sets the city associated with this address.
        /// </summary>
        public City City { get; set; }
        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Gets or sets the postal code for the address.
        /// </summary>
        public string PostalCode { get; set; }
    }
}
