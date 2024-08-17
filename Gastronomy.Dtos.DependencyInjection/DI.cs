using FluentValidation;
using Gastronomy.Dtos.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Gastronomy.Dtos.DependencyInjection;

public static class DI
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services.AddTransient<IValidator<CreateDishDto>, CreateDishDtoValidator>()
            .AddTransient<IValidator<UpdateDishDto>, UpdateDishDtoValidator>();
    }
}