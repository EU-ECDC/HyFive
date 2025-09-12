using System;
using System.IO;
using HyFive.Api.Common;
using HyFive.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HyFive.Admin
{
    public class ProgramAdmin 
    {
        

        protected static readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        protected static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                Env == "Development" ? "appsettings.Development.json" : "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        protected static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        protected static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupAdmin>();
                })
                .UseSerilog();
    }
}
