using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Domain.Place
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Facility Facility { get; set; }
        public ICollection<Department> Departments { get; set; }
    }
}
