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
            .ForMember(d => d.ExistingCategoryId, o => o.MapFrom(s => s.DishCategoryId));

        CreateMap<UpdateDishDto, Dish>()
            .ForMember(d => d.DishCategoryId, c => c.MapFrom(s => s.ExistingCategoryId))
            .ForMember(d => d.RowVersion, o => o.Ignore());
    }
}