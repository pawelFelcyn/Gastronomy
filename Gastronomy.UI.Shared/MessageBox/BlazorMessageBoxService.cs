using Gastronomy.Core.Abstractions.MessageBoxes;
using MudBlazor;
using MessageBoxOptions = Gastronomy.Core.Abstractions.MessageBoxes.MessageBoxOptions;

namespace Gastronomy.UI.Shared.MessageBox;

public sealed class BlazorMessageBoxService : IMessageBoxService
{
    private readonly IDialogService _dialogService;

    public BlazorMessageBoxService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<MessageBoxResult> Show(string? caption, string message, MessageBoxOptions options, MessageIcon icon)
    {
        var parameters = new DialogParameters<BlazorMessageBox>
        {
            { x => x.Message , message },
            { x => x.Options , options },
            { x => x.Icon , icon }
        };

        var dialogRef = await _dialogService.ShowAsync<BlazorMessageBox>(caption, parameters);
        var res = await dialogRef.Result;
        if (res is null || res.Data is null)
        {
            return MessageBoxResult.Unknown;
        }

        return (MessageBoxResult)res.Data;
    }
}