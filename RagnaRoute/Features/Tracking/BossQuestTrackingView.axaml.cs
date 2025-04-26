using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RagnaRoute.Services;
using RagnaRoute.ViewModels;
using System.Threading.Tasks;

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
        if (sender is Button { DataContext: BossQuestViewModel { WarpLocation: string location } })
        {
            var result = await _clipboardService.CopyTextAsync(location);

            if (result)
            {
                popup.PlacementTarget = (Button)sender;
                popup.IsOpen = true;

                await Task.Delay(1000);
                popup.IsOpen = false;
                popup.PlacementTarget = null;
            }
        }
    }
}
