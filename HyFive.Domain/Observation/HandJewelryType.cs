using System.Collections.Generic;

namespace HyFive.Domain.Observation
{
    public class HandJewelryType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }

        public ICollection<HandJewelryObservation> Observations { get; set; }
    }
}