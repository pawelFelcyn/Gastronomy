using Gastronomy.Core.Abstractions.MessageBoxes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Diagnostics;
using MessageBoxOptions = Gastronomy.Core.Abstractions.MessageBoxes.MessageBoxOptions;

namespace Gastronomy.UI.Shared.MessageBox;

public partial class BlazorMessageBox
{
    [CascadingParameter]
    public MudDialogInstance? DialogInstance { get; set; }
    [Parameter]
    public string Message { get; set; } = null!;
    [Parameter]
    public MessageIcon Icon { get; set; }
    [Parameter]
    public MessageBoxOptions Options { get; set; }

    [Inject]
    public IStringLocalizer<Resources.Locales.Resource> Localizer { get; set; } = null!;

    private void CloseWithResult(MessageBoxResult result)
    {
        DialogInstance?.Close(DialogResult.Ok(result));
    }

    private string IconString => Icon switch
    {
        MessageIcon.Question => Icons.Material.Filled.QuestionMark,
        _ => throw new UnreachableException()
    };
}