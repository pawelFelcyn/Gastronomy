using Gastronomy.UI.Dishes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Gastronomy.Presentation.Web.Components.Pages;

public partial class DishesPage 
{
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    private void OpenCreateNewDishPopup()
    {
        DialogService.ShowAsync<CreateDishDialogComponent>("Dodaj nowe danie");
    }
}
