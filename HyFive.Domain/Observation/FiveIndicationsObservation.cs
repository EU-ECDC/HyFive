using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HyFive.Domain.Session;

namespace HyFive.Domain.Observation
{
    public class FiveIndicationsObservation : Observation
    {
        public FiveIndicationsSession FiveIndicationsSession { get; set; }
        public Activity Activity { get; set; }
        [Column("Indication")]
        public ICollection<IndicationTypes> IndicationTypes { get; set; }
    }
}
