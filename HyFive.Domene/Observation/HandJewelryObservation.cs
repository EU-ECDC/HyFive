using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HyFive.Domain.Session;

namespace HyFive.Domain.Observation
{
    public class HandJewelryObservation : Observation
    {
        public ICollection<HandJewelryType> HandJewelries { get; set; }
        public HandJewelrySession HandJewelrySession { get; set; }
    }
}
