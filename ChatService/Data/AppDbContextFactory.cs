using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ChatService.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        var useInMemory = configuration.GetValue<bool>("Database:UseInMemory");
        if (useInMemory)
        {
            var inMemoryName = configuration.GetValue<string>("Database:InMemoryName") ?? "ChatServiceInMemory";
            optionsBuilder.UseInMemoryDatabase(inMemoryName);
        }
        else
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
        
        return new AppDbContext(optionsBuilder.Options);
    }
}