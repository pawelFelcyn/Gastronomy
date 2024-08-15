using Gastronomy.Dtos;
using LanguageExt.Common;

namespace Gastronomy.Services.Abstractions;

public interface IDishCategoryService
{
    Task<Result<IEnumerable<DishCategoryDto>>> GetAllCategories(Guid restaurantId);
}