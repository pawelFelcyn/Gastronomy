using AutoMapper;
using Gastronomy.Backend.Database;
using Gastronomy.Domain;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using LanguageExt.Common;

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

        if (dto.NewCategoryName is not null)
        {
            dish.DishCategory = new DishCategory
            {
                Name = dto.NewCategoryName,
                RestaurantId = retaurantId
            };
        }

        await _dbContext.AddAsync(dish);
        await _dbContext.SaveChangesAsync();

        return dish.Id;
    }
}