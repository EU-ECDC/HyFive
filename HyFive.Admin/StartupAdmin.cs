using System;
using HyFive.Api.Common;
using Microsoft.Extensions.Configuration;

namespace HyFive.Admin
{
    public class StartupAdmin : BaseApiStartup
    {
        protected override string ApiTittel { get; } = "HyFive.Admin.Api";
        protected override Type ApiType { get; } = typeof(StartupAdmin);
        
        public StartupAdmin(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
