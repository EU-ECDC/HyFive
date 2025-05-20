
using System.Collections.Generic;
using HyFive.Domain.Observation;

namespace HyFive.Domain.Session
{
    public class FourIndicationsSession : Session
    {
        public ICollection<FourIndicationsObservation> Observations { get; set; }
    }
}
