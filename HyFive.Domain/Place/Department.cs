using System.Collections.Generic;
using HyFive.Domain.Observation;
using HyFive.Domain.Session;

namespace HyFive.Domain.Place
{
    public class Department
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public Facility Facility { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<Session.Session> Sessions { get; set; }
        public ICollection<Unit> Units { get; set; }
        public DepartmentType DepartmentType { get; set; }
    }
}
