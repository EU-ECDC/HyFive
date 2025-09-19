using System;
using HyFive.Domain.Place;

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
        public Facility Facility { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentityPseudonym { get; set; }
        public bool IsDeactivated { get; set; }
        public string Discriminator { get; set; }
        public string HPRNumber { get; set; }
    }
}
