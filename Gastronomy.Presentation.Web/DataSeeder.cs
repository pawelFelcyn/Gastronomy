using Gastronomy.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Gastronomy.Presentation.Web;

public class DataSeeder
{
    public static async Task SeedData(GastronomyDbContext ctx)
    {
        if (!await ctx.Restaurants.AnyAsync())
        {
            await ctx.Restaurants.AddAsync(new() 
            {
                Name = "Sample restaurant"
            });
        }
        await ctx.SaveChangesAsync();
    }
}
