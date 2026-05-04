using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class UpdateAddressRequest
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

    }
}
