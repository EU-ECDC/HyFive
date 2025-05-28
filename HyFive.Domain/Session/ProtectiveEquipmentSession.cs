using System.Collections.Generic;
using HyFive.Domain.Observation.ProtectiveEquipment;

namespace HyFive.Domain.Session
{
    public class ProtectiveEquipmentSession : Session
    {
        public ICollection<ProtectiveEquipmentObservation> Observations { get; set; }
    }
}
