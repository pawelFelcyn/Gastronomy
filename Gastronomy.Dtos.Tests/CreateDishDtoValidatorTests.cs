using FluentValidation.TestHelper;
using Gastronomy.Dtos.Resources;
using Gastronomy.Dtos.Validators;
using Microsoft.Extensions.Localization;
using NSubstitute;

namespace Gastronomy.Dtos.Tests;

public sealed class CreateDishDtoValidatorTests
{
    private readonly IStringLocalizer<Resource> _stringLocalizer;
    private readonly ICreateDishDtoValidationService _validationService;
    private readonly CreateDishDtoValidator _validator;

    public CreateDishDtoValidatorTests()
    {
        _stringLocalizer = Substitute.For<IStringLocalizer<Resource>>();
        _validationService = Substitute.For<ICreateDishDtoValidationService>();

        _stringLocalizer["TooLongDishNameErrorMessage"].Returns(new LocalizedString("TooLongDishNameErrorMessage", "Dish name is too long."));
        _stringLocalizer["EmptyDishNameErrorMessage"].Returns(new LocalizedString("EmptyDishNameErrorMessage", "Dish name cannot be empty."));
        _stringLocalizer["DishNameTakenErrorMessage"].Returns(new LocalizedString("DishNameTakenErrorMessage", "Dish name is already taken."));
        _stringLocalizer["PriceLessOrEqualToErrorMessage"].Returns(new LocalizedString("PriceLessOrEqualToErrorMessage", "Price must be greater than zero."));
        _stringLocalizer["NewCategoryNameEmptyErrorMessage"].Returns(new LocalizedString("NewCategoryNameEmptyErrorMessage", "New category name cannot be empty."));
        _stringLocalizer["TooLongDishCategoryNameErrorMessage"].Returns(new LocalizedString("TooLongDishCategoryNameErrorMessage", "Category name is too long."));
        _stringLocalizer["DishCategoryNameTakenErrorMessage"].Returns(new LocalizedString("DishCategoryNameTakenErrorMessage", "Category name is already taken."));
        _stringLocalizer["NewDishCategoryMustBeNullWhenExistingSelectedErrorMessage"].Returns(new LocalizedString("NewDishCategoryMustBeNullWhenExistingSelectedErrorMessage", "New category name must be null when an existing category is selected."));
        _stringLocalizer["ThisDishCategoryDoesNotExistErrorMessage"].Returns(new LocalizedString("ThisDishCategoryDoesNotExistErrorMessage", "This dish category does not exist."));
        _validator = new CreateDishDtoValidator(_stringLocalizer, _validationService);
    }

    [Fact]
    public async Task Validate_ShouldReturnNoErrors_ForValidDto()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish Name",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = "Valid Category"
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsTooLong()
    {
        var dto = new CreateDishDto
        {
            Name = new string('A', 51),
            BasePrice = 10m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Dish name is too long.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsEmpty()
    {
        var dto = new CreateDishDto
        {
            Name = "",
            BasePrice = 10m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Dish name cannot be empty.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsTaken()
    {
        _validationService.IsNameTaken("Taken Dish Name").Returns(Task.FromResult(true));

        var dto = new CreateDishDto
        {
            Name = "Taken Dish Name",
            BasePrice = 10m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Dish name is already taken.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenPriceIsZero()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 0m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.BasePrice)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsEmpty_AndExistingCategoryIdIsNull()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = ""
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("New category name cannot be empty.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsTooLong()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = new string('A', 51)
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("Category name is too long.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsTaken()
    {
        _validationService.IsNewCategoryNameTaken("Taken Category Name").Returns(Task.FromResult(true));

        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = "Taken Category Name"
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("Category name is already taken.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsNotNull_AndExistingCategoryIdIsProvided()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = Guid.NewGuid(),
            NewCategoryName = "New Category"
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("New category name must be null when an existing category is selected.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenExistingCategoryIdDoesNotExist()
    {
        _validationService.DishCategoryExists(Arg.Any<Guid>()).Returns(Task.FromResult(false));

        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.ExistingCategoryId)
            .WithErrorMessage("This dish category does not exist.");
    }
}