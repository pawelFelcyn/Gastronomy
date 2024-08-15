using FluentValidation;

namespace Gastronomy.UI.Shared.Extensions;

public static class ValidatorExtensions
{
    public static async Task<IEnumerable<string>> ValidateSingleProperty<T>(this IValidator<T> validator, T model, string propertyName)
    {
        return (await validator.ValidateAsync(model, x => x.IncludeProperties(propertyName)))
            .Errors.Select(x => x.ErrorMessage);
    }
}