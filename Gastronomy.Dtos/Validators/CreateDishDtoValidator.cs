using FluentValidation;
using Gastronomy.Dtos.Resources;
using Microsoft.Extensions.Localization;

namespace Gastronomy.Dtos.Validators;

public sealed class CreateDishDtoValidator : UpsertDishValidator<CreateDishDto>
{
    public CreateDishDtoValidator(IStringLocalizer<Resource> stringLocalizer, 
        IDishValidationService createDishDtoValidationService) :
        base(stringLocalizer, createDishDtoValidationService)
    {
        RuleFor(x => x.Name)
           .MustAsync(async (x, token) => x is null || !await createDishDtoValidationService.IsNameTaken(x, Guid.NewGuid()))
           .WithMessage(stringLocalizer["DishNameTakenErrorMessage"]);
    }
}