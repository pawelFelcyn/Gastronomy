using Gastronomy.Backend.Database.Configurations;
using Gastronomy.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gastronomy.Backend.Database;

public class GastronomyDbContext : DbContext
{
    public GastronomyDbContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<Restaurant> Restaurants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RestaurantConfiguration());
    }
}