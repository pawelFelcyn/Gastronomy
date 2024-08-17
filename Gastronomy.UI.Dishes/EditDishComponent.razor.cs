using AutoMapper;
using FluentValidation;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Gastronomy.UI.Dishes;

public partial class EditDishComponent
{
    private UpdateDishDto? _model;
    private bool _isInEditMode;
    private bool _modified;
    private MudForm _form = null!;

    [Inject]
    public IMapper Mapper { get; set; } = null!;
    [Parameter]
    public DishDetailsDto? DishDetails { get; set; }
    [Inject]
    public IStringLocalizer<Resources.Locales.Resource> Localizer { get; set; } = null!;
    [Parameter]
    public List<DishCategoryDto>? Categories { get; set; }
    [Inject]
    public IValidator<UpdateDishDto> Validator { get; set; } = null!;
    [Inject]
    public IDishService DishService { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public ILogger<EditDishComponent> Logger { get; set; } = null!;

    private bool CreateNewCategory
    {
        get => _model?.IsNewCategory ?? false;
        set
        {
            if (_model is null || value == _model.IsNewCategory)
            {
                return;
            }

            _model.IsNewCategory = value;
            if (value)
            {
                _model.ExistingCategoryId = null;
                return;
            }

            _model.NewCategoryName = null;
        }
    }

    private DishCategoryDto? SelectedCategory 
    { 
        get => Categories?.FirstOrDefault(c => _model is not null && c.Id == _model.ExistingCategoryId);
        set
        {
            if (_model is not null)
            {
                _model.ExistingCategoryId = value?.Id;
            }
        }
    }

    protected override void OnParametersSet()
    {
        RevertChanges();
    }

    private void SelectedCategoryValueChanged(DishCategoryDto? newCategory)
    {
        if (_model is not null)
        {
            _model.ExistingCategoryId = newCategory?.Id;
        }
    }

    private void FormulateFieldChanged()
    {
        _modified = true;
    }

    private void RevertChanges()
    {
        _model = Mapper.Map<UpdateDishDto>(DishDetails);
        _modified = false;
    }

    private async Task Save()
    {
        if (DishDetails is null || _model is null)
        {
            return;
        }

        try
        {
            await _form.Validate();
            if (!_form.IsValid)
            {
                return;
            }

            var result = await DishService.Update(DishDetails.Id, _model);
            result.IfSucc(d =>
            {
                DishDetails = d;
                RevertChanges();
                _isInEditMode = false;
                Snackbar.Add(Localizer["SuccessfulyUpdatedDishMessage"], MudBlazor.Severity.Success);
            });

            result.IfFail(_ =>
            {
                Snackbar.Add(Localizer["FailedUpdatingDishMessage"], MudBlazor.Severity.Error);
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed updating the dish");
            Snackbar.Add(Localizer["FailedUpdatingDishMessage"], MudBlazor.Severity.Error);
        }
    }
}