using Gastronomy.Dtos;
using LanguageExt.Common;

namespace Gastronomy.Services.Abstractions;

public interface IDishService
{
    Task<Result<Guid>> Create(CreateDishDto dto);
    Task<Result<DishDetailsDto>> GetById(Guid id);
}