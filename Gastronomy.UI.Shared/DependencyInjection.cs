using Gastronomy.Core.Abstractions.MessageBoxes;
using Gastronomy.UI.Shared.MessageBox;
using Microsoft.Extensions.DependencyInjection;

namespace Gastronomy.UI.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        return services.AddTransient<IMessageBoxService, BlazorMessageBoxService>();
    }
}