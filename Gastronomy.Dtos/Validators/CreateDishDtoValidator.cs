using FluentValidation;
using Gastronomy.Dtos.Resources;
using Microsoft.Extensions.Localization;

namespace Gastronomy.Dtos.Validators;

public sealed class CreateDishDtoValidator : AbstractValidator<CreateDishDto>
{
    private readonly int _maxNameLen = 50;
    private readonly decimal _minPrice = 0.0m;

    public CreateDishDtoValidator(IStringLocalizer<Resource> stringLocalizer, 
        ICreateDishDtoValidationService createDishDtoValidationService)
    {
        RuleFor(x => x.Name)
            .MaximumLength(_maxNameLen)
            .WithMessage(string.Format(stringLocalizer["TooLongDishNameErrorMessage"], _maxNameLen));
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(stringLocalizer["EmptyDishNameErrorMessage"]);
        RuleFor(x => x.Name)
            .MustAsync(async (x, token) => x is null || !await createDishDtoValidationService.IsNameTaken(x))
            .WithMessage(stringLocalizer["DishNameTakenErrorMessage"]);
        RuleFor(x => x.BasePrice)
            .GreaterThan(0)
            .WithMessage(stringLocalizer["PriceLessOrEqualToErrorMessage"]);

        When(x => x.ExistingCategoryId is null, () =>
        {
            RuleFor(x => x.NewCategoryName)
                .NotEmpty()
                .WithMessage(stringLocalizer["NewCategoryNameEmptyErrorMessage"]);
            RuleFor(x => x.NewCategoryName)
                .MaximumLength(_maxNameLen)
                .WithMessage(stringLocalizer["TooLongDishCategoryNameErrorMessage"]);
            RuleFor(x => x.NewCategoryName)
                .MustAsync(async (x, token) => x is null || !await createDishDtoValidationService.IsNewCategoryNameTaken(x))
                .WithMessage(stringLocalizer["DishCategoryNameTakenErrorMessage"]);
        }).Otherwise(() =>
        {
            RuleFor(x => x.NewCategoryName)
                .Null()
                .WithMessage(stringLocalizer["NewDishCategoryMustBeNullWhenExistingSelectedErrorMessage"]);
            RuleFor(x => x.ExistingCategoryId)
                .MustAsync(async (x, _) => await createDishDtoValidationService.DishCategoryExists(x!.Value))
                .WithMessage(stringLocalizer["ThisDishCategoryDoesNotExistErrorMessage"]);
        });
    }
}