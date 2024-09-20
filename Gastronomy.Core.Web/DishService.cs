using AutoMapper;
using Gastronomy.Backend.Database;
using Gastronomy.Domain;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Gastronomy.Core.Web;

public sealed class DishService : IDishService
{
    private readonly GastronomyDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public DishService(GastronomyDbContext dbContext, IMapper mapper,
        IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public async Task<Result<Guid>> Create(CreateDishDto dto)
    {
        var retaurantId = await _userContextService.RestaurentId;
        var dish = _mapper.Map<Dish>(dto);

        if (dto.IsNewCategory)
        {
            dish.DishCategory = new DishCategory
            {
                Name = dto.NewCategoryName!,
                RestaurantId = retaurantId
            };
        }

        await _dbContext.AddAsync(dish);
        await _dbContext.SaveChangesAsync();

        return dish.Id;
    }

    public async  Task<Result<DishDetailsDto>> GetById(Guid id)
    {
        var detailsResult = await GetDetails(id);
        return detailsResult.Map(d => _mapper.Map<DishDetailsDto>(d));
    }

    private async Task<Result<Dish>> GetDetails(Guid id)
    {
        var dishWithRestaurantId = await _dbContext
             .Dishes
             .Select(d => new
             {
                 Dish = d,
                 d.DishCategory!.RestaurantId
             })
             .FirstOrDefaultAsync(d => d.Dish.Id == id);

        if (dishWithRestaurantId is null)
        {
            return new(new NotFoundException());
        }

        if (dishWithRestaurantId.RestaurantId != await _userContextService.RestaurentId)
        {
            return new(new ForbidException());
        }

        return dishWithRestaurantId.Dish;
    }

    public async Task<Result<DishDetailsDto>> Update(Guid id, UpdateDishDto dto)
    {
        var detailsResult = await GetDetails(id);

        bool resourceChanged = false;
        detailsResult.IfSucc(dish => resourceChanged = dish.RowVersion > dto.RowVersion);

        if (resourceChanged)
        {
            return new(new ResourceChangedException());
        }

        try
        {
            return await detailsResult.MapAsync<DishDetailsDto>(async dish =>
            {
                var retaurantId = await _userContextService.RestaurentId;
                _mapper.Map(dto, dish);

                if (dto.IsNewCategory)
                {
                    dish.DishCategoryId = default;
                    dish.DishCategory = new DishCategory
                    {
                        Name = dto.NewCategoryName!,
                        RestaurantId = retaurantId
                    };
                }

                await _dbContext.SaveChangesAsync();
                return _mapper.Map<DishDetailsDto>(dish);
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(new ResourceChangedException());
        }
    }
}