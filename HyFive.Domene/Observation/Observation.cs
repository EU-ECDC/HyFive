using System;

namespace HyFive.Domain.Observation
{
    public abstract class Observation
    {
        public Guid Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public Role Role { get; set; }
        public DateTime RegistrationTime { get; set; }
        public string Comment { get; set; }
    }
}