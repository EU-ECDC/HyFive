using System;
using System.Collections.Generic;
using HyFive.Models.V1.Institution;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Overview
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SessionOverviewReport
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string ObserverName { get; set; }
        public Department Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comment { get; set; }
        public TransferStatusType TransferStatus { get; set; }
        public List<ObservationOverviewReport> Observations { get; set; }
        public bool IsSelected { get; set; }
    }
}
