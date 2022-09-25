using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RagnaRoute.Services;
using RagnaRoute.ViewModels;

namespace RagnaRoute.Views;
public partial class BossQuestTrackingView : UserControl
{
    private readonly IClipboardService _clipboardService;

    public BossQuestTrackingView()
    {
        InitializeComponent();

        _clipboardService = new ClipboardService();
    }

    private async void WarpLocation_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BossQuestViewModel vm && vm.WarpLocation is string location)
            await _clipboardService.CopyTextAsync(location);
    }
}
