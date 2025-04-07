using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HyFive.Dataaksess
{
    public class HandhygieneContextFactory : IDesignTimeDbContextFactory<HandhygieneContext>
    {
        public HandhygieneContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HandhygieneContext>();
            var connectionString = configuration.GetConnectionString("HandhygieneConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new HandhygieneContext(optionsBuilder.Options);
        }
    }
}
