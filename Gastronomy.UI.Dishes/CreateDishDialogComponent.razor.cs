using Gastronomy.Dtos;

namespace Gastronomy.UI.Dishes;

public partial class CreateDishDialogComponent
{
    public CreateDishDto Model { get; } = new();
    private readonly List<DishCategoryDto> _categories = new();
    private bool _createNewCategory = false;
    private bool CreateNewCategory 
    {
        get => _createNewCategory;
        set
        {
            if (value == _createNewCategory)
            {
                return;
            }

            _createNewCategory = value;
            if (value)
            {
                Model.ExistingCategoryId = null;
                return;
            }

            Model.NewCategoryName = null;
        }
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            LoadCategories();
        }
        return Task.CompletedTask;
    }

    private void LoadCategories()
    {
        _categories.AddRange([
            new DishCategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Danie główne"
            },
             new DishCategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Zupy"
            },
             new DishCategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Desery"
            }
            ]);
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
}