using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Services.Authentication.Configuration
{
    public class RedirectPagesSettings
    {
        public string RedirectUri { get; set; }
        public string LoggedOut { get; set; }
        public string LoggedIn { get; set; }
    }
}
