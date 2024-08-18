using AutoMapper;
using Azure.Storage.Blobs;
using Gastronomy.Core.Web.MappingProfiles;
using Gastronomy.Dtos.Validators;
using Gastronomy.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gastronomy.Core.Web.DependencyInjection;

public static class DI
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services.AddScoped<IUserContextService, HttpUserContextService>()
            .AddTransient<IDishValidationService, DishValidationService>()
            .AddTransient<IDishCategoryService, DishCategoryService>()
            .AddTransient<IDishService, DishService>()
            .AddSingleton<IMapper>(_ => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DishCategoryMappingProfile());
                cfg.AddProfile(new DishMappingProfile());
            }).CreateMapper())
            .AddTransient<IPhotosService, AzurePhotosService>()
            .AddTransient<BlobServiceClient>(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                var connectionString = cfg.GetConnectionString("AzureBlobStorage");
                return new(connectionString);
            });
    }
}
