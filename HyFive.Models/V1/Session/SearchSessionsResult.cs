using HyFive.Models.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Session
{
    public class SearchSessionsResult
    {
        public List<SessionReport> sessionReports {  get; set; }
        public int totalCount { get; set; }
    }
}
