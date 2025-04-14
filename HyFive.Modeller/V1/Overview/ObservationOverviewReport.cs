using System;
using System.Collections.Generic;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Observation.Gloves;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Overview
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ObservationOverviewReport
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
        public string Comment { get; set; }
        public DateTime RegisteredTime { get; set; }
        public List<IndicationType> IndicationTypes { get; set; }
        public Activity Activity { get; set; }
        public List<HandJewelryType> HandJewelryTypes { get; set; }
        public string PPEConfigurationTypes { get; set; }
        public List<ProtectiveEquipmentOverviewReport> ProtectiveEquipment { get; set; }
        public GloveObservation GloveObservation { get; set; }
        public ProtectiveEquipmentObservation ProtectiveEquipmentObservation { get; set; }
    }
}
