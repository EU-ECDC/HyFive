using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Services.Authentication.Configuration
{
    public class OpenIdConnectSettings
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Authority { get; set; }

        public bool AuthUse { get; set; }
    }
}
