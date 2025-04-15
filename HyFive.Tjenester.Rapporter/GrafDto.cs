using System.Collections.Generic;

namespace HyFive.Services.Reports
{
    public class GrafDto
    {
        public string Title { get; set; }
        public List<GrafDataDto> GraphDataList { get; set; }
    }
    public class GrafDataDto
    {
        public string Name { get; set; }
        public List<PointDto> Data { get; set; }
    }
    public class PointDto
    {
        public string Name { get; set; }
        public decimal Y { get; set; }
    }
}
