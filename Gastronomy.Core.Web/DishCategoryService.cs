using AutoMapper;
using Gastronomy.Backend.Database;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Gastronomy.Core.Web;

public sealed class DishCategoryService : IDishCategoryService
{
    private readonly GastronomyDbContext _dbContext;
    private readonly IMapper _mapper;

    public DishCategoryService(GastronomyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<DishCategoryDto>>> GetAllCategories(Guid restaurantId)
    {
        var categories = await
            _dbContext
            .DishCategories
            .Where(dc => dc.RestaurantId == restaurantId)
            .ToListAsync();

        return new(_mapper.Map<IEnumerable<DishCategoryDto>>(categories));
    }
}