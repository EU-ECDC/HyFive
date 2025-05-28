using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyFive.Domain.Observation
{
    [Table("IndicationType")]
    public class IndicationTypes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public ICollection<FourIndicationsObservation> Observations { get; set; }
    }
}
