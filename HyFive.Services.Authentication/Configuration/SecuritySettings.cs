using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Services.Authentication.Configuration
{
    public class SecuritySettings
    {
        
        public OpenIdConnectSettings OpenIdConnect { get; set; }

        public ClaimTypes ClaimTypes { get; set; }

    }
}
