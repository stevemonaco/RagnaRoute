using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace RagnaRoute.Services;

public interface IClipboardService
{
    Task<bool> CopyTextAsync(string text);
}

public class ClipboardService : IClipboardService
{
    private IClipboard? _clipboard = GetClipboard();

    public async Task<bool> CopyTextAsync(string text)
    {
        if (_clipboard is not null)
        {
            await _clipboard.SetTextAsync(text);
            return true;
        }

        return false;
    }

    private static IClipboard? GetClipboard()
    {
        return Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => TopLevel.GetTopLevel(desktop.MainWindow)?.Clipboard,
            ISingleViewApplicationLifetime singleView => TopLevel.GetTopLevel(singleView.MainView)?.Clipboard,
            _ => null
        };
    }
}
