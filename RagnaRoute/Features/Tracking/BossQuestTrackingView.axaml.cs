using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RagnaRoute.Services;

namespace RagnaRoute.Views;
public partial class BossQuestTrackingView : UserControl
{
    private readonly IClipboardService _clipboardService;

    public BossQuestTrackingView()
    {
        InitializeComponent();

        _clipboardService = new ClipboardService();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void WarpLocation_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: string location })
            await _clipboardService.CopyTextAsync(location);
    }
}
