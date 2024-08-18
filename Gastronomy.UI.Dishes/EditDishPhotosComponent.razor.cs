using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using Gastronomy.UI.Dishes.Resources.Locales;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Gastronomy.UI.Dishes;

public partial class EditDishPhotosComponent
{
    private readonly List<string> _photos = [];

    [Parameter]
    public Guid DishId { get; set; }
    [Inject]
    public IStringLocalizer<Resource> Localizer { get; set; } = null!;
    [Inject]
    public IPhotosService PhotosService { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public ILogger<EditDishPhotosComponent> Logger { get; set; } = null!;

    private async Task UpladFiles(IReadOnlyList<IBrowserFile> files)
    {
        try
        {
            var dtos = files.Select(f => new UploadPhotoDto
            {
                Extension = Path.GetExtension(f.Name),
                FileStream = f.OpenReadStream()
            }).ToArray();
            var result = await PhotosService.UploadDishPhoto(DishId, dtos);

            result.IfSucc(_photos.AddRange);
            result.IfFail(ex =>
            {
                if (ex is NotFoundException)
                {
                    Snackbar.Add(Localizer["UploadPhotosFailedNotFound"], Severity.Error);
                }

                if (ex is ForbidException)
                {
                    Snackbar.Add(Localizer["UploadPhotosForbidden"], Severity.Error);
                }

                if (ex is CreatingDishImageFailedException)
                {
                    Snackbar.Add(Localizer["UploadPhotoFailed"], Severity.Error);
                }
            });
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown on uploading images to blob account");
            Snackbar.Add(Localizer["UploadPhotoFailed"], Severity.Error);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            _photos.Clear();
            var result = await PhotosService.GetDishPhotos(DishId);
            result.IfSucc(_photos.AddRange);
            result.IfFail(ex =>
            {
                if (ex is GettingDishImageFailedException)
                {
                    Snackbar.Add(Localizer["CouldNotLoadDidhPhotos"], Severity.Error);
                }
            });
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown on getting images of a dish");
            Snackbar.Add(Localizer["CouldNotLoadDidhPhotos"], Severity.Error);
        }
    }
}