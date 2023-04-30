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
    public async Task<bool> CopyTextAsync(string text)
    {
        if (GetClipboard() is { } clipboard)
        {
            await clipboard.SetTextAsync(text);
            return true;
        }

        return false;
    }

    private static IClipboard? GetClipboard()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            if (TopLevel.GetTopLevel(lifetime.MainWindow)?.Clipboard is { } clipboard)
                return clipboard;
        }
        return null;
    }
}
