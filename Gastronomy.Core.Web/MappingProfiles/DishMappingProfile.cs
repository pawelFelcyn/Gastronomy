using AutoMapper;
using Gastronomy.Domain;
using Gastronomy.Dtos;

namespace Gastronomy.Core.Web.MappingProfiles;

public sealed class DishMappingProfile : Profile
{
    public DishMappingProfile()
    {
        CreateMap<CreateDishDto, Dish>()
            .ForMember(d => d.DishCategoryId, c => c.MapFrom(s => s.ExistingCategoryId));

        CreateMap<Dish, DishDetailsDto>();

        CreateMap<DishDetailsDto, UpdateDishDto>()
            .ForMember(d => d.ExistingDishCategoryId, o => o.MapFrom(s => s.DishCategoryId));
    }
}