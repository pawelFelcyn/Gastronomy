namespace Gastronomy.Core.Abstractions.MessageBoxes;

public interface IMessageBoxService
{
    Task<MessageBoxResult> Show(string? caption, string message, MessageBoxOptions options, MessageIcon icon);
}