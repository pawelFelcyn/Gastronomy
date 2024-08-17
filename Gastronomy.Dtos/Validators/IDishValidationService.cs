namespace Gastronomy.Dtos.Validators;

public interface IDishValidationService
{
    Task<bool> IsNameTaken(string name, Guid dishId);
    Task<bool> IsNewCategoryNameTaken(string name);
    Task<bool> DishCategoryExists(Guid guid);
}