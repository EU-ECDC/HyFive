using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyFive.Domain.Place
{
    public class Institution
    {
        public Institution()
        {
            CreatedTime = DateTime.UtcNow;
            Users = new List<User.User>();
            PredefinedComments = new List<PredefinedComments>();
        }
        public int Id { get; set; }
        public string HERId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public ICollection<Clinic> Clinic { get; set; }
        public ICollection<Department> Departments { get; set; }
        public string Abbreviation { get; set; }
        [Column("User")]
        public ICollection<User.User> Users { get; set; }
        public ICollection<PredefinedComments> PredefinedComments { get; set; }
        public InstitutionType InstitutionType { get; set; }
        public Region Region { get; set; }
        public HealthcareOrganization HealthcareOrganization { get; set; }
        public Municipality Municipality { get; set; }
    }
}
