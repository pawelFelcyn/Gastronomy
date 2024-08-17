using FluentValidation;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using Gastronomy.UI.Dishes.Resources.Locales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Gastronomy.UI.Dishes;

public partial class CreateDishDialogComponent
{
    private bool _isValid;
    private MudForm _form = null!;

    [Inject]
    IStringLocalizer<Resource> Localizer { get; set; } = null!;
    [Inject]
    IValidator<CreateDishDto> Validator { get; set; } = null!;
    [Inject]
    IDishCategoryService DishCategoryService { get; set; } = null!;
    [Inject]
    IUserContextService UserContextService { get; set; } = null!;
    [Inject]
    public ILogger<CreateDishDialogComponent> Logger { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public IDishService DishService { get; set; } = null!;
    [CascadingParameter]
    public MudDialogInstance? DialogInstance { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public CreateDishDto Model { get; } = new();
    private readonly List<DishCategoryDto> _categories = new();
    private bool CreateNewCategory 
    {
        get => Model.IsNewCategory;
        set
        {
            if (value == Model.IsNewCategory)
            {
                return;
            }

            Model.IsNewCategory = value;
            if (value)
            {
                Model.ExistingCategoryId = null;
                return;
            }

            Model.NewCategoryName = null;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadCategories();
        }
    }

    private async Task LoadCategories()
    {
        var restaurantId = await UserContextService.RestaurentId;
        var categoriesResult = await DishCategoryService.GetAllCategories(restaurantId);
        categoriesResult.IfSucc(_categories.AddRange);
    }

    private void SelectedCategoryChanged(DishCategoryDto? newCategory)
    {
        if (newCategory is null)
        {
            Model.ExistingCategoryId = null;
            return;
        }

        Model.ExistingCategoryId = newCategory.Id;
    }

    private async Task Submit()
    {
        try
        {
            await _form.Validate();

            if (!_form.IsValid)
            {
                return;
            }

            var result = await DishService.Create(Model);

            result.IfSucc(id =>
            {
                DialogInstance?.Close();
                NavigationManager.NavigateTo($"/dishes/details/{id}");
            });
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An exception has been thrown while submitting a new dish");
            var snackbarMsg = Localizer["ErrorOnCreatingDish"];
            Snackbar.Add(snackbarMsg, MudBlazor.Severity.Error);
        }
    }

    private void Cancel()
    {
        DialogInstance?.Close();
    }
}