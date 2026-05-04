using HyFive.Domain.Place;
using System;
using System.Collections.Generic;

namespace HyFive.Domain.User
{
    public class User
    {
        public User()
        {
            IsDeactivated = false;
            CreatedTime = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentityPseudonym { get; set; }
        public bool IsDeactivated { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; }
        public ICollection<UserIdentifier> UserIdentifiers { get; set; }
    }
}
