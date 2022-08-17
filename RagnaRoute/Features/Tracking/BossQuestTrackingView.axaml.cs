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

    private async void InfoButton_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not string model)
            return;

        await _clipboardService.CopyTextAsync(model);
    }
}
