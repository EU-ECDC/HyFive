using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.User
{
    public class CreateUpdateUserRequest
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentityPseudonym { get; set; }
        public bool IsDeactivated { get; set; }
    }

}
