using Gastronomy.Backend.Database;
using Gastronomy.Dtos.Validators;
using Gastronomy.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Gastronomy.Core.Web;

public sealed class CreateDishDtoValidationService : ICreateDishDtoValidationService
{
    private readonly GastronomyDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public CreateDishDtoValidationService(GastronomyDbContext dbContext, 
        IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }

    public Task<bool> DishCategoryExists(Guid guid)
    {
        try
        {
            _dbContext.Semaphore.WaitAsync();
            return _dbContext.DishCategories.AnyAsync(x => x.Id == guid);
        }
        finally
        {
            _dbContext.Semaphore.Release();   
        }
    }

    public async Task<bool> IsNameTaken(string name)
    {
        try
        {
            await _dbContext.Semaphore.WaitAsync();
            var restaurantId = await _userContextService.RestaurentId;
            return await _dbContext.Dishes.AnyAsync(x => x.Name == name && x.DishCategory!.RestaurantId == restaurantId);
        }
        finally
        {
            _dbContext.Semaphore.Release();
        }
    }

    public async Task<bool> IsNewCategoryNameTaken(string name)
    {
        try
        {
            await _dbContext.Semaphore.WaitAsync();
            var restaurantId = await _userContextService.RestaurentId;
            return await _dbContext.DishCategories.AnyAsync(x => x.Name == name && x.RestaurantId == restaurantId);
        }
        finally
        {
            _dbContext.Semaphore.Release();
        }
    }
}