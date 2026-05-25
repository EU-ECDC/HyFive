using HyFive.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Place
{
    /// <summary>
    /// Represents a city entity with auditing information, including its unique identifier, name, and associated addresses.
    /// </summary>
    public class City : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the city.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of addresses associated with the city.
        /// </summary>
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
