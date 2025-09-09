using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MarketplaceAPI.Data
{
    public class MarketplaceDbContextFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
    {
        public MarketplaceDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<MarketplaceDbContext>()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MarketplaceDbContext>();
            var connStr = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connStr);

            return new MarketplaceDbContext(optionsBuilder.Options);
        }
    }
}
