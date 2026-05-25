using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    /// <summary>
    /// Represents a city within an organisation unit.
    /// </summary>
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class City
    {
        /// <summary>
        /// Gets or sets the unique identifier of the city.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        public string Name { get; set; }
    }
}
