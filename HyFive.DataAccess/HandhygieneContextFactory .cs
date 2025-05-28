using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HyFive.DataAccess
{
    public class HandhygieneContextFactory : IDesignTimeDbContextFactory<HandHygieneContext>
    {
        public HandHygieneContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HandHygieneContext>();
            var connectionString = configuration.GetConnectionString("HandhygieneConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new HandHygieneContext(optionsBuilder.Options);
        }
    }
}
