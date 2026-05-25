using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    /// <summary>
    /// Request model for creating a new organisation unit (v1).
    /// </summary>
    public class CreateOrganisationUnitRequest
    {
        /// <summary>
        /// Gets or sets the display name of the organisation unit.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short abbreviation or code for the organisation unit.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets an optional description of the organisation unit.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the organisation unit type.
        /// </summary>
        public int OrganisationUnitTypeId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the city where the organisation unit is located.
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Gets or sets the street address of the organisation unit.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the postal code for the organisation unit's address.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the first name of the primary contact for the organisation unit.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the primary contact for the organisation unit.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the primary contact for the organisation unit.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a pseudonym or display name for the primary contact (optional).
        /// </summary>
        public string Pseudonym { get; set; }
    }
}
