using FluentValidation;
using Gastronomy.Dtos.Resources;
using Microsoft.Extensions.Localization;

namespace Gastronomy.Dtos.Validators;

public sealed class UpdateDishDtoValidator : UpsertDishValidator<UpdateDishDto>
{
    public UpdateDishDtoValidator(IStringLocalizer<Resource> stringLocalizer,
        IDishValidationService createDishDtoValidationService)
        : base (stringLocalizer, createDishDtoValidationService)
    {
        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.MaxDishDescriptionLength)
            .WithMessage(string.Format(stringLocalizer["TooLongDishDescriptionValidationMessage"], ValidationConstants.MaxDishDescriptionLength));
        RuleFor(x => x.Name)
            .MustAsync(async (model, name, ctx, token) => name is null || !await createDishDtoValidationService.IsNameTaken(name, model.Id))
            .WithMessage(stringLocalizer["DishNameTakenErrorMessage"]);
    }
}