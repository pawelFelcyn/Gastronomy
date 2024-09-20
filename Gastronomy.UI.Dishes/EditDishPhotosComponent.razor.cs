using Gastronomy.Core.Abstractions.MessageBoxes;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using Gastronomy.UI.Dishes.Resources.Locales;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MessageBoxOptions = Gastronomy.Core.Abstractions.MessageBoxes.MessageBoxOptions;

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
    [Inject]
    public IMessageBoxService MessageBoxService { get; set; } = null!;

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
                else if (ex is ForbidException)
                {
                    Snackbar.Add(Localizer["UploadPhotosForbidden"], Severity.Error);
                }
                else if (ex is CreatingDishImageFailedException)
                {
                    Snackbar.Add(Localizer["UploadPhotoFailed"], Severity.Error);
                }
                else
                {
                    Logger.LogError(ex, "Exception thrown on uploading images to blob account");
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


    protected override void OnParametersSet()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await InvokeAsync(() => 
                {
                    _photos.Clear();
                });
                var result = await PhotosService.GetDishPhotos(DishId);
                await InvokeAsync(() =>
                {
                    result.IfSucc(_photos.AddRange);
                });
                result.IfFail(ex =>
                {
                    if (ex is GettingDishImageFailedException)
                    {
                        Snackbar.Add(Localizer["CouldNotLoadDidhPhotos"], Severity.Error);
                    }
                    else
                    {
                        Logger.LogError(ex, "Exception thrown on getting images of a dish");
                        CouldNotLoadDidhPhotosSnackbar();
                    }
                });
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Exception thrown on getting images of a dish");
                await CouldNotLoadDidhPhotosSnackbar();
            }
        });
    }

    private async Task CouldNotLoadDidhPhotosSnackbar()
    {
        await InvokeAsync(() =>
        {
            Snackbar.Add(Localizer["CouldNotLoadDidhPhotos"], Severity.Error);
        });
    }

    private async Task DeletePhoto(string url)
    {
        try
        {
            var confirmationQuestion = Localizer["DeleteDishPhotoConfirmationQuestion"];
            var confirmationResult = await MessageBoxService.Show(
                null, confirmationQuestion, MessageBoxOptions.ConfirmCancel, MessageIcon.Question);

            if (confirmationResult != MessageBoxResult.Confirm)
            {
                return;
            }

            string name = SubstractImageName(url);
            var result = await PhotosService.DeleteDishPhoto(DishId, name);

            result.IfSucc(_ =>
            {
                _photos.Remove(url);
            });

            result.IfFail(ex =>
            {
                if (ex is ForbidException)
                {
                    Snackbar.Add(Localizer["DeletingPhotoForbidden"], Severity.Error);
                }
                else if (ex is NotFoundException)
                {
                    _photos.Remove(name);
                }
                else if (ex is DeletingDishImageFailedException)
                {
                    Snackbar.Add(Localizer["DeletingImageFailed"], Severity.Error);
                    Logger.LogError(ex, "Exception thrown on deleting image of a dish");
                }
            });
        }
        catch (Exception e)
        {
            Snackbar.Add(Localizer["DeletingImageFailed"], Severity.Error);
            Logger.LogError(e, "Exception thrown on deleting image of a dish");
        }
    }

    private string SubstractImageName(string url)
    {
        if (url == "")
        {
            return url;
        }

        var lastSlesh = url.LastIndexOf('/');
        return url.Substring(lastSlesh + 1);
    }
}