using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Gastronomy.Presentation.Web.Components.Pages;

public partial class DishPage
{
    private bool _rendered = false;
    private DishDetailsDto? _dishDetails;
    private List<DishCategoryDto> _allCategories = [];

    [Parameter]
    public Guid Id { get; set; }
    [Inject]
    public IDishService DishService { get; set; } = null!;
    [Inject]
    public IDishCategoryService DishCategoryService { get; set; } = null!;
    [Inject]
    public IUserContextService UserContextService { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        //if (!_rendered)
        //{
        //    return;
        //}

        await LoadDetails();
    }

    private async Task LoadDetails()
    {
        var detailsResult = await DishService.GetById(Id);
        detailsResult.IfSucc(d =>
        {
            _dishDetails = d;
        });
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        _rendered = true;

        if (firstRender)
        {
            await LoadCategories();
            StateHasChanged();
        }
    }

    private async Task LoadCategories()
    {
        _allCategories.Clear();
        var result = await DishCategoryService.GetAllCategories(await UserContextService.RestaurentId);
        result.IfSucc(_allCategories.AddRange);
    }
}