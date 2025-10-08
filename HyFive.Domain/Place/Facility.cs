using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyFive.Domain.Place
{
    public class Facility
    {
        public Facility()
        {
            CreatedTime = DateTime.UtcNow;
            Users = new List<User.User>();
            PredefinedComment = new List<PredefinedComment>();
        }
        public int Id { get; set; }
        public string HERId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public ICollection<Unit> Units { get; set; }
        public ICollection<Department> Departments { get; set; }
        public string Abbreviation { get; set; }
        [Column("User")]
        public ICollection<User.User> Users { get; set; }
        public ICollection<PredefinedComment> PredefinedComment { get; set; }
        public FacilityType FacilityType { get; set; }
        public City City { get; set; }
    }
}
