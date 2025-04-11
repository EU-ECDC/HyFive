using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Role
    {
        public Role() { }
        public Role(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
