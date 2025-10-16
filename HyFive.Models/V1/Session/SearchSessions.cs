using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.Session
{
    public class SearchSessions
    {
        public int Take { get; set; } = 25;
        public int Skip { get; set; }
    }
}
