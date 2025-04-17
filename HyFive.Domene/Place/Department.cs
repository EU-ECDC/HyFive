using System.Collections.Generic;
using HyFive.Domain.Observation;
using HyFive.Domain.Session;

namespace HyFive.Domain.Place
{
    public class Department
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public Institution Institution { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<Session.Session> Sessions { get; set; }
        public ICollection<Clinic> Clinics { get; set; }
        public DepartmentType DepartmentType { get; set; }
    }
}
