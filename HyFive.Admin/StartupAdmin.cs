using System;
using HyFive.Api.Common;
using HyFive.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HyFive.Admin
{
    public class StartupAdmin : BaseApiStartup
    {
        protected override string ApiTitle { get; } = "HyFive.Admin.Api";
        protected override Type ApiType { get; } = typeof(StartupAdmin);
        
        public StartupAdmin(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services); // Ensure the base setup is applied

            services.AddDbContext<HandHygieneContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("HandhygieneConnection")));

            // Add other services needed
        }
    }
}
