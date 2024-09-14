using FluentAssertions;
using Gastronomy.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gastronomy.Core.Web.Tests;

[Collection("TestContextCollection")]
public class DishCategoryServiceTests
{
    private readonly TestContext _testContext;

    public DishCategoryServiceTests(TestContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async Task GetAllCategories_ForGivenRestaurantId_ShouldReturnAllDishCategories()
    {
        //Seed another restaurant just to make sure, that there are two restaurants, and still only one of them is returned
        await _testContext.SeedRestaurantWithExampleDishCategories();
        var seededRestaurant = await _testContext.SeedRestaurantWithExampleDishCategories();
        using var scope = _testContext.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDishCategoryService>();
        var result = await service.GetAllCategories(seededRestaurant.Id);
        result.IsSuccess.Should().BeTrue();
        result.IfSucc(categories =>
        {
            var dbCategoriesDict = seededRestaurant.DishCategories!.ToDictionary(x => x.Id);
            seededRestaurant.DishCategories!.Count.Should().Be(categories.Count());
            foreach (var category in categories)
            {
                var dbCategory = dbCategoriesDict.GetValueOrDefault(category.Id);
                dbCategory.Should().NotBeNull();
                category.Name.Should().Be(dbCategory!.Name);
            }
        });
    }
}