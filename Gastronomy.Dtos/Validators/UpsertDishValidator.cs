using FluentValidation;
using Gastronomy.Dtos.Resources;
using Microsoft.Extensions.Localization;

namespace Gastronomy.Dtos.Validators;

public abstract class UpsertDishValidator<T> : AbstractValidator<T> where T : IUpsertDish
{
    protected UpsertDishValidator(IStringLocalizer<Resource> stringLocalizer,
        IDishValidationService createDishDtoValidationService)
    {
        RuleFor(x => x.Name)
            .MaximumLength(ValidationConstants.MaxDishNameLength)
            .WithMessage(string.Format(stringLocalizer["TooLongDishNameErrorMessage"], ValidationConstants.MaxDishNameLength));
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(stringLocalizer["EmptyDishNameErrorMessage"]);
        RuleFor(x => x.BasePrice)
            .GreaterThan(ValidationConstants.MinDishBasePrice)
            .WithMessage(string.Format(stringLocalizer["PriceLessOrEqualToErrorMessage"], ValidationConstants.MinDishBasePrice));

        When(x => x.IsNewCategory, () =>
        {
            RuleFor(x => x.NewCategoryName)
                .NotEmpty()
                .WithMessage(stringLocalizer["NewCategoryNameEmptyErrorMessage"]);
            RuleFor(x => x.NewCategoryName)
                .MaximumLength(ValidationConstants.MaxDishCategoryNameLength)
                .WithMessage(string.Format(stringLocalizer["TooLongDishCategoryNameErrorMessage"], ValidationConstants.MaxDishCategoryNameLength));
            RuleFor(x => x.NewCategoryName)
                .MustAsync(async (x, token) => x is null || !await createDishDtoValidationService.IsNewCategoryNameTaken(x))
                .WithMessage(stringLocalizer["DishCategoryNameTakenErrorMessage"]);
            RuleFor(x => x.ExistingCategoryId)
                .Null()
                .WithMessage(stringLocalizer["ExistingDishCategoryMustBeNullWhenNewSelectedErrorMessage"]);
        }).Otherwise(() =>
        {
            RuleFor(x => x.NewCategoryName)
                .Null()
                .WithMessage(stringLocalizer["NewDishCategoryMustBeNullWhenExistingSelectedErrorMessage"]);
            RuleFor(x => x.ExistingCategoryId)
                .MustAsync(async (x, _) => x is null || await createDishDtoValidationService.DishCategoryExists(x!.Value))
                .WithMessage(stringLocalizer["ThisDishCategoryDoesNotExistErrorMessage"]);
            RuleFor(x => x.ExistingCategoryId)
                .NotNull()
                .WithMessage(stringLocalizer["NullExistingCategoryErrorMessage"]);
        });
    }
}