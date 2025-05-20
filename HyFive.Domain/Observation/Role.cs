
using System.Collections.Generic;
using HyFive.Domain.Place;

namespace HyFive.Domain.Observation
{
    public class Role
    {
        public Role()
        {

        }
        public Role(string name)
        {
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Department> Departments { get; set; }
    }
}
