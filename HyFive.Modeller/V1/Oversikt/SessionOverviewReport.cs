using System;
using System.Collections.Generic;
using HyFive.Modeller.V1.Institution;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SessionOverviewReport
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string ObserverName { get; set; }
        public Department Department { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Comment { get; set; }
        public TransferStatusType TransferStatus { get; set; }
        public List<ObservationOverviewReport> Observations { get; set; }
        public bool IsSelected { get; set; }
    }
}
