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
        public int Id { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}
