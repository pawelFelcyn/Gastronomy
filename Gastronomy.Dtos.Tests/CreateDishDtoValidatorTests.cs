using FluentValidation.TestHelper;
using Gastronomy.Dtos.Resources;
using Gastronomy.Dtos.Validators;
using Microsoft.Extensions.Localization;
using NSubstitute;

namespace Gastronomy.Dtos.Tests;

public sealed class CreateDishDtoValidatorTests
{
    private readonly IStringLocalizer<Resource> _stringLocalizer;
    private readonly IDishValidationService _validationService;
    private readonly CreateDishDtoValidator _validator;

    public CreateDishDtoValidatorTests()
    {
        _stringLocalizer = Substitute.For<IStringLocalizer<Resource>>();
        _validationService = Substitute.For<IDishValidationService>();

        _stringLocalizer["TooLongDishNameErrorMessage"].Returns(new LocalizedString("TooLongDishNameErrorMessage", "Dish name is too long."));
        _stringLocalizer["EmptyDishNameErrorMessage"].Returns(new LocalizedString("EmptyDishNameErrorMessage", "Dish name cannot be empty."));
        _stringLocalizer["DishNameTakenErrorMessage"].Returns(new LocalizedString("DishNameTakenErrorMessage", "Dish name is already taken."));
        _stringLocalizer["PriceLessOrEqualToErrorMessage"].Returns(new LocalizedString("PriceLessOrEqualToErrorMessage", "Price must be greater than zero."));
        _stringLocalizer["NewCategoryNameEmptyErrorMessage"].Returns(new LocalizedString("NewCategoryNameEmptyErrorMessage", "New category name cannot be empty."));
        _stringLocalizer["TooLongDishCategoryNameErrorMessage"].Returns(new LocalizedString("TooLongDishCategoryNameErrorMessage", "Category name is too long."));
        _stringLocalizer["DishCategoryNameTakenErrorMessage"].Returns(new LocalizedString("DishCategoryNameTakenErrorMessage", "Category name is already taken."));
        _stringLocalizer["NewDishCategoryMustBeNullWhenExistingSelectedErrorMessage"].Returns(new LocalizedString("NewDishCategoryMustBeNullWhenExistingSelectedErrorMessage", "New category name must be null when an existing category is selected."));
        _stringLocalizer["ExistingDishCategoryMustBeNullWhenNewSelectedErrorMessage"].Returns(new LocalizedString("ExistingDishCategoryMustBeNullWhenNewSelectedErrorMessage", "Existing category must be null when a new category provided."));
        _stringLocalizer["ThisDishCategoryDoesNotExistErrorMessage"].Returns(new LocalizedString("ThisDishCategoryDoesNotExistErrorMessage", "This dish category does not exist."));
        _stringLocalizer["NullExistingCategoryErrorMessage"].Returns(new LocalizedString("NullExistingCategoryErrorMessage", "Existing category must not be null."));
        _validator = new CreateDishDtoValidator(_stringLocalizer, _validationService);
    }


    [Fact]
    public async Task Validate_ShouldReturnNoErrors_ForValidDtoWithNewCategoryName()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish Name",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = "Valid Category",
            IsNewCategory = true
        };

        var validationService = Substitute.For<IDishValidationService>();
        validationService.IsNameTaken(Arg.Any<string>(), Arg.Any<Guid>()).Returns(Task.FromResult(false)); ;
        validationService.IsNewCategoryNameTaken(Arg.Any<string>()).Returns(Task.FromResult(false));
        var validator = new CreateDishDtoValidator(_stringLocalizer, validationService);
        var result = await validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldReturnNoErrors_ForValidDtoWithExistingCategory()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish Name",
            BasePrice = 10m,
            ExistingCategoryId = Guid.NewGuid(),
            IsNewCategory = false,
        };

        var validationService = Substitute.For<IDishValidationService>();
        validationService.IsNameTaken(Arg.Any<string>(), Arg.Any<Guid>()).Returns(Task.FromResult(false));
        validationService.DishCategoryExists(Arg.Any<Guid>()).Returns(Task.FromResult(true));
        var validator = new CreateDishDtoValidator(_stringLocalizer, validationService);
        var result = await validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsTooLong()
    {
        var dto = new CreateDishDto
        {
            Name = new string('A', ValidationConstants.MaxDishNameLength + 1),
            BasePrice = 10m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Dish name is too long.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_ShouldReturnError_WhenNameIsEmptyOrNull(string name)
    {
        var dto = new CreateDishDto
        {
            Name = name,
            BasePrice = 10m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Dish name cannot be empty.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsTaken()
    {
        _validationService.IsNameTaken("Taken Dish Name", Arg.Any<Guid>()).Returns(Task.FromResult(true));

        var dto = new CreateDishDto
        {
            Name = "Taken Dish Name",
            BasePrice = 10m
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Dish name is already taken.");
    }

    public static IEnumerable<object[]> GetInvalidPrices()
    {
        yield return [0m];
        yield return [-1m];
        yield return [-2m];
        yield return [-10.53m];
    }

    [Theory]
    [MemberData(nameof(GetInvalidPrices))]
    public async Task Validate_ShouldReturnError_WhenPriceIsZeroOrLess(decimal price)
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = price
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.BasePrice)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsNullOrEmpty_AndIsNewCategoryIsTrue(string newCategoryName)
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            NewCategoryName = newCategoryName,
            IsNewCategory = true
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("New category name cannot be empty.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenIsNewCategoryIsFalseAndNoExistingCategoryIsGiven()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            IsNewCategory = false
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.ExistingCategoryId)
            .WithErrorMessage("Existing category must not be null.");
    }

    [Fact]
    public async Task ShouldReturnError_WhenNewCategoryNameIsGivenButIsNewCategoryIsFalse()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            IsNewCategory = false,
            NewCategoryName = "Category name"
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("New category name must be null when an existing category is selected.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsTooLong()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = new string('A', ValidationConstants.MaxDishCategoryNameLength + 1),
            IsNewCategory = true
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("Category name is too long.");
    }

    [Fact]
    public async Task ShouldReturnError_WhenExistingCategoryIsGivenButIsNewCategoryIsTrue()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = Guid.NewGuid(),
            NewCategoryName = "Name",
            IsNewCategory = true
        };

        var result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.ExistingCategoryId)
            .WithErrorMessage("Existing category must be null when a new category provided.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNewCategoryNameIsTaken()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = null,
            NewCategoryName = "Taken Category Name",
            IsNewCategory = true
        };

        var validationService = Substitute.For<IDishValidationService>();
        validationService.IsNewCategoryNameTaken("Taken Category Name").Returns(Task.FromResult(true));
        var validator = new CreateDishDtoValidator(_stringLocalizer, validationService);
        var result = await validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewCategoryName)
            .WithErrorMessage("Category name is already taken.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenExistingCategoryIdDoesNotExist()
    {
        var dto = new CreateDishDto
        {
            Name = "Valid Dish",
            BasePrice = 10m,
            ExistingCategoryId = Guid.NewGuid()
        };

        var validationService = Substitute.For<IDishValidationService>();
        validationService.DishCategoryExists(Arg.Any<Guid>()).Returns(Task.FromResult(false));
        var validator = new CreateDishDtoValidator(_stringLocalizer, validationService);
        var result = await validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.ExistingCategoryId)
            .WithErrorMessage("This dish category does not exist.");
    }
}