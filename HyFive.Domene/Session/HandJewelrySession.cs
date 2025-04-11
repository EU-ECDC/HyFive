
using System.Collections.Generic;
using HyFive.Domain.Observation;

namespace HyFive.Domain.Session
{
    public class HandJewelrySession : Session
    {
        public ICollection<HandJewelryObservation> Observations { get; set; }
    }
}
