
using System.Collections.Generic;
using HyFive.Domain.Observation;

namespace HyFive.Domain.Session
{
    public class FiveIndicationsSession : Session
    {
        public ICollection<FiveIndicationsObservation> Observations { get; set; }
    }
}
