using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Services.Authentication.Configuration
{
    public class ClaimTypes
    {
        public string RoleClaimType { get; set; }
        public string EnvironmentAccessClaimType { get; set; }
        public string DomainClaimType { get; set; }
        public string DisplayNameClaimType { get; set; }
        public string UniqueIdentifierClaimType { get; set; }
    }
}
