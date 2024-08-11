using Gastronomy.Dtos.Validators;
using Gastronomy.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;

namespace Gastronomy.Core.Web.DependencyInjection;

public static class DI
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services.AddScoped<IUserContextService, HttpUserContextService>()
            .AddTransient<ICreateDishDtoValidationService, CreateDishDtoValidationService>();
    }
}
