using Gastronomy.Backend.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Gastronomy.Core.Web.DependencyInjection;
using Gastronomy.Domain;

namespace Gastronomy.Core.Web.Tests;

public class TestContext : IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlDbContainer = new MsSqlBuilder().Build();
    public IServiceProvider Services { get; private set; } = null!;

    public async Task DisposeAsync()
    {
        await _mssqlDbContainer.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _mssqlDbContainer.StartAsync();
        Services = new ServiceCollection()
            .AddDbContext<GastronomyDbContext>(options => options.UseSqlServer(
            _mssqlDbContainer.GetConnectionString(), x => x.MigrationsAssembly("Gastronomy.Backend.Database.MSSQL")))
            .AddCore()
            .BuildServiceProvider();

        using var scope = Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GastronomyDbContext>();
        if (dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync();
        }
    }

    public async Task<Restaurant> SeedRestaurantWithExampleDishCategories()
    {
        using var scope = Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GastronomyDbContext>();
        var restaurant = new Restaurant
        {
            Name = Guid.NewGuid().ToString(),
            DishCategories = [
                new DishCategory
                {
                    Name = "Drinks"
                },
                new DishCategory
                {
                    Name = "Main"
                },
                new DishCategory
                {
                    Name = "Soups"
                }
                ]
        };
        await dbContext.AddAsync(restaurant);
        await dbContext.SaveChangesAsync();
        return restaurant;
    }
}