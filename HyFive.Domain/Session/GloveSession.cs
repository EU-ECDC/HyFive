using System.Collections.Generic;
using HyFive.Domain.Observation.Gloves;

namespace HyFive.Domain.Session
{
    public class GloveSession : Session
    {
        public ICollection<GloveObservation> Observations { get; set; }
    }
}