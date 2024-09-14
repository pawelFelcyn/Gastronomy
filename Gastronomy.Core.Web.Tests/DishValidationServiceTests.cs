using FluentAssertions;
using Gastronomy.Dtos.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Gastronomy.Core.Web.Tests;

[Collection("TestContextCollection")]
public class DishValidationServiceTests
{
    private readonly TestContext _testContext;

    public DishValidationServiceTests(TestContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async Task DishCategoryExists_WhenCategoryExists_ReturnsTrue()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();

        foreach (var category in restaurant.DishCategories!)
        {
            var result = await dishValdationService.DishCategoryExists(category.Id);
            result.Should().BeTrue();
        }
    }


    [Fact]
    public async Task IsNewCategoryNameTaken_WhenCategoryNameIsTaken_ReturnsTrue()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();

        foreach (var category in restaurant.DishCategories!)
        {
            var result = await dishValdationService.IsNewCategoryNameTaken(category.Name);
            result.Should().BeTrue();
        }
    }

    [Fact]
    public async Task IsNewCategoryNameTaken_WhenCategoryNameIsTakenButInAnotherRestaurant_ReturnsFalse()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var anotherRestaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        
        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();

        foreach (var category in anotherRestaurant.DishCategories!)
        {
            var result = await dishValdationService.IsNewCategoryNameTaken(category.Name);
            result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task IsNameTaken_WhenNameIsTakenByAnotherDish_ShouldReturnTrue()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var dish = await _testContext.SeedDish(restaurant.DishCategories!.First().Id);

        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();
        var result = await dishValdationService.IsNameTaken(dish.Name, Guid.NewGuid());
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsNameTaken_WhenNameIsTakenByAnotherDishButInAnotherRestaurant_ShouldReturnFalse()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var anotherRestaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var dish = await _testContext.SeedDish(anotherRestaurant.DishCategories!.First().Id);

        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();
        var result = await dishValdationService.IsNameTaken(dish.Name, Guid.NewGuid());
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsNameTaken_WhenNameIsTakenBySameDish_ShouldReturnFalse()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var dish = await _testContext.SeedDish(restaurant.DishCategories!.First().Id);

        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();
        var result = await dishValdationService.IsNameTaken(dish.Name, dish.Id);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsNameTaken_WhenNameIsNotTaken_ShouldReturnFalse()
    {
        var restaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        var dish = await _testContext.SeedDish(restaurant.DishCategories!.First().Id);

        var services = _testContext.WithUserContextService(restaurant.Id);
        using var scope = services.CreateScope();
        var dishValdationService = services.GetRequiredService<IDishValidationService>();
        var result = await dishValdationService.IsNameTaken(Guid.NewGuid().ToString(), dish.Id);
        result.Should().BeFalse();
    }
}