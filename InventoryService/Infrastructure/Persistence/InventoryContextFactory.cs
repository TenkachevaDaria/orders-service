using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryService.Infrastructure.Persistence;

public class InventoryContextFactory : IDesignTimeDbContextFactory< InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=inventory-db;Username=postgres;Password=root");
        
        return new InventoryDbContext(optionsBuilder.Options);
    }
    
}