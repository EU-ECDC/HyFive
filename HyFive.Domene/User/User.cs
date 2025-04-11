using System;
using HyFive.Domain.Place;

namespace HyFive.Domain.User
{
    public class User
    {
        public User()
        {
            IsDisabled = false;
            CreatedTime = DateTime.Now;
        }
        public int Id { get; set; }
        public Institution Institution { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string IdentPseudonym { get; set; }
        public bool IsDisabled { get; set; }
        public string Discriminator { get; set; }
        public string HPRNummer { get; set; }
    }
}
