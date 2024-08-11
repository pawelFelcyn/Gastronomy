namespace Gastronomy.Dtos.Validators;

public interface ICreateDishDtoValidationService
{
    Task<bool> IsNameTaken(string name);
    Task<bool> IsNewCategoryNameTaken(string name);
    Task<bool> DishCategoryExists(Guid guid);
}