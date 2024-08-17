using AutoMapper;
using Gastronomy.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Gastronomy.UI.Dishes;

public partial class EditDishComponent
{
    private UpdateDishDto? _model;
    private bool _isInEditMode;
    private bool _modified;

    [Inject]
    public IMapper Mapper { get; set; } = null!;
    [Parameter]
    public DishDetailsDto? DishDetails { get; set; }
    [Inject]
    public IStringLocalizer<Resources.Locales.Resource> Localizer { get; set; } = null!;
    [Parameter]
    public List<DishCategoryDto>? Categories { get; set; }

    private DishCategoryDto? SelectedCategory 
    { 
        get => Categories?.FirstOrDefault(c => _model is not null && c.Id == _model.ExistingDishCategoryId);
        set
        {
            if (_model is not null)
            {
                _model.ExistingDishCategoryId = value?.Id;
            }
        }
    }

    protected override void OnParametersSet()
    {
        _model = Mapper.Map<UpdateDishDto>(DishDetails);
    }

    private void SelectedCategoryValueChanged(DishCategoryDto? newCategory)
    {
        if (_model is not null)
        {
            _model.ExistingDishCategoryId = newCategory?.Id;
        }
    }

    private void FormulateFieldChanged()
    {
        _modified = true;
    }
}