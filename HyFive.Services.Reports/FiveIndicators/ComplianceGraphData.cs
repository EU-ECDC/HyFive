using System.Collections.Generic;

namespace HyFive.Services.Reports.FiveIndicators
{
    public class ComplianceGraphData
    {
        public string Name { get; set; }
        public List<CompliancePoint> Data { get; set; }
    }

    public class CompliancePoint
    {
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public decimal Quantity { get; set; }
    }
}
