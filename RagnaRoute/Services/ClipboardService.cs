using Avalonia;
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
        if (Application.Current?.Clipboard is not null)
        {
            await Application.Current.Clipboard.SetTextAsync(text);
            return true;
        }

        return false;
    }
}
