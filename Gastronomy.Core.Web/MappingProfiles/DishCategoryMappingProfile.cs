using AutoMapper;
using Gastronomy.Domain;
using Gastronomy.Dtos;

namespace Gastronomy.Core.Web.MappingProfiles;

public sealed class DishCategoryMappingProfile : Profile
{
    public DishCategoryMappingProfile()
    {
        CreateMap<DishCategory, DishCategoryDto>();
    }
}