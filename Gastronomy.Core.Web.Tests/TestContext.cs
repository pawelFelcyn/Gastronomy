using Gastronomy.Backend.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Gastronomy.Core.Web.DependencyInjection;
using Gastronomy.Domain;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Gastronomy.Services.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Gastronomy.Core.Web.Tests;

public class TestContext : IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlDbContainer = new MsSqlBuilder().Build();
    private IServiceCollection _serviceCollection = null!;
    public IServiceProvider Services { get; private set; } = null!;

    public async Task DisposeAsync()
    {
        await _mssqlDbContainer.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _mssqlDbContainer.StartAsync();
        _serviceCollection = new ServiceCollection()
            .AddDbContext<GastronomyDbContext>(options => options.UseSqlServer(
            _mssqlDbContainer.GetConnectionString(), x => x.MigrationsAssembly("Gastronomy.Backend.Database.MSSQL")))
            .AddCore();
        Services = _serviceCollection
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
                    Name = Guid.NewGuid().ToString()
                },
                new DishCategory
                {
                    Name = Guid.NewGuid().ToString()
                },
                new DishCategory
                {
                    Name = Guid.NewGuid().ToString()
                }
                ]
        };
        await dbContext.AddAsync(restaurant);
        await dbContext.SaveChangesAsync();
        return restaurant;
    }

    public async Task<Dish> SeedDish(Guid categoryId)
    {
        using var scope = Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GastronomyDbContext>();
        var dish = new Dish 
        {
            Name = Guid.NewGuid().ToString() ,
            BasePrice = 1m,
            DishCategoryId = categoryId
        };
        await dbContext.AddAsync(dish);
        await dbContext.SaveChangesAsync();
        return dish;
    }

    public IServiceProvider ConfigureServices(Action<IServiceCollection> servicesCfg)
    {
        var newServices = new ServiceCollection();

        foreach (var descriptor in _serviceCollection)
        {
            newServices.Add(descriptor);
        }

        servicesCfg(newServices);
        return newServices.BuildServiceProvider();
    }

    public IServiceProvider WithUserContextService(Guid restaurantId)
    {
        return ConfigureServices(services =>
        {
            var currentService = services.FirstOrDefault(x => x.ServiceType == typeof(IUserContextService));

            if (currentService is not null)
            {
                services.Remove(currentService);
            }

            var fake = new FakeUserContextService(() => Task.FromResult(restaurantId));

            services.AddSingleton<IUserContextService>(fake);
        });
    }
}